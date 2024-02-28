using Celeste.Mod.CelesteRecordHelper.Libraries;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.RecordHelper.Entities
{
    [CustomEntity("RecordHelper/RecordedSwapBlock")]
    // Token: 0x0200062B RID: 1579
    [Tracked(false)]
    public class RecordedSwapBlock : Solid
    {
        ////so it replaces Celeste.Audio.
        //RecordedAudio Audio = new();
        EntityRecordComponemt<RecordedSwapBlock> record;
        // Token: 0x06002762 RID: 10082 RVA: 0x000DBDE8 File Offset: 0x000D9FE8
        public RecordedSwapBlock(Vector2 position, float width, float height, Vector2 node, Themes theme)
        : base(position, width, height, false)
        {
            redAlpha = 1f;

            Theme = theme;
            start = Position;
            end = node;
            maxForwardSpeed = 360f / Vector2.Distance(start, end);
            maxBackwardSpeed = maxForwardSpeed * 0.4f;
            Direction.X = Math.Sign(end.X - start.X);
            Direction.Y = Math.Sign(end.Y - start.Y);
            Add(new DashListener
            {
                OnDash = new Action<Vector2>(OnDash)
            });
            int num = (int)MathHelper.Min(X, node.X);
            int num2 = (int)MathHelper.Min(Y, node.Y);
            int num3 = (int)MathHelper.Max(X + Width, node.X + Width);
            int num4 = (int)MathHelper.Max(Y + Height, node.Y + Height);
            moveRect = new Rectangle(num, num2, num3 - num, num4 - num2);
            MTexture mtexture;
            MTexture mtexture2;
            MTexture mtexture3;
            if (Theme == Themes.Moon)
            {
                mtexture = GFX.Game["objects/swapblock/moon/block"];
                mtexture2 = GFX.Game["objects/swapblock/moon/blockRed"];
                mtexture3 = GFX.Game["objects/swapblock/moon/target"];
            }
            else
            {
                mtexture = GFX.Game["objects/swapblock/block"];
                mtexture2 = GFX.Game["objects/swapblock/blockRed"];
                mtexture3 = GFX.Game["objects/swapblock/target"];
            }
            nineSliceGreen = new MTexture[3, 3];
            nineSliceRed = new MTexture[3, 3];
            nineSliceTarget = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    nineSliceGreen[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                    nineSliceRed[i, j] = mtexture2.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                    nineSliceTarget[i, j] = mtexture3.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            if (Theme == Themes.Normal)
            {
                Add(middleGreen = GFX.SpriteBank.CreateRecord("swapBlockLight"));
                Add(middleRed = GFX.SpriteBank.CreateRecord("swapBlockLightRed"));
            }
            else if (Theme == Themes.Moon)
            {
                Add(middleGreen = GFX.SpriteBank.CreateRecord("swapBlockLightMoon"));
                Add(middleRed = GFX.SpriteBank.CreateRecord("swapBlockLightRedMoon"));
            }
            Add(new LightOcclude(0.2f));
            Depth = -9999;

            Add(record = new());
            record
                .Record<Vector2>(nameof(Speed))
                .Record<bool>(nameof(Swapping))
                .Record<float>(nameof(lerp))
                .Record<int>(nameof(target))
                .Record<float>(nameof(speed))
                .Record<float>(nameof(returnTimer))
                .Record<float>(nameof(redAlpha))
                .RecordRefType<float>(nameof(path), nameof(redAlpha));
            PostUpdate += (x) => { dashed = false; };
            record.stillUpdate.Add(middleGreen!);
            record.stillUpdate.Add(middleRed!);
            //record.stillUpdate.Add(Audio);
            //Add(Audio);
        }

        // Token: 0x06002763 RID: 10083 RVA: 0x000DC120 File Offset: 0x000DA320
        public RecordedSwapBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset, data.Enum("theme", Themes.Normal))
        {
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Add(path = new PathRenderer(this));
        }
        // Token: 0x06002764 RID: 10084 RVA: 0x000DC160 File Offset: 0x000DA360
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
        }

        // Token: 0x06002765 RID: 10085 RVA: 0x000DC189 File Offset: 0x000DA389

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            Audio.Stop(moveSfx, true);
            Audio.Stop(returnSfx, true);
        }

        // Token: 0x06002766 RID: 10086 RVA: 0x000DC1AA File Offset: 0x000DA3AA

        public override void SceneEnd(Scene scene)
        {
            base.SceneEnd(scene);
            Audio.Stop(moveSfx, true);
            Audio.Stop(returnSfx, true);
        }

        // Token: 0x06002767 RID: 10087 RVA: 0x000DC1CC File Offset: 0x000DA3CC
        bool dashed = false;
        private void OnDash(Vector2 direction)
        {
            dashed = true;
        }

        // Token: 0x06002768 RID: 10088 RVA: 0x000DC2C4 File Offset: 0x000DA4C4
        public void orig_Update()
        {
            if (dashed)
            {
                Swapping = (lerp < 1f);
                target = 1;
                returnTimer = 0.8f;
                burst = (Scene as Level)!.Displacement.AddBurst(Center, 0.2f, 0f, 16f, 1f, null, null);
                if (lerp >= 0.2f)
                {
                    speed = maxForwardSpeed;
                }
                else
                {
                    speed = MathHelper.Lerp(maxForwardSpeed * 0.333f, maxForwardSpeed, lerp / 0.2f);
                }
                Audio.Stop(returnSfx, true);
                Audio.Stop(moveSfx, true);
                if (!Swapping)
                {
                    Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", Center);
                    goto update;
                }
                moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", Center);
            }
        update:
            base.Update();
            if (returnTimer > 0f)
            {
                returnTimer -= Engine.DeltaTime;
                if (returnTimer <= 0f)
                {
                    target = 0;
                    speed = 0f;
                    returnSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_return", Center);
                }
            }
            if (burst != null)
            {
                burst.Position = Center;
            }
            redAlpha = Calc.Approach(redAlpha, (target == 1) ? 0 : 1, Engine.DeltaTime * 32f);
            if (target == 0 && lerp == 0f)
            {
                middleRed.SetAnimationFrame(0);
                middleGreen.SetAnimationFrame(0);
            }
            if (target == 1)
            {
                speed = Calc.Approach(speed, maxForwardSpeed, maxForwardSpeed / 0.2f * Engine.DeltaTime);
            }
            else
            {
                speed = Calc.Approach(speed, maxBackwardSpeed, maxBackwardSpeed / 1.5f * Engine.DeltaTime);
            }
            float num = lerp;
            lerp = Calc.Approach(lerp, target, speed * Engine.DeltaTime);
            if (lerp != num)
            {
                Vector2 vector = (end - start) * speed;
                Vector2 position = Position;
                if (target == 1)
                {
                    vector = (end - start) * maxForwardSpeed;
                }
                if (lerp < num)
                {
                    vector *= -1f;
                }
                if (target == 1 && Scene.OnInterval(0.02f))
                {
                    MoveParticles(end - start);
                }
                MoveTo(Vector2.Lerp(start, end, lerp), vector);
                if (position != Position)
                {
                    Audio.Position(moveSfx, Center);
                    Audio.Position(returnSfx, Center);
                    if (Position == start && target == 0)
                    {
                        Audio.SetParameter(returnSfx, "end", 1f);
                        Audio.Play("event:/game/05_mirror_temple/swapblock_return_end", Center);
                    }
                    else if (Position == end && target == 1)
                    {
                        Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", Center);
                    }
                }
            }
            if (Swapping && lerp >= 1f)
            {
                Swapping = false;
            }
            StopPlayerRunIntoAnimation = (lerp <= 0f || lerp >= 1f);

        }
        public override void Update()
        {
            var _old_returnTimer = returnTimer;
            Vector2 _old_Position = Position;
            float _old_lerp = lerp;

            if (!record.ShouldSkipUpdate())
            {
                orig_Update();
            }
            record.TryUpdate();

            if (record.ShouldSkipUpdate())
            {
                if (dashed)
                {
                    Audio.Stop(returnSfx, true);
                    Audio.Stop(moveSfx, true);
                    if (!Swapping)
                    {
                        Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", Center);
                        goto update;
                    }
                    moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", Center);
                }
            update:
                base.Update();
                if (returnTimer > 0f)
                {
                    if (_old_returnTimer <= 0f)
                    {
                        Audio.SetParameter(returnSfx, "end", 1f);
                    }
                }
                if (lerp != _old_lerp)
                {
                    if (_old_Position != Position)
                    {
                        Audio.Position(moveSfx, Center);
                        Audio.Position(returnSfx, Center);
                        if (_old_Position == start && target == 0)
                        {
                            if (returnSfx.getParameter("end", out var ins) != FMOD.RESULT.OK
                                || ins.getValue(out var val) != FMOD.RESULT.OK
                                || val != 0)
                            {
                                returnSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_return", Center);
                            }
                            Audio.Play("event:/game/05_mirror_temple/swapblock_return_end", Center);
                        }
                        else if (_old_Position == end && target == 1)
                        {
                            Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", Center);
                        }
                    }
                }
            }
        }

        // Token: 0x06002769 RID: 10089 RVA: 0x000DC5C4 File Offset: 0x000DA7C4

        private void MoveParticles(Vector2 normal)
        {
            Vector2 position;
            Vector2 vector;
            float direction;
            float num;
            if (normal.X > 0f)
            {
                position = CenterLeft;
                vector = Vector2.UnitY * (Height - 6f);
                direction = 3.14159274f;
                num = Math.Max(2f, Height / 14f);
            }
            else if (normal.X < 0f)
            {
                position = CenterRight;
                vector = Vector2.UnitY * (Height - 6f);
                direction = 0f;
                num = Math.Max(2f, Height / 14f);
            }
            else if (normal.Y > 0f)
            {
                position = TopCenter;
                vector = Vector2.UnitX * (Width - 6f);
                direction = -1.57079637f;
                num = Math.Max(2f, Width / 14f);
            }
            else
            {
                position = BottomCenter;
                vector = Vector2.UnitX * (Width - 6f);
                direction = 1.57079637f;
                num = Math.Max(2f, Width / 14f);
            }
            particlesRemainder += num;
            int num2 = (int)particlesRemainder;
            particlesRemainder -= num2;
            vector *= 0.5f;
            SceneAs<Level>().Particles.Emit(P_Move, num2, position, vector, direction);
        }

        // Token: 0x0600276A RID: 10090 RVA: 0x000DC740 File Offset: 0x000DA940

        public override void Render()
        {
            Vector2 vector = Position + Shake;
            if (lerp != target && speed > 0f)
            {
                Vector2 value = (end - start).SafeNormalize();
                if (target == 1)
                {
                    value *= -1f;
                }
                float num = speed / maxForwardSpeed;
                float num2 = 16f * num;
                int num3 = 2;
                while (num3 < num2)
                {
                    DrawBlockStyle(vector + value * num3, Width, Height, nineSliceGreen, middleGreen, Color.White * (1f - num3 / num2));
                    num3 += 2;
                }
            }
            if (redAlpha < 1f)
            {
                DrawBlockStyle(vector, Width, Height, nineSliceGreen, middleGreen, Color.White);
            }
            if (redAlpha > 0f)
            {
                DrawBlockStyle(vector, Width, Height, nineSliceRed, middleRed, Color.White * redAlpha);
            }
        }

        // Token: 0x0600276B RID: 10091 RVA: 0x000DC884 File Offset: 0x000DAA84

        private void DrawBlockStyle(Vector2 pos, float width, float height, MTexture[,] ninSlice, Sprite middle, Color color)
        {
            int num = (int)(width / 8f);
            int num2 = (int)(height / 8f);
            ninSlice[0, 0].Draw(pos + new Vector2(0f, 0f), Vector2.Zero, color);
            ninSlice[2, 0].Draw(pos + new Vector2(width - 8f, 0f), Vector2.Zero, color);
            ninSlice[0, 2].Draw(pos + new Vector2(0f, height - 8f), Vector2.Zero, color);
            ninSlice[2, 2].Draw(pos + new Vector2(width - 8f, height - 8f), Vector2.Zero, color);
            for (int i = 1; i < num - 1; i++)
            {
                ninSlice[1, 0].Draw(pos + new Vector2(i * 8, 0f), Vector2.Zero, color);
                ninSlice[1, 2].Draw(pos + new Vector2(i * 8, height - 8f), Vector2.Zero, color);
            }
            for (int j = 1; j < num2 - 1; j++)
            {
                ninSlice[0, 1].Draw(pos + new Vector2(0f, j * 8), Vector2.Zero, color);
                ninSlice[2, 1].Draw(pos + new Vector2(width - 8f, j * 8), Vector2.Zero, color);
            }
            for (int k = 1; k < num - 1; k++)
            {
                for (int l = 1; l < num2 - 1; l++)
                {
                    ninSlice[1, 1].Draw(pos + new Vector2(k, l) * 8f, Vector2.Zero, color);
                }
            }
            if (middle != null)
            {
                middle.Color = color;
                middle.RenderPosition = pos + new Vector2(width / 2f, height / 2f);
                middle.Render();
            }
        }

        // Token: 0x040020F7 RID: 8439
        public static ParticleType P_Move { get => SwapBlock.P_Move; }

        // Token: 0x040020F8 RID: 8440
        private const float ReturnTime = 0.8f;

        // Token: 0x040020F9 RID: 8441
        public Vector2 Direction;

        // Token: 0x040020FA RID: 8442
        public bool Swapping;

        // Token: 0x040020FB RID: 8443
        public Themes Theme;

        // Token: 0x040020FC RID: 8444
        private Vector2 start;

        // Token: 0x040020FD RID: 8445
        private Vector2 end;

        // Token: 0x040020FE RID: 8446
        private float lerp;

        // Token: 0x040020FF RID: 8447
        private int target;

        // Token: 0x04002100 RID: 8448
        private Rectangle moveRect;

        // Token: 0x04002101 RID: 8449
        private float speed;

        // Token: 0x04002102 RID: 8450
        private float maxForwardSpeed;

        // Token: 0x04002103 RID: 8451
        private float maxBackwardSpeed;

        // Token: 0x04002104 RID: 8452
        private float returnTimer;

        // Token: 0x04002105 RID: 8453
        private float redAlpha;

        // Token: 0x04002106 RID: 8454
        private MTexture[,] nineSliceGreen;

        // Token: 0x04002107 RID: 8455
        private MTexture[,] nineSliceRed;

        // Token: 0x04002108 RID: 8456
        private MTexture[,] nineSliceTarget;

        // Token: 0x04002109 RID: 8457
        private Sprite middleGreen;

        // Token: 0x0400210A RID: 8458
        private Sprite middleRed;

        // Token: 0x0400210B RID: 8459
        private PathRenderer path;

        // Token: 0x0400210C RID: 8460
        private EventInstance moveSfx;

        // Token: 0x0400210D RID: 8461
        private EventInstance returnSfx;

        // Token: 0x0400210E RID: 8462
        private DisplacementRenderer.Burst burst;

        // Token: 0x0400210F RID: 8463
        private float particlesRemainder;

        // Token: 0x0200062C RID: 1580
        public enum Themes
        {
            // Token: 0x04002111 RID: 8465
            Normal,
            // Token: 0x04002112 RID: 8466
            Moon
        }

        // Token: 0x0200062D RID: 1581
        private class PathRenderer : Entity
        {
            // Token: 0x0600276C RID: 10092 RVA: 0x000DCAA0 File Offset: 0x000DACA0

            public PathRenderer(RecordedSwapBlock block)
            : base(block.Position)
            {
                clipTexture = new MTexture();
                this.block = block;
                Depth = 8999;
                pathTexture = GFX.Game["objects/swapblock/path" + ((block.start.X == block.end.X) ? "V" : "H")];
                timer = Calc.Random.NextFloat();
            }

            // Token: 0x0600276D RID: 10093 RVA: 0x000DCB24 File Offset: 0x000DAD24

            public override void Update()
            {
                base.Update();
                timer += Engine.DeltaTime * 4f;
            }

            // Token: 0x0600276E RID: 10094 RVA: 0x000DCB44 File Offset: 0x000DAD44

            public override void Render()
            {
                if (block.Theme != Themes.Moon)
                {
                    for (int i = block.moveRect.Left; i < block.moveRect.Right; i += pathTexture.Width)
                    {
                        for (int j = block.moveRect.Top; j < block.moveRect.Bottom; j += pathTexture.Height)
                        {
                            pathTexture.GetSubtexture(0, 0, Math.Min(pathTexture.Width, block.moveRect.Right - i), Math.Min(pathTexture.Height, block.moveRect.Bottom - j), clipTexture);
                            clipTexture.DrawCentered(new Vector2(i + clipTexture.Width / 2, j + clipTexture.Height / 2), Color.White);
                        }
                    }
                }
                float scale = 0.5f * (0.5f + ((float)Math.Sin(timer) + 1f) * 0.25f);
                block.DrawBlockStyle(new Vector2(block.moveRect.X, block.moveRect.Y), block.moveRect.Width, block.moveRect.Height, block.nineSliceTarget, null!, Color.White * scale);
            }

            // Token: 0x04002113 RID: 8467
            private RecordedSwapBlock block;

            // Token: 0x04002114 RID: 8468
            private MTexture pathTexture;

            // Token: 0x04002115 RID: 8469
            private MTexture clipTexture;

            // Token: 0x04002116 RID: 8470
            private float timer;
        }
    }
}
