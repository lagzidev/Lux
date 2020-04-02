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
            signature.Require<SpriteTexture>();
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
                var spriteTexture = World.Unpack<SpriteTexture>(entity);
                var transform = World.Unpack<Transform>(entity);

                float transformX = transform.X;
                float transformY = transform.Y;

                if (World.TryUnpack(entity, out Parent parent))
                {
                    var parentTransform = World.Unpack<Transform>(parent.ParentEntity);
                    transformX += parentTransform.X;
                    transformY += parentTransform.Y;
                }

                // If the scale is zero the sprite won't show, prevent that
                if (sprite.Scale == Vector2.Zero) sprite.Scale = Vector2.One;

                _spriteBatch.Draw(
                    loadedTextures.Textures[spriteTexture.TextureName],
                    new Vector2(transformX, transformY),
                    new Rectangle(0, 0, sprite.Width, sprite.Height),
                    sprite.Color,
                    sprite.Rotation,
                    Vector2.Zero,
                    sprite.Scale,
                    sprite.SpriteEffects,
                    0.8f);
            }

            _spriteBatch.End();
        }
    }
}
