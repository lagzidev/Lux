using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine.Utils
{
    public class GraphicsManager : GraphicsDeviceManager
    {
        public GraphicsManager(Game game, int windowWidth, int windowHeight, bool fullscreen) : base(game)
        {
            this.PreferredBackBufferWidth = windowWidth;
            this.PreferredBackBufferHeight = windowHeight;
            this.IsFullScreen = fullscreen;

            this.SynchronizeWithVerticalRetrace = true;
            this.PreferMultiSampling = false;
            this.GraphicsProfile = GraphicsProfile.HiDef;
            this.PreferredBackBufferFormat = SurfaceFormat.Color;
            this.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            this.ApplyChanges();
        }
    }
}
