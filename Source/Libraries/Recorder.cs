using Celeste.Mod.RecordHelper.Libraries;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Celeste.Mod.CelesteRecordHelper.Libraries
{
    public static class Recorder
    {
        static Recorder()
        {
            (movementcounterget, movementcounterset) = typeof(Platform).GetField("movementCounter", ReflectionHandler.bf)!.GetGetterAndSetter<Platform, Vector2>();
        }
        static Func<Platform, Vector2> movementcounterget;
        static Action<Platform, Vector2> movementcounterset;
        public static Recorder<Vector2, TEntity> CreatePositionRecorder<TEntity>() where TEntity : Entity
        {
            var r = new Recorder<Vector2, TEntity>();
            r.get = (entity) => entity.Position;
            r.set = (entity, val) =>
            {
                if (entity is Platform solid)
                {
                    Vector2 move = val - solid.Position - movementcounterget(solid);
                    Vector2 lift = move / Math.Abs(Engine.DeltaTime);
                    solid.MoveH(move.X, lift.X);
                    solid.MoveV(move.Y, lift.Y);
                }
                else
                {
                    entity.Position = val;
                }
            };
            return r;
        }
        /// <summary>
        /// Record EntityObject.{name[0]}.{name[1]}.{name[2]}. ... .
        /// </summary>
        public static Recorder<TValue, TEntity> CreateRefTypeRecorder<TValue, TEntity>(string[] name)
        {
            var r = new Recorder<TValue, TEntity>();
            DynamicMethod get = new("", typeof(TValue), [typeof(TEntity)]);
            var icg = get.GetILGenerator();
            icg.Emit(OpCodes.Ldarg_0);

            DynamicMethod set = new("", typeof(void), [typeof(TEntity), typeof(TValue)]);
            var ics = set.GetILGenerator();
            ics.Emit(OpCodes.Ldarg_0);
            foreach (var (s, i) in name.Zip(Enumerable.Range(1, name.Length)))
            {
                var f = typeof(TEntity).GetField(s, ReflectionHandler.bf);
                if (f is not null)
                {
                    icg.Emit(OpCodes.Ldfld, f);
                    if (i != name.Length)
                    {
                        ics.Emit(OpCodes.Ldfld, f);
                    }
                    else
                    {
                        ics.Emit(OpCodes.Ldarg_1);
                        ics.Emit(OpCodes.Stfld, f);
                    }
                }
                var p = typeof(TEntity).GetProperty(s, ReflectionHandler.bf);
                if (p is not null)
                {
                    icg.Emit(OpCodes.Call, p.GetMethod!);
                    if (i != name.Length)
                    {
                        ics.Emit(OpCodes.Call, p.GetMethod!);
                    }
                    else
                    {
                        ics.Emit(OpCodes.Ldarg_1);
                        ics.Emit(OpCodes.Call, p.SetMethod!);
                    }
                }
            }
            ics.Emit(OpCodes.Ret);
            icg.Emit(OpCodes.Ret);
            r.get = get.CreateDelegate<Func<TEntity, TValue>>();
            r.set = set.CreateDelegate<Action<TEntity, TValue>>();
            return r;
        }
        /// <summary>
        /// Record EntityObject.ComponentObject.{name}.
        /// </summary>
        public static Recorder<TValue, TEntity> CreateComponentRecorder<TValue, TCom, TEntity>(string name) where TEntity : Entity where TCom : Component
        {
            var r = new Recorder<TValue, TEntity>();
            var (get, set) = ReflectionHandler.GetGetterAndSetter<TCom, TValue>(name)!.Value;
            r.get = (e) => get(e.Components.Get<TCom>());
            r.set = (e, val) => set(e.Components.Get<TCom>(), val);
            return r;
        }
    }
    //interface
    public abstract class Recorder<TEntity>
    {
        public virtual void Awake(TEntity Entity) { }
        public abstract void Update(float time, TEntity Entity);
    }


    /// <summary>
    /// Supports one record stuff.
    /// </summary>
    /// <typeparam name="TValue">Value type to be tracked.</typeparam>
    /// <typeparam name="TEntity">Type to be reflected. ONLY used for default getter/setter and Awake/Update param.</typeparam>
    public class Recorder<TValue, TEntity> : Recorder<TEntity>
    {
        public Stack<(TValue value, float time)> rec = new();
        public Func<TEntity, TValue> get;
        public Action<TEntity, TValue> set;

        public Recorder()
        {
        }
        public Recorder(FieldInfo info)
        {
            (get, set) = info.GetGetterAndSetter<TEntity, TValue>();
        }
        public Recorder(PropertyInfo info)
        {
            (get, set) = info.GetGetterAndSetter<TEntity, TValue>();
        }
        //initialize and get first value.
        public override void Awake(TEntity Entity)
        {
            rec.Push((get(Entity), float.MaxValue));
        }
        //record value, or restore value.
        public override void Update(float time, TEntity entity)
        {
            if (time > 0)
            {
                var (val, tm) = rec.Peek();
                var now = get(entity);
                if (val is null && now is null || (val?.Equals(now) ?? false))
                {
                    rec.Pop();
                    rec.Push((val, tm + time));
                }
                else
                {
                    rec.Push((now, time));
                }
            }
            else
            {
            recalc:
                var (val, tm) = rec.Pop();
                time += tm;
                if (time > 0)
                {
                    rec.Push((val, time));
                }
                else
                {
                    goto recalc;
                }
                set(entity, val);
            }
        }
    }
    /// <summary>
    /// Do update logic in DelayedUpdate(), and make Update() blank.
    /// </summary>
    public interface RecordComponemtBase
    {
        public abstract void DelayedUpdate();
    }
    /// <summary>
    /// Supports all record stuff.
    /// and other RecordComponent stuff.
    /// </summary>
    /// <typeparam name="TEntity">Type to be recorded.</typeparam>
    public class EntityRecordComponemt<TEntity> : Component where TEntity : Entity
    {
        public List<Recorder<TEntity>> rec = new();
        //mark your component was able to work properly even time is reversed. 
        public HashSet<Component> stillUpdate = new();
        public static HashSet<Type> stillUpdateType = new();
        /// <summary>
        /// not used now.
        /// </summary>
        public enum TimeType
        {
            forward, backward, reverse, normal,
        }
        //temporarily const
        public TimeType dir = TimeType.normal;
        public EntityRecordComponemt() : base(true, false)
        {
        }
        /// <summary>
        /// If possible, use RecordRefType instead.
        /// That one should be better in performance.
        /// </summary>
        /// <returns>Return itself, so you can call next Record without repeat this component.</returns>
        public EntityRecordComponemt<TEntity> RecordComponent<C, V>(string name) where C : Component
        {
            rec.Add(Recorder.CreateComponentRecorder<V, C, TEntity>(name));
            return this;
        }
        /// <summary>
        /// Record EntityObject.{name[0]}.{name[1]}.{name[2]}. ... .
        /// </summary>
        public EntityRecordComponemt<TEntity> RecordRefType<V>(params string[] name)
        {
            rec.Add(Recorder.CreateRefTypeRecorder<V, TEntity>(name));
            return this;
        }
        /// <summary>
        /// shortcut to Record{V}(string) if you have got FieldInfo.
        /// </summary>
        public EntityRecordComponemt<TEntity> Record<V>(FieldInfo info)
        {
            rec.Add(new Recorder<V, TEntity>(info));
            return this;
        }
        /// <summary>
        /// shortcut to Record{V}(string) if you have got PropertyInfo.
        /// </summary>
        public EntityRecordComponemt<TEntity> Record<V>(PropertyInfo info)
        {
            rec.Add(new Recorder<V, TEntity>(info));
            return this;
        }
        /// <summary>
        /// Record EntityObject.{name}
        /// </summary>
        /// <typeparam name="V">type of EntityObject.{name}</typeparam>
        /// <returns>Return itself, so you can call next Record without repeat this component.</returns>
        public EntityRecordComponemt<TEntity> Record<V>(string name)
        {
            var f = typeof(TEntity).GetField(name, ReflectionHandler.bf);
            if (f is not null)
            {
                return Record<V>(f);
            }
            var p = typeof(TEntity).GetProperty(name, ReflectionHandler.bf);
            if (p is not null)
            {
                return Record<V>(p);
            }
            return this;
        }
        public override void EntityAwake()
        {
            base.EntityAwake();
            rec.Add(Recorder.CreatePositionRecorder<TEntity>());
            foreach (var r in rec)
            {
                r.Awake(EntityAs<TEntity>());
            }
            //Entity.PreUpdate += x => { (x.Active, active) = (active, x.Active); TryUpdate(); };
            //Entity.PostUpdate += x => { (x.Active, active) = (active, x.Active); };
        }
        //better to get Engine.DeltaTime from here.
        public float DeltaTime => dir switch
        {
            TimeType.forward => Math.Abs(Engine.DeltaTime),
            TimeType.backward => -Math.Abs(Engine.DeltaTime),
            TimeType.reverse => -Engine.DeltaTime,
            TimeType.normal => Engine.DeltaTime,
            _ => throw new NotImplementedException(), //but why?
        };

        public bool ShouldSkipUpdate()
        {
            return DeltaTime < 0;
        }

        //bool active = false;
        public void TryUpdate()
        {
            var rt = DeltaTime;
            foreach (var r in rec)
            {
                r.Update(rt, EntityAs<TEntity>());
            }
            if (rt < 0)
            {
                foreach (var r in Entity.Components)
                {
                    if (stillUpdateType.Contains(r.GetType()) || stillUpdate.Contains(r))
                    {
                        r.Update();
                    }
                }
            }
            foreach (var r in Entity.Components.OfType<RecordComponemtBase>())
            {
                r.DelayedUpdate();
            }
        }
    }

    public class UniversalRecordComponent<TObj>
    {
        public List<Recorder<TObj>> rec = new();
        /// <summary>
        /// not used now.
        /// </summary>
        public enum TimeType
        {
            forward, backward, reverse, normal,
        }
        //temporarily const
        public const TimeType dir = TimeType.normal;
        public UniversalRecordComponent()
        {
        }
        public UniversalRecordComponent<TObj> RecordRefType<V>(params string[] name)
        {
            rec.Add(Recorder.CreateRefTypeRecorder<V, TObj>(name));
            return this;
        }
        public UniversalRecordComponent<TObj> Record<V>(FieldInfo info)
        {
            rec.Add(new Recorder<V, TObj>(info));
            return this;
        }
        public UniversalRecordComponent<TObj> Record<V>(PropertyInfo info)
        {
            rec.Add(new Recorder<V, TObj>(info));
            return this;
        }

        public UniversalRecordComponent<TObj> Record<V>(string name)
        {
            var f = typeof(TObj).GetField(name, ReflectionHandler.bf);
            if (f is not null)
            {
                return Record<V>(f);
            }
            var p = typeof(TObj).GetProperty(name, ReflectionHandler.bf);
            if (p is not null)
            {
                return Record<V>(p);
            }
            return this;
        }
        public void EntityAwake(TObj self)
        {
            foreach (var r in rec)
            {
                r.Awake(self);
            }
        }
        public float DeltaTime => dir switch
        {
            TimeType.forward => Math.Abs(Engine.DeltaTime),
            TimeType.backward => -Math.Abs(Engine.DeltaTime),
            TimeType.reverse => -Engine.DeltaTime,
            TimeType.normal => Engine.DeltaTime,
            _ => throw new NotImplementedException(), //but why?
        };
        public bool ShouldSkipUpdate(TObj self)
        {
            return DeltaTime < 0;
        }

        public void TryUpdate(TObj self)
        {
            var rt = DeltaTime;

            foreach (var r in rec)
            {
                r.Update(rt, self);
            }
            if (rt < 0)
            {
            }
        }
    }
}
