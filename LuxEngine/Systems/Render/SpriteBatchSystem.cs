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
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<VirtualResolutionSingleton>();
            signature.Using<TransformMatrixSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(
                World.GraphicsDeviceManager.GraphicsDevice));
        }

        protected override void LoadContent()
        {
            var virtualResolution = World.UnpackSingleton<VirtualResolutionSingleton>();
            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();

            // Must be recreated when device is reset (and LoadContent is called on device reset)
            spriteBatchSingleton.RenderTarget = new RenderTarget2D(
                World.GraphicsDeviceManager.GraphicsDevice,
                virtualResolution.VWidth,
                virtualResolution.VHeight);
        }

        protected override void PreDraw(GameTime gameTime)
        {
            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();
            var virtualResolution = World.UnpackSingleton<VirtualResolutionSingleton>();

            World.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(spriteBatchSingleton.RenderTarget);
            World.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Beige);

            spriteBatchSingleton.SpriteBatch.Begin(
                SpriteSortMode.BackToFront, // Defferred?
                BlendState.AlphaBlend,
                SamplerState.PointClamp, // Wrap?
                DepthStencilState.Default,
                RasterizerState.CullNone);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            var spriteBatchSingleton = World.UnpackSingleton<SpriteBatchSingleton>();
            SpriteBatch spriteBatch = spriteBatchSingleton.SpriteBatch;

            // End batch drawing on the render target
            spriteBatch.End();

            // Reset render target
            World.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            // Get transform matrix
            Matrix transformMatrix = Matrix.Identity;
            if (World.TryUnpackSingleton(out TransformMatrixSingleton transformMatrixSingleton))
            {
                transformMatrix = transformMatrixSingleton.TransformMatrix;
            }

            // Draw to the actual screen with the matrix for zoom, scaling, etc.
            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                transformMatrix * Matrix.CreateScale(1.01f));

            spriteBatch.Draw(
                spriteBatchSingleton.RenderTarget,
                new Vector2(0,0),
                new Rectangle(0,0, spriteBatchSingleton.RenderTarget.Width, spriteBatchSingleton.RenderTarget.Height),
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0.5f);

            spriteBatch.End();
        }
    }
}
