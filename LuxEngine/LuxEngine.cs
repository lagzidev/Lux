﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class LuxEngine : Game
    {
        public string Title;
        GraphicsDeviceManager graphics;

        SpriteBatch SpriteBatch;

        public List<GameObject> Objects = new List<GameObject>();
        public Map Map = new Map();

        public LuxEngine(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen)
        {
            Title = Window.Title = windowTitle;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";

            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(width, height); // Resolution our assets are based in
            Resolution.SetResolution(windowWidth, windowHeight, fullscreen);
        }

        /// <summary>
        /// This function is automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Camera.Initialize();
        }

        /// <summary>
        /// Automatically called when your game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Map.Load(Content);
            LoadLevel();
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            //map.Update(objects);
            UpdateObjects();
            UpdateCamera();

            //Update the things FNA handles for us underneath the hood:
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            //This will clear what's on the screen each frame, if we don't clear the screen will look like a mess:
            GraphicsDevice.Clear(Color.Black);

            Resolution.BeginDraw();

            SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, // SamplerState.LinearClamp
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                Camera.GetTransformMatrix());

            DrawObjects();
            Map.DrawWalls(SpriteBatch);
            SpriteBatch.End();

            //Draw the things FNA handles for us underneath the hood:
            base.Draw(gameTime);
        }

        public void LoadLevel()
        {
            Objects.Add(new Player(new Vector2(640, 360)));

            // Add walls
            Map.Walls.Add(new Wall(new Rectangle(256, 256, 256, 256)));
            Map.Walls.Add(new Wall(new Rectangle(0, 650, 1280, 128)));

            LoadObjects();
        }

        public void LoadObjects()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Initialize();
                Objects[i].Load(Content);
            }
        }

        public void UpdateObjects()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Update(Objects, Map);
            }
        }

        public void DrawObjects()
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Draw(SpriteBatch);
            }
        }

        private void UpdateCamera()
        {
            if (0 == Objects.Count)
            {
                return;
            }

            // objects[0] is the player
            Camera.Update(Objects[0].Position);
        }
    }
}