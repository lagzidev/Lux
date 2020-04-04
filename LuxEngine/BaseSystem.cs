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
        public readonly ComponentMask ComponentMask;
        public readonly ComponentMask SingletonComponentMask;
        public bool SingletonMatches { get; private set; }

        /// <summary>
        /// A reference to the system that owns this signature.
        /// The reason for this backwards behaviour is to simplify the signature
        /// setting for the user, so that they could simply signature.Require<T>()
        /// </summary>
        private readonly InternalBaseSystem _system;

        public SystemSignature(InternalBaseSystem system)
        {
            ComponentMask = new ComponentMask();
            SingletonComponentMask = new ComponentMask();
            SingletonMatches = true;

            _system = system;
        }

        /// <summary>
        /// Registers the component to the world so that the system could use it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Using<T>() where T : BaseComponent<T>
        {
            _system.World.RegisterComponent<T>();
        }

        /// <summary>
        /// Requires any registered entities to have the given component.
        /// This registers the component to the world and adds it to the
        /// system's component mask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Require<T>() where T : BaseComponent<T>
        {
            _system.World.RegisterComponent<T>();
            ComponentMask.AddComponent<T>();
        }

        /// <summary>
        /// Requires the singleton component to be present for the system to
        /// function. IMPORTANT: Whenever the required singleton component doesn't
        /// exist the system won't run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RequireSingleton<T>() where T : BaseComponent<T>
        {
            _system.World.RegisterComponent<T>();

            // At this stage there aren't any components created, so this is false for sure
            SingletonMatches = false;
            SingletonComponentMask.AddComponent<T>();
        }

        /// <summary>
        /// Determines whether a component mask matches the system.
        /// </summary>
        /// <param name="componentMask">Component mask to check if contained in the system</param>
        /// <returns>
        /// <c>true</c> if component mask matches the system,
        /// <c>false otherwise</c>
        /// </returns>
        public bool Matches(ComponentMask componentMask)
        {
            return componentMask.Contains(ComponentMask);
        }

        /// <summary>
        /// Sets whether the singleton component mask matches the system.
        /// </summary>
        /// <param name="singletonComponentMask">Singleton entity's component mask</param>
        public void SetSingletonMatch(ComponentMask singletonComponentMask)
        {
            SingletonMatches = singletonComponentMask.Contains(SingletonComponentMask);
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
        private readonly SystemSignature _signature;

        public World World { get; set; }
        public readonly HashSet<Entity> RegisteredEntities;
        public bool IsReadyToLoadContent;

        public InternalBaseSystem()
        {
            _signature = new SystemSignature(this);

            World = null;
            RegisteredEntities = new HashSet<Entity>();
            IsReadyToLoadContent = false;
        }

        public void ApplySignature()
        {
            SetSignature(_signature);
        }

        /// <summary>
        /// The system is disabled if the singleton component mask doesn't match.
        /// </summary>
        /// <param name="singletonComponentMask">Singleton entity's component mask</param>
        public void UpdateSingleton(ComponentMask singletonComponentMask)
        {
            _signature.SetSingletonMatch(singletonComponentMask);
        }

        /// <summary>
        /// Updates an entity's registration status based on its component mask.
        /// Registers it if the component mask matches, unregisters otherwise.
        /// </summary>
        /// <param name="entity">Entity to register/unregister</param>
        /// <param name="componentMask">The entity's component mask</param>
        public void UpdateEntity(Entity entity, ComponentMask componentMask)
        {
            if (_signature.Matches(componentMask))
            {
                // Register entity if doesn't already exist in set
                if (RegisteredEntities.Contains(entity))
                {
                    return;
                }

                RunOnRegisterEntity(entity);

                RegisteredEntities.Add(entity);
            }
            else
            {
                UnregisterEntity(entity);
            }
        }

        private void UnregisterEntity(Entity entity)
        {
            // Unregister entity if exists in set
            if (RegisteredEntities.Remove(entity))
            {
                return;
            }

            RunOnUnregisterEntity(entity);
        }

        /// <summary>
        /// Define your system's signature here. The signature specifies which
        /// components the system needs. Only entities that match the system's
        /// signature will be registered to the system.
        /// </summary>
        /// <param name="signature">System's signature to mutate</param>
        protected abstract void SetSignature(SystemSignature signature);

        #region Handlers

        /// <summary>
        /// Called before an entity is registered to the system.
        /// </summary>
        /// <param name="entity">Entity to unregister</param>
        protected virtual void OnRegisterEntity(Entity entity) { }
        protected void RunOnRegisterEntity(Entity entity)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            OnRegisterEntity(entity);
        }


        /// <summary>
        /// Called after an entity is unregistered from the system.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnUnregisterEntity(Entity entity) { }
        protected void RunOnUnregisterEntity(Entity entity)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            OnUnregisterEntity(entity);
        }

        /// <summary>
        /// Called before an entity in the world is destroyed.
        /// </summary>
        /// <param name="entity">The entity that is about to be destroyed.</param>
        protected virtual void OnDestroyEntity(Entity entity) { }
        public void RunOnDestroyEntity(Entity entity)
        {
            UnregisterEntity(entity);

            if (!_signature.SingletonMatches)
            {
                return;
            }

            OnDestroyEntity(entity);
        }

        #endregion

        #region Phases

        /// <summary>
        /// Singleton components should be added here (and not in Init).
        /// This phase runs regardless of if the signature matches.
        /// </summary>
        protected virtual void InitSingleton() { }
        public void RunInitSingleton()
        {
            if (World == null)
            {
                LuxCommon.Assert(false);
                return;
            }

            InitSingleton();
        }

        /// <summary>
        /// Automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        protected virtual void Init() { }
        public void RunInit()
        {
            if (World == null)
            {
                LuxCommon.Assert(false);
                return;
            }

            //UpdateSingleton(World.GetSingletonMask()); // TODO Make GetSingletonMAsk private??

            if (!_signature.SingletonMatches)
            {
                return;
            }

            Init();
        }

        /// <summary>
        /// Automatically called when the game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        protected virtual void LoadContent() { }
        public void RunLoadContent()
        {
            IsReadyToLoadContent = true;

            if (!_signature.SingletonMatches)
            {
                return;
            }

            LoadContent();
        }

        /// <summary>
        /// Called before Update
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void PreUpdate(GameTime gameTime) { }
        public void RunPreUpdate(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            PreUpdate(gameTime);
        }

        /// <summary>
        /// Called each frame to update the game.
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Update(GameTime gameTime) { }
        public void RunUpdate(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            Update(gameTime);
        }

        /// <summary>
        /// Called after Update
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void PostUpdate(GameTime gameTime) { }
        public void RunPostUpdate(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            PostUpdate(gameTime);
        }

        /// <summary>
        /// This is called every frame when the game is ready to draw to the screen.
        /// </summary>
        protected virtual void PrePreDraw(GameTime gameTime) { }
        public void RunPrePreDraw(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            PrePreDraw(gameTime);
        }

        /// <summary>
        /// This is called every frame when the game is ready to draw to the screen.
        /// </summary>
        protected virtual void PreDraw(GameTime gameTime) { }
        public void RunPreDraw(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            PreDraw(gameTime);
        }

        /// <summary>
        /// This is called every frame when the game is ready to draw to the screen.
        /// </summary>
        protected virtual void Draw(GameTime gameTime) { }
        public void RunDraw(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            Draw(gameTime);
        }

        /// <summary>
        /// This is called after Draw
        /// </summary>
        protected virtual void PostDraw(GameTime gameTime) { }
        public void RunPostDraw(GameTime gameTime)
        {
            if (!_signature.SingletonMatches)
            {
                return;
            }

            PostDraw(gameTime);
        }

        #endregion
    }
}
