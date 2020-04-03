using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class ResolutionSingleton : BaseComponent<ResolutionSingleton>
    {
        public readonly int VWidth;
        public readonly int VHeight;
        public readonly int RequestedWidth;
        public readonly int RequestedHeight;
        public readonly bool FullScreen;

        public ResolutionSingleton(int virtualWidth, int virtualHeight, int windowWidth, int windowHeight, bool fullscreen)
        {
            VWidth = virtualWidth;
            VHeight = virtualHeight;
            RequestedWidth = windowWidth;
            RequestedHeight = windowHeight;
            FullScreen = fullscreen;
        }
    }

    public class ResolutionSystem : BaseSystem<ResolutionSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<ResolutionSingleton>();
            signature.Require<ScaleMatrixSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new ScaleMatrixSingleton());
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var resolution = World.Unpack<ResolutionSingleton>(entity);

            // Apply resolution and set new window size
            ApplyResolutionSettings(World.GraphicsDeviceManager, resolution);

            var scaleMatrix = World.UnpackSingleton<ScaleMatrixSingleton>();
            var worldViewport = World.GraphicsDeviceManager.GraphicsDevice.Viewport;

            scaleMatrix.Matrix = Matrix.CreateScale(
                (float)worldViewport.Width / resolution.VWidth,
                (float)worldViewport.Width / resolution.VWidth,
                1f);
        }

        protected override void PrePreDraw(GameTime gameTime)
        {
            if (RegisteredEntities.Count > 1) return;

            foreach (var entity in RegisteredEntities)
            {
                var resolution = World.Unpack<ResolutionSingleton>(entity);

                // Resize the viewport to the whole window
                World.GraphicsDeviceManager.GraphicsDevice.Viewport = new Viewport(0, 0, resolution.RequestedWidth, resolution.RequestedHeight);

                // Clear to Black
                World.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

                // Calculate Proper Viewport according to Aspect Ratio
                World.GraphicsDeviceManager.GraphicsDevice.Viewport = GetVirtualViewport(resolution);

                // Now sprites will be drawn only within the viewport and there will
                // be black bars on the sides
            }
        }

        private static Viewport GetVirtualViewport(ResolutionSingleton resolution)
        {
            float targetAspectRatio = GetVirtualAspectRatio(resolution);

            // Figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = resolution.RequestedWidth;
            int height = (int)(width / targetAspectRatio + .5f);

            if (height > resolution.RequestedHeight)
            {
                height = resolution.RequestedHeight;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
            }

            // set up the new viewport centered in the backbuffer
            Viewport viewport = new Viewport();

            viewport.X = (resolution.RequestedWidth / 2) - (width / 2);
            viewport.Y = (resolution.RequestedHeight / 2) - (height / 2);
            //virtualViewportX = viewport.X;
            //virtualViewportY = viewport.Y; // TODO: Make the virtual viewport available through the ResolutionSingleton
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            return viewport;
        }

        private static float GetVirtualAspectRatio(ResolutionSingleton resolution)
        {
            return (float)resolution.VWidth / (float)resolution.VHeight;
        }

        private void ApplyResolutionSettings(GraphicsDeviceManager graphicsDeviceManager, ResolutionSingleton resolution)
        {

#if XBOX360
           resolution.FullScreen = true;
#endif

            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (resolution.FullScreen == false)
            {
                if ((resolution.RequestedWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (resolution.RequestedHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphicsDeviceManager.PreferredBackBufferWidth = resolution.RequestedWidth;
                    graphicsDeviceManager.PreferredBackBufferHeight = resolution.RequestedHeight;
                    graphicsDeviceManager.IsFullScreen = resolution.FullScreen;
                    graphicsDeviceManager.PreferMultiSampling = true;
                    graphicsDeviceManager.ApplyChanges();
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == resolution.RequestedWidth) && (dm.Height == resolution.RequestedHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphicsDeviceManager.PreferredBackBufferWidth = resolution.RequestedWidth;
                        graphicsDeviceManager.PreferredBackBufferHeight = resolution.RequestedHeight;
                        graphicsDeviceManager.IsFullScreen = resolution.FullScreen;
                        graphicsDeviceManager.PreferMultiSampling = true;
                        graphicsDeviceManager.ApplyChanges();
                    }
                }
            }
        }
    }
}
