using Celeste.Mod.CelesteRecordHelper.Libraries;
using Celeste.Mod.RecordHelper.Libraries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.RecordHelper.Entities
{
    public static partial class RecordedSpriteExt
    {
        static Func<Sprite, Sprite, Sprite> CloneInto = typeof(Sprite).GetMethod("CloneInto", ReflectionHandler.bf)!.CreateDelegate<Func<Sprite, Sprite, Sprite>>();

        public static RecordedSprite CreateRecord(this SpriteBank spriteb, string id, RecordedSprite.RecordList what_to_record = RecordedSprite.RecordList.All)
        {
            if (spriteb.SpriteData.TryGetValue(id, out var sprite))
            {
                return (CloneInto(sprite.Sprite, new RecordedSprite(what_to_record)) as RecordedSprite)!;
            }

            throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
        }
    }

    public class RecordedSprite : Sprite,RecordComponemtBase
    {
        public enum RecordList : uint
        {
            None = 0,
            All = uint.MaxValue ^ NOTOrigin,
            OnEvent = 1 << 0,
            DiffSize = 1 << 1,
            FromComponent = 1 << 2,
            EffColor = 1 << 3,
            RawTime = 1 << 4,
            NOTOrigin = 1 << 5,
            Rate = 1 << 6,
        }
        public UniversalRecordComponent<Sprite> rec = new();
        public RecordedSprite(RecordList recr) : base(null, null)
        {
            if (recr.HasFlag(RecordList.OnEvent))
            {
                rec.Record<Action<string>>("OnFinish");
                rec.Record<Action<string>>("OnLoop");
                rec.Record<Action<string>>("OnFrameChange");
                rec.Record<Action<string>>("OnLastFrame");
                rec.Record<Action<string, string>>("OnChange");
            }
            if (recr.HasFlag(RecordList.DiffSize))
            {
                rec.Record<int>("width");
                rec.Record<int>("height");
                rec.Record<Vector2>("Scale");
                rec.Record<float>("Rotation");
            }
            if (recr.HasFlag(RecordList.FromComponent))
            {
                rec.Record<bool>("Active");
                rec.Record<bool>("Visible");
            }
            {
                rec.Record<Vector2>("Position");
            }
            if (recr.HasFlag(RecordList.EffColor))
            {
                rec.Record<Color>("Color");
                rec.Record<SpriteEffects>("Effects");
            }
            if (recr.HasFlag(RecordList.RawTime))
            {
                rec.Record<bool>("UseRawDeltaTime");
            }
            if (!recr.HasFlag(RecordList.NOTOrigin))
            {
                rec.Record<Vector2?>("Justify");
                rec.Record<Vector2>("Origin");
            }
            if (recr.HasFlag(RecordList.Rate))
            {
                rec.Record<float>("Rate");
            }
            {
                rec.Record<Animation>("currentAnimation")
                    .Record<float>("animationTimer")
                    .Record<bool>("Animating")
                    .Record<string>("CurrentAnimationID")
                    .Record<string>("LastAnimationID")
                    .Record<int>("CurrentAnimationFrame")
                    .Record<MTexture>("Texture");
            }
        }
        public override void EntityAwake()
        {

            base.EntityAwake();
            rec.EntityAwake(this);
        }
        public override void Update()
        {
        }

        public void DelayedUpdate()
        {
            if (!rec.ShouldSkipUpdate(this))
            {
                base.Update();
            }
            rec.TryUpdate(this);
        }
    }
}
