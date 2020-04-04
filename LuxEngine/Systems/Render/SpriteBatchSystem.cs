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
            signature.Using<TransformMatrixSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(
                World.GraphicsDeviceManager.GraphicsDevice));
        }

        protected override void PreDraw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

            Matrix transformMatrix = Matrix.Identity;
            if (World.TryUnpackSingleton(out TransformMatrixSingleton transformMatrixSingleton))
            {
                transformMatrix = transformMatrixSingleton.TransformMatrix;
            }

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                transformMatrix);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            spriteBatch.End();
        }
    }
}
