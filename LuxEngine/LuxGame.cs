using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
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
            //Camera.Initialize();
        }

        /// <summary>
        /// Automatically called when your game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            worlds.ForEach(x => x.LoadContent(GraphicsDevice, Content));

            //Map.Load(Content);
            //LoadLevel();
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            worlds.ForEach(x => x.Update(gameTime));

            //Input.Update();
            //map.Update(objects);
            //UpdateObjects();
            //UpdateCamera();

            //Update the things FNA handles for us underneath the hood:
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            worlds.ForEach(x => x.PreDraw(gameTime));
            worlds.ForEach(x => x.Draw(gameTime));
            worlds.ForEach(x => x.PostDraw(gameTime));

            base.Draw(gameTime);
        }

        //public void LoadLevel()
        //{
        //    Objects.Add(new Player(new Vector2(640, 360)));

        //    // Add walls
        //    Map.Walls.Add(new Wall(new Rectangle(256, 256, 256, 256)));
        //    Map.Walls.Add(new Wall(new Rectangle(0, 650, 1280, 128)));

        //    LoadObjects();
        //}

        //public void LoadObjects()
        //{
        //    for (int i = 0; i < Objects.Count; i++)
        //    {
        //        Objects[i].Initialize();
        //        Objects[i].Load(Content);
        //    }
        //}

        //public void UpdateObjects()
        //{
        //    for (int i = 0; i < Objects.Count; i++)
        //    {
        //        Objects[i].Update(Objects, Map);
        //    }
        //}

        //public void DrawObjects()
        //{
        //    for (int i = 0; i < Objects.Count; i++)
        //    {
        //        Objects[i].Draw(SpriteBatch);
        //    }
        //}

        //private void UpdateCamera()
        //{
        //    if (0 == Objects.Count)
        //    {
        //        return;
        //    }

        //    // objects[0] is the player
        //    Camera.Update(Objects[0].Position);
        //}
    }
}