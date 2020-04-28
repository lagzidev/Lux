using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LuxEngine.ECS;
using System;

namespace LuxEngine.ECS
{
    public class SpriteBatchSingleton : AComponent<SpriteBatchSingleton>, ISingleton
    {
        public readonly SpriteBatch Batch;
        public readonly RenderTarget2D RenderTarget;

        public SpriteBatchSingleton(SpriteBatch spriteBatch, RenderTarget2D renderTarget)
        {
            Batch = spriteBatch;
            RenderTarget = renderTarget;
        }
    }

    public static class SpriteBatchSystem
    {
        public static void CreateSpriteBatchSingleton(Context context)
        {
            context.AddSingleton(new SpriteBatchSingleton(
                new SpriteBatch(LuxGame.Graphics.GraphicsDevice),
                new RenderTarget2D(
                    LuxGame.Graphics.GraphicsDevice,
                    LuxGame.Width,
                    LuxGame.Height)));
        }

        public static void BeginDrawToRenderTarget(SpriteBatchSingleton spriteBatch)
        {
            // Everything will be drawn to our render target
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(spriteBatch.RenderTarget);

            spriteBatch.Batch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                null,
                null);
        }

        public static void DrawToRenderTarget(
            SpriteBatchSingleton spriteBatch,
            LoadedTexturesSingleton loadedTextures,
            Sprite sprite,
            Transform transform,
            TextureComponent texture)
        {
            // TODO: Enable parents in a better way. Try without optionals first
            //float transformX = transform.X;
            //float transformY = transform.Y;
            //if (Unpack(entity, out Parent parent))
            //{
            //    if (Unpack(parent.ParentEntity, out Transform parentTransform))
            //    {
            //        transformX += parentTransform.X;
            //        transformY += parentTransform.Y;
            //    }
            //}

            Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
            var currentAnimationFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

            LuxCommon.Assert(currentAnimationFrame.Scale != Vector2.Zero);

            // Draw to sprite batch
            // We round the transform but it's important to note it gets
            // rounded anyways. This is because we're initially drawing it on a small RenderTarget
            // which can't be drawn on half a pixel so it rounds the vector for us.
            // For this reason, if we want to eliminate stutter, we have to round the
            // coordinates we give to the camera as well.
            spriteBatch.Batch.Draw(
                loadedTextures.Textures[texture.Name],
                CalcUtils.Round(transform.X, transform.Y),
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

        // TODO: Make sure this works with multiple cameras
        // TODO: Change camera component to transformMatrix component
        public static void DrawRenderTargetToScreen(SpriteBatchSingleton spriteBatch)
        {
            // End batch drawing on the render target
            spriteBatch.Batch.End();

            // Reset render target and draw to actual screen
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(null);
            LuxGame.Graphics.GraphicsDevice.Viewport = LuxGame.Viewport;
            LuxGame.Graphics.GraphicsDevice.Clear(Color.Black);

            // Draw from render target to the actual screen using the matrices for zoom, scaling, etc.
            spriteBatch.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointWrap,
                null,
                null,
                null,
                /* camera.Matrix * */ LuxGame.ScreenMatrix); // Order matters

            spriteBatch.Batch.Draw(
                spriteBatch.RenderTarget,
                new Vector2(0, 0),
                new Rectangle(0, 0, spriteBatch.RenderTarget.Width, spriteBatch.RenderTarget.Height),
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0.0f);

            spriteBatch.Batch.End();
        }
    }
}
