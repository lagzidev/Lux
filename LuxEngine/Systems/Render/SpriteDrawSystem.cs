using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteDrawSystem : BaseSystem<SpriteDrawSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
            signature.RequireSingleton<VirtualResolutionSingleton>();
            signature.RequireSingleton<ResolutionSettingsSingleton>();
            signature.RequireSingleton<SpriteBatchSingleton>();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Get loaded textures
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

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
                    DrawUtils.CalculateSpriteDepth(currentAnimationFrame.SpriteDepth));
            }
        }
    }
}
