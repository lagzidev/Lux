using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    /// <summary>
    /// Represents a system's signature - what components it's interested in.
    /// </summary>
    public class SystemSignature
    {
        public ComponentMask ComponentMask { get; private set; }
        private readonly InternalBaseSystem _system;

        public SystemSignature(InternalBaseSystem system)
        {
            ComponentMask = new ComponentMask();
            _system = system;
        }

        public void Require<T>() where T : BaseComponent<T>
        {
            _system.World.RegisterComponent<T>();
            ComponentMask.AddComponent<T>();
        }
    }

    /// <summary>
    /// Base system class from which all systems are derived
    /// </summary>
    /// <typeparam name="T">System class that inherits this BaseSystem</typeparam>
    /// <example>
    /// <code>
    /// public SomeComponent : BaseSystem<SomeComponent> { }
    /// </code>
    /// </example>
    public abstract class BaseSystem<T> : InternalBaseSystem
    {
    }

    public abstract class InternalBaseSystem
    {
        public World World { get; set; }
        public readonly HashSet<Entity> RegisteredEntities;

        private readonly SystemSignature _signature;

        /// <summary>
        /// A string defining the components the system requires.
        /// Pass an empty array if no types are required.
        /// </summary>
        /// <returns>A list of component types the system requires</returns>
        //public SystemSignature Signature = new SystemSignature()

        /// <param name="requiredComponentTypes">
        /// Specifies which components are required for the entities this
        /// system operates on.
        /// </param>
        public InternalBaseSystem()
        {
            World = null;
            //ComponentMask = new ComponentMask(Array.ConvertAll<Type, int>(GetRequiredComponents, x => x));
            RegisteredEntities = new HashSet<Entity>();
            _signature = new SystemSignature(this);
        }

        public void ApplySignature()
        {
            SetSignature(_signature);
        }

        /// <summary>
        /// Updates an entity's registration status based on its component mask.
        /// Registers it if the component mask matches, unregisters otherwise.
        /// </summary>
        /// <param name="entity">Entity to register/unregister</param>
        /// <param name="componentMask">The entity's component mask</param>
        public void UpdateEntity(Entity entity, ComponentMask componentMask)
        {
            if (Matches(componentMask))
            {
                RegisterEntity(entity);
            }
            else
            {
                UnregisterEntity(entity);
            }
        }

        /// <summary>
        /// Checks if a component mask matches a system's signature.
        /// </summary>
        /// <param name="componentMask">An entity's component mask</param>
        /// <returns>
        /// <c>true</c> if the entity matches the system's signature,
        /// <c>false</c> otherwise.
        /// </returns>
        private bool Matches(ComponentMask componentMask)
        {
            return componentMask.Contains(_signature.ComponentMask);
        }

        /// <summary>
        /// Unregisters an entity from the system.
        /// </summary>
        /// <param name="entity"></param>
        private void UnregisterEntity(Entity entity)
        {
            // If entity is found and removed from system, call the handler
            if (RegisteredEntities.Remove(entity))
            {
                OnUnregisterEntity(entity);
            }
        }

        /// <summary>
        /// Registers an entity to the system. Does nothing if already registered.
        /// </summary>
        /// <param name="entity">Entity that was registered</param>
        private void RegisterEntity(Entity entity)
        {
            if (RegisteredEntities.Contains(entity))
            {
                return;
            }

            OnRegisterEntity(entity);
            RegisteredEntities.Add(entity);
        }

        /// <summary>
        /// Define your system's signature here. The signature specifies which
        /// components the system needs. Only entities that match the system's
        /// signature will be registered to the system.
        /// </summary>
        /// <param name="signature">Signature to define</param>
        protected abstract void SetSignature(SystemSignature signature);

        #region Handlers

        /// <summary>
        /// Called before an entity is registered to the system.
        /// </summary>
        /// <param name="entity">Entity to unregister</param>
        protected void OnRegisterEntity(Entity entity)
        {
        }

        /// <summary>
        /// Called after an entity is unregistered from the system.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnUnregisterEntity(Entity entity)
        {
        }

        /// <summary>
        /// Called before an entity in the world is destroyed.
        /// </summary>
        /// <param name="entity">The entity that is about to be destroyed.</param>
        public virtual void OnDestroyEntity(Entity entity)
        {
            UnregisterEntity(entity);
        }

        #endregion

        #region Phases

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

        #endregion
    }
}
