using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.RecordHelper.Libraries
{
    public static class ReflectionHandler
    {
        public const BindingFlags bf = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static (Func<Value> getter, Action<Value> setter)? StaticGetGetterAndSetter<Value>(string name)
        {
            var f = typeof(Type).GetField(name);
            if (f is not null)
            {
                return f.StaticGetGetterAndSetter<Value>();
            }
            var p = typeof(Type).GetProperty(name);
            if (p is not null)
            {
                return p.StaticGetGetterAndSetter<Value>();
            }
            return null;
        }
        public static (Func<Type, Value> getter, Action<Type, Value> setter)? GetGetterAndSetter<Type, Value>(string name)
        {
            var f = typeof(Type).GetField(name);
            if (f is not null)
            {
                return f.GetGetterAndSetter<Type, Value>();
            }
            var p = typeof(Type).GetProperty(name);
            if (p is not null)
            {
                return p.GetGetterAndSetter<Type, Value>();
            }
            return null;
        }
        public static (Func<Value> getter, Action<Value> setter) StaticGetGetterAndSetter<Value>(this PropertyInfo property)
        {
            return (property.GetMethod!.CreateDelegate<Func<Value>>(), property.SetMethod!.CreateDelegate<Action<Value>>());
        }

        //almost as fast as proprty that saved in a lambda.
        public static (Func<Type, Value> getter, Action<Type, Value> setter) GetGetterAndSetter<Type, Value>(this PropertyInfo property)
        {
            return (property.GetMethod!.CreateDelegate<Func<Type, Value>>(), property.SetMethod!.CreateDelegate<Action<Type, Value>>());
        }
        //as fast as lambda.
        public static (Func<Type, Value> getter, Action<Type, Value> setter) GetGetterAndSetter<Type, Value>(this FieldInfo field)
        {
            DynamicMethod get = new("", typeof(Value), [typeof(Type)]);
            var ic = get.GetILGenerator();
            ic.Emit(OpCodes.Ldarg_0);
            ic.Emit(OpCodes.Ldfld, field);
            ic.Emit(OpCodes.Ret);

            DynamicMethod set = new("", typeof(void), [typeof(Type), typeof(Value)]);
            ic = set.GetILGenerator();
            ic.Emit(OpCodes.Ldarg_0);
            ic.Emit(OpCodes.Ldarg_1);
            ic.Emit(OpCodes.Stfld, field);
            ic.Emit(OpCodes.Ret);

            return (get.CreateDelegate<Func<Type, Value>>(), set.CreateDelegate<Action<Type, Value>>());
        }
        public static (Func<Value> getter, Action<Value> setter) StaticGetGetterAndSetter<Value>(this FieldInfo field)
        {
            DynamicMethod get = new("", typeof(Value), []);
            var ic = get.GetILGenerator();
            ic.Emit(OpCodes.Ldsfld, field);
            ic.Emit(OpCodes.Ret);

            DynamicMethod set = new("", typeof(void), [typeof(Value)]);
            ic = set.GetILGenerator();
            ic.Emit(OpCodes.Ldarg_0);
            ic.Emit(OpCodes.Stsfld, field);
            ic.Emit(OpCodes.Ret);

            return (get.CreateDelegate<Func<Value>>(), set.CreateDelegate<Action<Value>>());
        }
    }
}
