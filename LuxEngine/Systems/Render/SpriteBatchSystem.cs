using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteBatchSingleton : BaseComponent<SpriteBatchSingleton>
    {
        public SpriteBatch SpriteBatch;

        public SpriteBatchSingleton(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }
    }

    public class SpriteBatchSystem : BaseSystem<SpriteBatchSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.Using<VirtualResolutionSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(
                World.GraphicsDeviceManager.GraphicsDevice));
        }

        protected override void PreDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

            Matrix scaleMatrix = Matrix.Identity;
            if (World.TryUnpackSingleton(out VirtualResolutionSingleton virtualResolution))
            {
                scaleMatrix = virtualResolution.ScaleMatrix;
            }

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                scaleMatrix); // Camera.GetTransformMatrix()
        }

        protected override void PostDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            spriteBatch.End();
        }
    }
}
