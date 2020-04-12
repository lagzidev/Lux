using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class LuxGame : Game
    {
        // ECS
        private static ECS _ecs;

        // References
        public static LuxGame Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        // Screen size
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int ViewPadding
        {
            get { return viewPadding; }
            set
            {
                viewPadding = value;
                Instance.UpdateView();
            }
        }
        public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix;

        private static int viewPadding = 0;

        /// <summary>
        /// Is currently resizing the window
        /// </summary>
        private static bool resizing;

        // Content directory
        public static string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Instance.Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Instance.Content.RootDirectory); }
#elif XBOXONE
            get { return Instance.Content.RootDirectory; }
#else
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    Instance.Content.RootDirectory);
            }
#endif
        }

        public LuxGame(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Window.Title = windowTitle;

            _ecs = new ECS();

            Width = width;
            Height = height;
            ScreenMatrix = Matrix.Identity;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferMultiSampling = false;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.ApplyChanges();

#if PS4 || XBOXONE
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
#else
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (fullscreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
                Graphics.IsFullScreen = false;
            }
#endif

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            IsFixedTimeStep = false;

            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing)
            {
                resizing = true;

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();

                resizing = false;
            }
        }
#endif

        protected virtual void OnGraphicsReset(object sender, EventArgs e)
        {
            UpdateView();
        }

        protected virtual void OnGraphicsCreate(object sender, EventArgs e)
        {
            UpdateView();
        }

        public static void SetWindowed(int width, int height)
        {
#if !CONSOLE
            if (width > 0 && height > 0)
            {
                resizing = true;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
                resizing = false;
            }
#endif
        }

        public static void SetFullscreen()
        {
#if !CONSOLE
            resizing = true;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            resizing = false;
#endif
        }

        /// <summary>
        /// Update the game's Viewport and ScreenMatrix
        /// </summary>
        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            int viewWidth;
            int viewHeight;

            // Get view size
            if (screenWidth / Width > screenHeight / Height)
            {
                viewWidth = (int)(screenHeight / Height * Width);
                viewHeight = (int)screenHeight;
            }
            else
            {
                viewWidth = (int)screenWidth;
                viewHeight = (int)(screenWidth / Width * Height);
            }

            // Apply view padding
            float aspect = viewHeight / (float)viewWidth;
            viewWidth -= ViewPadding * 2;
            viewHeight -= (int)(aspect * ViewPadding * 2);

            // Update screen matrix
            float scale = viewWidth / (float)Width;
            ScreenMatrix = Matrix.CreateScale(scale, scale, 0f);

            // Update viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - viewWidth / 2),
                Y = (int)(screenHeight / 2 - viewHeight / 2),
                Width = viewWidth,
                Height = viewHeight,
                MinDepth = 0f,
                MaxDepth = 1f
            };
        }

        // TODO
        //protected override void OnActivated(object sender, EventArgs args)
        //{
        //    base.OnActivated(sender, args);
        //}

        //protected override void OnDeactivated(object sender, EventArgs args)
        //{
        //    base.OnDeactivated(sender, args);
        //}

        /// <summary>
        /// Creates a new ECS world
        /// </summary>
        /// <returns>The newly created ECS world</returns>
        public static World CreateWorld()
        {
            return _ecs.CreateWorld();
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            _ecs.Initialize();
            base.Initialize(); // calls LoadContent
        }

        /// <summary>
        /// Automatically called after Initialize when your game launches to load any game assets (graphics, audio etc.)
        /// In XNA (not in FNA, maybe in MonoGame) it's called on device reset.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            _ecs.LoadContent();
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            _ecs.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            _ecs.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
