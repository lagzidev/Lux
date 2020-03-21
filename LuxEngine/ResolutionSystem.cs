using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
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

            var resolution = World.GlobalEntity.Unpack<ResolutionSingleton>();

            ApplyResolutionSettings(graphicsDeviceManager, resolution);
        }

        public static Viewport GetFullViewport(ResolutionSingleton resolution)
        {
            Viewport viewport = new Viewport();
            viewport.X = 0;
            viewport.Y = 0;
            viewport.Width = resolution.Width;
            viewport.Height = resolution.Height;

            return viewport;
        }

        static public Matrix CalculateTransformationMatrix(ResolutionSingleton resolution, int viewportWidth)
        {
            if (resolution.DirtyMatrix)
            {
                // Recreate scale matrix
                resolution.DirtyMatrix = false;
                resolution.ScaleMatrix = Matrix.CreateScale(
                    (float)viewportWidth / resolution.VWidth,
                    (float)viewportWidth / resolution.VWidth,
                    1f);
            }

            return resolution.ScaleMatrix;
        }

        private static float GetVirtualAspectRatio(ResolutionSingleton resolution)
        {
            return (float)resolution.VWidth / (float)resolution.VHeight;
        }

        public static Viewport GetVirtualViewport(ResolutionSingleton resolution)
        {
            float targetAspectRatio = GetVirtualAspectRatio(resolution);

            // Figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = resolution.Width;
            int height = (int)(width / targetAspectRatio + .5f);

            if (height > resolution.Height)
            {
                height = resolution.Height;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
                resolution.DirtyMatrix = true;
            }

            // set up the new viewport centered in the backbuffer
            Viewport viewport = new Viewport();

            viewport.X = (resolution.Width / 2) - (width / 2);
            viewport.Y = (resolution.Height / 2) - (height / 2);
            //virtualViewportX = viewport.X;
            //virtualViewportY = viewport.Y; // TODO: Make the virtual viewport available through the ResolutionSingleton
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            return viewport;
        }
        
        //private void SetVirtualResolution(ResolutionSingleton resolution, int virtualWidth, int virtualHeight)
        //{
        //    resolution.VWidth = virtualWidth;
        //    resolution.VHeight = virtualHeight;
        //    resolution.DirtyMatrix = true;
        //}

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
