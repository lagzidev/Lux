using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteBatchSingleton : BaseComponent<SpriteBatchSingleton>
    {
        public SpriteBatch SpriteBatch;
        public RenderTarget2D RenderTarget;

        public SpriteBatchSingleton(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            RenderTarget = null;
        }
    }

    public class SpriteBatchSystem : BaseSystem<SpriteBatchSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Camera>();
            signature.RequireSingleton<SpriteBatchSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(LuxGame.Graphics.GraphicsDevice));
        }

        protected override void LoadContent()
        {
            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();

            spriteBatchSingleton.RenderTarget = new RenderTarget2D(
                LuxGame.Graphics.GraphicsDevice,
                LuxGame.Width,
                LuxGame.Height);
        }

        protected override void PreDraw(GameTime gameTime)
        {
            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();

            // Everything will be drawn to our render target
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(spriteBatchSingleton.RenderTarget);

            spriteBatchSingleton.SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.Default,
                RasterizerState.CullNone);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            LuxCommon.Assert(RegisteredEntities.Count == 1); // No support for multiple cameras yet

            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();
            SpriteBatch spriteBatch = spriteBatchSingleton.SpriteBatch;

            // End batch drawing on the render target
            spriteBatch.End();

            // Reset render target and draw to actual screen
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(null);
            LuxGame.Graphics.GraphicsDevice.Viewport = LuxGame.Viewport;
            LuxGame.Graphics.GraphicsDevice.Clear(Color.Black);

            foreach (var entity in RegisteredEntities)
            {
                Camera camera = World.Unpack<Camera>(entity);

                // Draw from render target to the actual screen using the matrices for zoom, scaling, etc.
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone,
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
