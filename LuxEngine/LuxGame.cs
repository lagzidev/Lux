using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LuxEngine;

namespace Haven
{
    public class LuxGame : Game
    {
        private List<World> worlds;
        private GraphicsDeviceManager graphicsDeviceManager;

        public LuxGame(string windowTitle)
        {
            Window.Title = windowTitle;

            worlds = new List<World>();
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";
        }

        public World CreateWorld()
        {
            World newWorld = new World();
            worlds.Add(newWorld);

            return newWorld;
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            worlds.ForEach(x => x.Init(graphicsDeviceManager));
        }

        /// <summary>
        /// Automatically called when your game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            worlds.ForEach(x => x.LoadContent(GraphicsDevice, Content));
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            worlds.ForEach(x => x.PreUpdate(gameTime));
            worlds.ForEach(x => x.Update(gameTime));
            worlds.ForEach(x => x.PostUpdate(gameTime));
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            worlds.ForEach(x => x.Draw(gameTime));

            base.Draw(gameTime);
        }
    }
}