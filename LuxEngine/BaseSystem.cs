using System;
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
        /// <summary>
        /// Gets the list of components the system requires.
        /// Pass an empty array if no types are required.
        /// </summary>
        /// <returns>A list of component types the system requires</returns>
        public abstract Type[] GetRequiredComponents();
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

        /// <summary>
        /// Called when an entity is registered to the system
        /// </summary>
        /// <param name="entity">Entity that was registered</param>
        public virtual void RegisterEntity(Entity entity, ContentManager contentManager)
        {
            RegisteredEntities.Add(entity);
        }

        /// <summary>
        /// Unregisters an entity from the system.
        /// </summary>
        /// <param name="entity">Entity to unregister</param>
        public virtual void UnregisterEntity(Entity entity)
        {
            RegisteredEntities.Remove(entity);
        }

        /// <summary>
        /// Automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        public virtual void Init(GraphicsDeviceManager graphicsDeviceManager)
        {
            LuxCommon.Assert(World != null);
        }

        /// <summary>
        /// Automatically called when the game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
        }

        /// <summary>
        /// Automatically called before an entity (any entity in the world) is destroyed.
        /// Do not attempt to unregister the entity yourself, it happens automatically.
        /// </summary>
        /// <param name="entity">The entity that is about to be destroyed.</param>
        public virtual void PreDestroyEntity(Entity entity)
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
