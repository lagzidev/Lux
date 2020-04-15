﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SpriteBatchSingleton : AComponent<SpriteBatchSingleton>
    {
        public SpriteBatch SpriteBatch;
        public RenderTarget2D RenderTarget;

        public SpriteBatchSingleton(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            RenderTarget = null;
        }
    }

    public class SpriteBatchSystem : ASystem<SpriteBatchSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Camera>();
            signature.RequireSingleton<SpriteBatchSingleton>();
        }

        protected override void InitSingleton()
        {
            _world.AddSingletonComponent(new SpriteBatchSingleton(LuxGame.Graphics.GraphicsDevice));
        }

        protected override void LoadContent()
        {
            var spriteBatchSingleton = _world.UnpackSingleton<SpriteBatchSingleton>();

            spriteBatchSingleton.RenderTarget = new RenderTarget2D(
                LuxGame.Graphics.GraphicsDevice,
                LuxGame.Width,
                LuxGame.Height);
        }

        protected override void PreDraw()
        {
            var spriteBatchSingleton = _world.UnpackSingleton<SpriteBatchSingleton>();

            // Everything will be drawn to our render target
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(spriteBatchSingleton.RenderTarget);

            spriteBatchSingleton.SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                null,
                null);
        }

        protected override void PostDraw()
        {
            LuxCommon.Assert(RegisteredEntities.Count == 1); // No support for multiple cameras yet

            var spriteBatchSingleton = _world.UnpackSingleton<SpriteBatchSingleton>();
            SpriteBatch spriteBatch = spriteBatchSingleton.SpriteBatch;

            // End batch drawing on the render target
            spriteBatch.End();

            // Reset render target and draw to actual screen
            LuxGame.Graphics.GraphicsDevice.SetRenderTarget(null);
            LuxGame.Graphics.GraphicsDevice.Viewport = LuxGame.Viewport;
            LuxGame.Graphics.GraphicsDevice.Clear(Color.Black);

            foreach (var entity in RegisteredEntities)
            {
                Camera camera = _world.Unpack<Camera>(entity);

                // Draw from render target to the actual screen using the matrices for zoom, scaling, etc.
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointWrap,
                    null,
                    null,
                    null,
                    camera.Matrix * LuxGame.ScreenMatrix); // Order matters

                // TODO: Make sure this works with multiple cameras

                spriteBatch.Draw(
                    spriteBatchSingleton.RenderTarget,
                    new Vector2(0, 0),
                    new Rectangle(0, 0, spriteBatchSingleton.RenderTarget.Width, spriteBatchSingleton.RenderTarget.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0.0f);

                spriteBatch.End();
            }
        }
    }
}
