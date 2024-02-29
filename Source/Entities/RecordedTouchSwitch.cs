using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;
using System;
using Celeste.Mod.CelesteRecordHelper.Libraries;


// WIP
namespace Celeste.Mod.RecordHelper.Entities
{
    [CustomEntity("RecordHelper/RecordedTouchSwitch")]
    [TrackedAs(typeof(TouchSwitch))]
    public class RecordedTouchSwitch : TouchSwitch
    {
        EntityRecordComponemt<RecordedTouchSwitch> EntityRecordComponemt;
        public RecordedTouchSwitch(Vector2 position) : base(position)
        {
            Depth = 2000;
            Add(Switch = new Switch(groundReset: false));
            Add(new PlayerCollider(OnPlayer, null, new Hitbox(30f, 30f, -15f, -15f)));
            Add(icon);
            Add(bloom = new BloomPoint(0f, 16f));
            bloom.Alpha = 0f;
            icon.Add("idle", "", 0f, default(int));
            icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), 0, 1, 2, 3, 4, 5);
            icon.Play("spin");
            icon.Color = inactiveColor;
            icon.CenterOrigin();
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new HoldableCollider(OnHoldable, new Hitbox(20f, 20f, -10f, -10f)));
            Add(new SeekerCollider(OnSeeker, new Hitbox(24f, 24f, -12f, -12f)));
            Switch.OnActivate = [MethodImpl(MethodImplOptions.NoInlining)] () =>
            {
                wiggler.Start();
                for (int i = 0; i < 32; i++)
                {
                    float num = Calc.Random.NextFloat(MathF.PI * 2f);
                    level.Particles.Emit(P_FireWhite, Position + Calc.AngleToVector(num, 6f), num);
                }

                icon.Rate = 4f;
            };
            Switch.OnFinish = delegate
            {
                ease = 0f;
            };
            Switch.OnStartFinished = [MethodImpl(MethodImplOptions.NoInlining)] () =>
            {
                icon.Rate = 0.1f;
                icon.Play("idle");
                icon.Color = finishColor;
                ease = 1f;
            };
            Add(wiggler = Wiggler.Create(0.5f, 4f, [MethodImpl(MethodImplOptions.NoInlining)] (float v) =>
            {
                pulse = Vector2.One * (1f + v * 0.25f);
            }));
            Add(new VertexLight(Color.White, 0.8f, 16, 32));
            Add(touchSfx = new SoundSource());

            Add(EntityRecordComponemt = new());
            //EntityRecordComponemt.Record<>()

        }

        public RecordedTouchSwitch(EntityData e, Vector2 offset) : this(e.Position + offset)
        {
        }



        public new void TurnOn()
        {
            if (!Switch.Activated)
            {
                touchSfx.Play("event:/game/general/touchswitch_any");
                if (Switch.Activate())
                {
                    SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
                    Add(new SoundSource("event:/game/general/touchswitch_last_cutoff"));
                }
            }
        }

        private new void OnPlayer(Player player)
        {
            TurnOn();
        }

        private new void OnHoldable(Holdable h)
        {
            TurnOn();
        }

        private new void OnSeeker(Seeker seeker)
        {
            if (SceneAs<Level>().InsideCamera(Position, 10f))
            {
                TurnOn();
            }
        }

        public override void Update()
        {
            timer += Engine.DeltaTime * 8f;
            ease = Calc.Approach(ease, (Switch.Finished || Switch.Activated) ? 1f : 0f, Engine.DeltaTime * 2f);
            icon.Color = Color.Lerp(inactiveColor, Switch.Finished ? finishColor : activeColor, ease);
            icon.Color *= 0.5f + ((float)Math.Sin(timer) + 1f) / 2f * (1f - ease) * 0.5f + 0.5f * ease;
            bloom.Alpha = ease;
            if (Switch.Finished)
            {
                if (icon.Rate > 0.1f)
                {
                    icon.Rate -= 2f * Engine.DeltaTime;
                    if (icon.Rate <= 0.1f)
                    {
                        icon.Rate = 0.1f;
                        wiggler.Start();
                        icon.Play("idle");
                        level.Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f);
                    }
                }
                else if (Scene.OnInterval(0.03f))
                {
                    Vector2 position = Position + new Vector2(0f, 1f) + Calc.AngleToVector(Calc.Random.NextAngle(), 5f);
                    level.ParticlesBG.Emit(P_Fire, position);
                }
            }

            base.Update();
        }

        public override void Render()
        {
            border.DrawCentered(Position + new Vector2(0f, -1f), Color.Black);
            border.DrawCentered(Position, icon.Color, pulse);
            base.Render();
        }

    }
}