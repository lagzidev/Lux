﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    /// <summary>
    /// Base system class from which all systems are derived
    /// </summary>
    /// <typeparam name="T">System class that inherits this BaseSystem</typeparam>
    public abstract class BaseSystem<T> : InternalBaseSystem
    {
        public static int SystemId { get; set; }

        public BaseSystem(params int[] requiredComponentTypes) : base(requiredComponentTypes)
        {
        }
    }

    public abstract class InternalBaseSystem
    {
        public World World { protected get; set; }
        public List<Entity> RegisteredEntities { get; protected set; }
        public ComponentMask ComponentMask { get; private set; }

        /// <param name="requiredComponentTypes">
        /// Specifies which components are required for the entities this
        /// system operates on.
        /// </param>
        public InternalBaseSystem(params int[] requiredComponentTypes)
        {
            World = null;
            RegisteredEntities = new List<Entity>();
            ComponentMask = new ComponentMask(
                Array.ConvertAll(requiredComponentTypes, x => (int)x));
        }

        public void RegisterEntity(Entity entity)
        {
            RegisteredEntities.Add(entity);
        }

        public void UnregisterEntity(Entity entity)
        {
            RegisteredEntities.Remove(entity);
        }

        /// <summary>
        /// Automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        public virtual void Init(GraphicsDeviceManager graphicsDeviceManager)
        {
            Debug.Assert(World != null);
        }

        /// <summary>
        /// Automatically called when the game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called every frame when the game is ready to draw to the screen.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}
