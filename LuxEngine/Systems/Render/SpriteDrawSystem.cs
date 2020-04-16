using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteDrawSystem : ASystem<SpriteDrawSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.Require<TextureComponent>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
        }

        private float _transformX = 0;

        protected override void Draw()
        {
            // Get loaded textures
            UnpackSingleton(out LoadedTexturesSingleton loadedTextures);
            UnpackSingleton(out SpriteBatchSingleton spriteBatch);

            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Sprite sprite);
                Unpack(entity, out Transform transform);
                Unpack(entity, out TextureComponent texture);

                float transformX = transform.X;
                float transformY = transform.Y;
                if (Unpack(entity, out Parent parent))
                {
                    if (Unpack(parent.ParentEntity, out Transform parentTransform))
                    {
                        transformX += parentTransform.X;
                        transformY += parentTransform.Y;
                    }
                }

                Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
                var currentAnimationFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

                LuxCommon.Assert(currentAnimationFrame.Scale != Vector2.Zero);

                // Draw to sprite batch
                spriteBatch.Batch.Draw(
                    loadedTextures.Textures[texture.Name],
                    CalcUtils.Round(transformX, transformY),
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
                    DrawUtils.CalculateSpriteDepth(currentAnimationFrame.SpriteDepth));
            }
        }
    }
}
