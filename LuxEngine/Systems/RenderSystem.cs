using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class ScaleMatrixSingleton : BaseComponent<ScaleMatrixSingleton>
    {
        public Matrix Matrix;

        public ScaleMatrixSingleton()
        {
            Matrix = Matrix.Identity;
        }
    }

    public class SpriteBatchSingleton : BaseComponent<SpriteBatchSingleton>
    {
        public SpriteBatch SpriteBatch;

        public SpriteBatchSingleton(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }
    }

    public class RenderSystem : BaseSystem<RenderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
            signature.RequireSingleton<ScaleMatrixSingleton>();
        }
        // TODO: Assert if using an optional without setting signature.Optional

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(
                World.GraphicsDeviceManager.GraphicsDevice));
        }

        protected override void PreDraw(GameTime gameTime)
        {
            World.GraphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            var scaleMatrix = World.UnpackSingleton<ScaleMatrixSingleton>().Matrix;
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                scaleMatrix); // Camera.GetTransformMatrix() // todo: Black bars?
        }

        protected override void Draw(GameTime gameTime)
        {
            // Get loaded textures
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<Sprite>(entity);
                var transform = World.Unpack<Transform>(entity);

                float transformX = transform.X;
                float transformY = transform.Y;

                if (World.TryUnpack(entity, out Parent parent))
                {
                    var parentTransform = World.Unpack<Transform>(parent.ParentEntity);
                    transformX += parentTransform.X;
                    transformY += parentTransform.Y;
                }

                Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
                var currentAnimationFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

                LuxCommon.Assert(currentAnimationFrame.Scale != Vector2.Zero);

                // Draw to sprite batch
                var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

                spriteBatch.Draw(
                    loadedTextures.Textures[sprite.TextureName],
                    new Vector2(transformX, transformY),
                    new Rectangle(
                        currentAnimationFrame.TexturePositionX,
                        currentAnimationFrame.TexturePositionY,
                        currentAnimationFrame.Width,
                        currentAnimationFrame.Height),
                    currentAnimationFrame.Color,
                    currentAnimationFrame.Rotation,
                    Vector2.Zero,
                    currentAnimationFrame.Scale,
                    currentAnimationFrame.SpriteEffects,
                    0.8f);
            }
        }

        protected override void PostDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            spriteBatch.End();
        }
    }
}
