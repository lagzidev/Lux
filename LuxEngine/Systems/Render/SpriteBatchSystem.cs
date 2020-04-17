using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteBatchSingleton : AComponent<SpriteBatchSingleton>
    {
        public SpriteBatch Batch;
        public RenderTarget2D RenderTarget;

        public SpriteBatchSingleton(GraphicsDevice graphicsDevice)
        {
            Batch = new SpriteBatch(graphicsDevice);
            RenderTarget = null;
        }
    }

    public class SpriteBatchSystem : ASystem<SpriteBatchSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Camera>();
            signature.RequireSingleton<SpriteBatchSingleton>();
        }

        public override void InitSingleton()
        {
            AddSingletonComponent(new SpriteBatchSingleton(LuxGame.Graphics.GraphicsDevice));
        }

        public override void LoadContent()
        {
            UnpackSingleton(out SpriteBatchSingleton spriteBatchSingleton);

            spriteBatchSingleton.RenderTarget = new RenderTarget2D(
                LuxGame.Graphics.GraphicsDevice,
                LuxGame.Width,
                LuxGame.Height);
        }

        public override void PreDraw()
        {
            UnpackSingleton(out SpriteBatchSingleton spriteBatchSingleton);

            // Everything will be drawn to our render target
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(spriteBatchSingleton.RenderTarget);

            spriteBatchSingleton.Batch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                null,
                null);
        }

        public override void PostDraw()
        {
            LuxCommon.Assert(RegisteredEntities.Count == 1); // No support for multiple cameras yet

            UnpackSingleton(out SpriteBatchSingleton spriteBatchSingleton);
            SpriteBatch spriteBatch = spriteBatchSingleton.Batch;

            // End batch drawing on the render target
            spriteBatch.End();

            // Reset render target and draw to actual screen
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(null);
            LuxGame.Graphics.GraphicsDevice.Viewport = LuxGame.Viewport;
            LuxGame.Graphics.GraphicsDevice.Clear(Color.Black);

            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Camera camera);

                // Draw from render target to the actual screen using the matrices for zoom, scaling, etc.
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointWrap,
                    null,
                    null,
                    null,
                    camera.Matrix * LuxGame.ScreenMatrix); // Order matters

                // TODO: Make sure this works with multiple cameras

                spriteBatch.Draw(
                    spriteBatchSingleton.RenderTarget,
                    new Vector2(0, 0),
                    new Rectangle(0, 0, spriteBatchSingleton.RenderTarget.Width, spriteBatchSingleton.RenderTarget.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0.0f);

                spriteBatch.End();
            }
        }
    }
}
