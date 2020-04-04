using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class VirtualResolutionSingleton : BaseComponent<VirtualResolutionSingleton>
    {
        public Matrix ScaleMatrix;
        public readonly int VWidth;
        public readonly int VHeight;

        public VirtualResolutionSingleton(int virtualWidth, int virtualHeight)
        {
            ScaleMatrix = Matrix.Identity;
            VWidth = virtualWidth;
            VHeight = virtualHeight;
        }
    }

    /// <summary>
    /// Responsible for initializing VirtualResolutionSingleton
    /// </summary>
    public class VirtualScaleSystem : BaseSystem<VirtualScaleSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<VirtualResolutionSingleton>();
            signature.Require<ResolutionSettingsSingleton>();
        }

        /// <summary>
        /// Initializes VirtualResolutionSingleton's ScaleMatrix
        /// </summary>
        protected override void OnRegisterEntity(Entity entity)
        {
            var virtualResolution = World.Unpack<VirtualResolutionSingleton>(entity);
            var worldViewport = World.GraphicsDeviceManager.GraphicsDevice.Viewport;

            virtualResolution.ScaleMatrix = Matrix.CreateScale(
                (float)worldViewport.Width / virtualResolution.VWidth,
                (float)worldViewport.Width / virtualResolution.VWidth,
                1f);
        }

        protected override void PreDraw(GameTime gameTime)
        {
            if (RegisteredEntities.Count == 0) return;

            foreach (var entity in RegisteredEntities)
            {
                var windowSize = World.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.Bounds;

                // Resize the viewport to the whole window
                World.GraphicsDeviceManager.GraphicsDevice.Viewport =
                    new Viewport(0, 0, windowSize.Width, windowSize.Height);

                // Clear to Black
                World.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

                // Calculate Proper Viewport according to Aspect Ratio
                World.GraphicsDeviceManager.GraphicsDevice.Viewport = GetVirtualViewport(entity);

                // Now sprites will be drawn only within the viewport
                // with black bars on the sides
            }
        }

        private Viewport GetVirtualViewport(Entity entity)
        {
            var gameSettings = World.Unpack<ResolutionSettingsSingleton>(entity);
            var virtualResolution = World.Unpack<VirtualResolutionSingleton>(entity);

            var graphicsDevice = World.GraphicsDeviceManager.GraphicsDevice;

            // Full screen sizes
            int viewportWidth = graphicsDevice.DisplayMode.Width;
            int viewportHeight = graphicsDevice.DisplayMode.Height;

            if (!gameSettings.FullScreen)
            {
                float targetAspectRatio = (float)virtualResolution.VWidth / (float)virtualResolution.VHeight;

                // Calculate sizes based on game settings
                viewportWidth = World.GraphicsDeviceManager.PreferredBackBufferWidth;
                viewportHeight = (int)(viewportWidth / targetAspectRatio + .5f); // Force round up
            }

            // Window sizes
            int windowWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
            int windowHeight = graphicsDevice.PresentationParameters.BackBufferHeight;

            //if (height > resolution.RequestedHeight)
            //{
            //    height = resolution.RequestedHeight;
            //    // PillarBox
            //    width = (int)(height * targetAspectRatio + .5f);
            //}

            // Set up the new viewport, centered
            Viewport viewport = new Viewport
            {
                X = (windowWidth / 2) - (viewportWidth / 2),
                Y = (windowHeight / 2) - (viewportHeight / 2),
                Width = viewportWidth,
                Height = viewportHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            return viewport;
        }
    }
}
