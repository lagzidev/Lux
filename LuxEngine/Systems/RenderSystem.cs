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

    public class RenderSystem : BaseSystem<RenderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
            signature.RequireSingleton<ResolutionSingleton>();
            signature.Using<SpriteBatchSingleton>();
        }
        // TODO: Assert if using an optional without setting signature.Optional

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new SpriteBatchSingleton(
                World.GraphicsDeviceManager.GraphicsDevice));
        }

        protected override void LoadContent()
        {
            var loadedTexturesSingleton = World.UnpackSingleton<LoadedTexturesSingleton>();

            // Generate CornflowerBlue texture
            var texture = new Texture2D(World.GraphicsDeviceManager.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.CornflowerBlue });
            loadedTexturesSingleton.Textures.Add("_CornflowerBlue", texture);
        } 

        protected override void PreDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();
            var resolution = World.UnpackSingleton<ResolutionSingleton>();

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                resolution.ScaleMatrix); // Camera.GetTransformMatrix()

            spriteBatch.Draw(
                loadedTextures.Textures["_CornflowerBlue"],
                new Vector2(0, 0),
                new Rectangle(
                    0,
                    0,
                    resolution.VWidth,
                    resolution.VHeight),
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                DrawUtils.CalculateSpriteDepth(SpriteDepth.Wall));
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

        protected override void PostDraw(GameTime gameTime)
        {
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            spriteBatch.End();
        }
    }
}
