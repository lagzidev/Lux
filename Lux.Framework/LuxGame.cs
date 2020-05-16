using System;
using System.IO;
using System.Reflection;
using Lux.Framework.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lux.Framework
{
    public class LuxGame : Game
    {
        /// <summary>
        /// Game's title
        /// </summary>
        public static string Title { get; private set; }

        /// <summary>
        /// Provides global access to the game's instance
        /// </summary>
        public static LuxGame Instance;

        /// <summary>
        /// Content manager for managing assets (textures, audio, etc.)
        /// </summary>
        //public new static LuxContentManager Content;

        /// <summary>
        /// Provides easy access to the GraphicsDevice
        /// </summary>
        public new static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Base directory for all of the game's assets
        /// </summary>
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
                    ((Game)Instance).Content.RootDirectory);
            }
#endif
        }

        /// <summary>
        /// ECS feature that is responsible for all game logic.
        /// </summary>
        private static ECS.ECS _ecs;


        public LuxGame(int width, int height, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Window.Title = Title = windowTitle;

            _ecs = new ECS.ECS();

            Screen.Initialize(new GraphicsDeviceManager(this), width, height, fullscreen);

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            IsFixedTimeStep = false;

            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            // TODO: support scanodes ? https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_backbuffer_scale_nearest
        }
        

        // TODO onactivated
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
        protected static WorldHandle CreateWorld()
        {
            return _ecs.CreateWorld();
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize(); // calls LoadContent

            // Set the static graphics device
            GraphicsDevice = base.GraphicsDevice;

            // Call all ECS systems' initializers
            _ecs.Initialize();
        }

        // TODO: We got rid of loadcontent so make sure it's working on device reset in monogame.
        // TODO: Handle buffer overflow with recycled entities' generation int

        /// <summary>
        /// Called each frame to update the game.
        /// We implement our own 
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime.TotalGameTime.TotalSeconds);
            _ecs.Update();

            // If accumulated enough time to run a tick, start ticking
            while (Time.Accumulator >= Time.Timestep)
            {
                Time.Tick();
                _ecs.UpdateFixed();
            }

#if FNA
            // We don't call base.Update so we do this.
            // MonoGame only updates old-school XNA Components in Update which we dont care about. FNA's core FrameworkDispatcher needs
            // Update called though so we do so here.
            FrameworkDispatcher.Update();
#endif
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            Time.Draw(gameTime.ElapsedGameTime.TotalSeconds);
            _ecs.Draw();
        }
    }
}
