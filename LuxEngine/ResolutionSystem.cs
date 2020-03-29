using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class ResolutionSingleton : BaseComponent<ResolutionSingleton>
    {
        public int VWidth;
        public int VHeight;
        public int Width;
        public int Height;
        public Matrix ScaleMatrix;
        public bool FullScreen;
        public bool DirtyMatrix; // Must be set to true when the virtual width/height changes
        public int VirtualViewportX;
        public int VirtualViewportY;

        public ResolutionSingleton(int virtualWidth, int virtualHeight, int windowWidth, int windowHeight, bool fullscreen)
        {
            VWidth = virtualWidth;
            VHeight = virtualHeight;
            Width = windowWidth;
            Height = windowHeight;
            FullScreen = fullscreen;
            DirtyMatrix = true;
            VirtualViewportX = 0;
            VirtualViewportY = 0;
        }
    }

    public class ResolutionSystem : BaseSystem<ResolutionSystem>
    {
        private GraphicsDeviceManager _graphicsDeviceManager;

        public ResolutionSystem() : base(ResolutionSingleton.ComponentType)
        {
        }

        public override void Init(GraphicsDeviceManager graphicsDeviceManager)
        {
            base.Init(graphicsDeviceManager);
            _graphicsDeviceManager = graphicsDeviceManager;

            foreach (var entity in RegisteredEntities)
            {
                var resolution = World.Unpack<ResolutionSingleton>(entity);
                ApplyResolutionSettings(graphicsDeviceManager, resolution);
            }
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
                if ((resolution.Width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (resolution.Height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphicsDeviceManager.PreferredBackBufferWidth = resolution.Width;
                    graphicsDeviceManager.PreferredBackBufferHeight = resolution.Height;
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
                    if ((dm.Width == resolution.Width) && (dm.Height == resolution.Height))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphicsDeviceManager.PreferredBackBufferWidth = resolution.Width;
                        graphicsDeviceManager.PreferredBackBufferHeight = resolution.Height;
                        graphicsDeviceManager.IsFullScreen = resolution.FullScreen;
                        graphicsDeviceManager.PreferMultiSampling = true;
                        graphicsDeviceManager.ApplyChanges();
                    }
                }
            }

            resolution.DirtyMatrix = true;

            resolution.Width = graphicsDeviceManager.PreferredBackBufferWidth;
            resolution.Height = graphicsDeviceManager.PreferredBackBufferHeight;
        }
    }
}
