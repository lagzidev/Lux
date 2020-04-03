using System;
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

    public class RenderSystem : BaseSystem<RenderSystem>
    {
        SpriteBatch _spriteBatch;

        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
            signature.RequireSingleton<ScaleMatrixSingleton>();
        }
        // TODO: Assert if using an optional without setting signature.Optional

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(World.GraphicsDeviceManager.GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            World.GraphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            var scaleMatrix = World.UnpackSingleton<ScaleMatrixSingleton>().Matrix;

            _spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                scaleMatrix);
                // , Camera.GetTransformMatrix() // Black bars?

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

                var currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
                var currentAnimationFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

                LuxCommon.Assert(currentAnimationFrame.Scale != Vector2.Zero);

                _spriteBatch.Draw(
                    loadedTextures.Textures[sprite.TextureName],
                    new Vector2(transformX, transformY),
                    new Rectangle(0, 0, currentAnimationFrame.Width, currentAnimationFrame.Height),
                    currentAnimationFrame.Color,
                    currentAnimationFrame.Rotation,
                    Vector2.Zero,
                    currentAnimationFrame.Scale,
                    currentAnimationFrame.SpriteEffects,
                    0.8f);
            }

            _spriteBatch.End();
        }
    }
}
