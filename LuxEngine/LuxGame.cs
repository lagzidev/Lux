using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class LuxGame : Game
    {
        private List<World> _worlds;
        public GraphicsDeviceManager GraphicsDeviceManager;

        public LuxGame(string windowTitle)
        {
            Window.Title = windowTitle;

            _worlds = new List<World>();
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";
            //Window.AllowUserResizing = true; // TODO: Consider allowing this
        }

        public World CreateWorld()
        {
            World newWorld = new World(GraphicsDeviceManager, Content, Window);
            _worlds.Add(newWorld);

            return newWorld;
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            _worlds.ForEach(x => x.InitSingleton());
            _worlds.ForEach(x => x.Init());
            base.Initialize(); // calls LoadContent
        }

        /// <summary>
        /// Automatically called when your game launches to load any game assets (graphics, audio etc.)
        /// In XNA (not in FNA, maybe in MonoGame) it's called on device reset.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            _worlds.ForEach(x => x.LoadContent());
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            _worlds.ForEach(x => x.PreUpdate(gameTime));
            _worlds.ForEach(x => x.Update(gameTime));
            _worlds.ForEach(x => x.PostUpdate(gameTime));
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            _worlds.ForEach(x => x.PrePreDraw(gameTime));
            _worlds.ForEach(x => x.PreDraw(gameTime));
            _worlds.ForEach(x => x.Draw(gameTime));
            _worlds.ForEach(x => x.PostDraw(gameTime));

            base.Draw(gameTime);
        }
    }
}