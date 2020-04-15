using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteDrawSystem : ASystem<SpriteDrawSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.Require<TextureComponent>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
        }

        protected override void Draw()
        {
            // Get loaded textures
            var loadedTextures = _world.UnpackSingleton<LoadedTexturesSingleton>();
            var spriteBatch = _world.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

            foreach (var entity in RegisteredEntities)
            {
                var sprite = _world.Unpack<Sprite>(entity);
                var transform = _world.Unpack<Transform>(entity);
                var texture = _world.Unpack<TextureComponent>(entity);

                float transformX = transform.X;
                float transformY = transform.Y;
                if (_world.TryUnpack(entity, out Parent parent))
                {
                    var parentTransform = _world.Unpack<Transform>(parent.ParentEntity);
                    transformX += parentTransform.X;
                    transformY += parentTransform.Y;
                }

                Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
                var currentAnimationFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

                LuxCommon.Assert(currentAnimationFrame.Scale != Vector2.Zero);

                // Draw to sprite batch
                spriteBatch.Draw(
                    loadedTextures.Textures[texture.Name],
                    new Vector2((float)(transformX * Time.Alpha), (float)(transformY * Time.Alpha)),
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
