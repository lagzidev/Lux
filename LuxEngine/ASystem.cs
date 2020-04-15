using System.Collections.Generic;

namespace LuxEngine
{
    /// <summary>
    /// Base system class from which all systems are derived
    /// </summary>
    /// <typeparam name="T">System class that implements this ASystem</typeparam>
    /// <example>
    /// <code>
    /// public MovementSystem : ASystem<MovementSystem> { }
    /// </code>
    /// </example>
    public abstract class ASystem<T> : AInternalSystem
    {
    }

    public abstract class AInternalSystem
    {
        /// <summary>
        /// A reference to the world that the system is running on.
        /// Used internally to enable systems to manage entities without going
        /// through the World object themselves.
        /// </summary>
        private World _world;
        internal World World
        {
            private get
            {
                return _world;
            }
            set
            {
                _world = value;
                Signature = new SystemSignature(_world);
            }
        }

        /// <summary>
        /// The system's signature
        /// </summary>
        public SystemSignature Signature { get; private set; }

        /// <summary>
        /// A list of all entities that are currently registered to the system.
        /// Meaning these entities have all the required components and so on.
        /// </summary>
        public readonly HashSet<Entity> RegisteredEntities;

        /// <summary>
        /// <c>true</c> if the system is ready to load content, <c>false</c> otherwise.
        /// Useful for loading content in phases/handlers that can be executed
        /// before LoadContent is.
        /// </summary>
        /// <remarks>
        /// TODO: Find a better solution.
        /// </remarks>
        public bool IsReadyToLoadContent { get; private set; }

        public AInternalSystem()
        {
            _world = null;
            Signature = null;
            RegisteredEntities = new HashSet<Entity>();
            IsReadyToLoadContent = false;
        }

        private void UnregisterEntity(Entity entity)
        {
            // Unregister entity if exists in set
            if (!RegisteredEntities.Remove(entity))
            {
                return;
            }

            RunOnUnregisterEntity(entity);
        }

        #region Signature

        /// <summary>
        /// Define your system's signature here. The signature specifies which
        /// components the system needs. Only entities that match the system's
        /// signature will be registered to the system.
        /// </summary>
        /// <param name="signature">System's signature to mutate</param>
        public abstract void SetSignature(SystemSignature signature);

        /// <summary>
        /// Updates an entity's registration status based on its component mask.
        /// Registers it if the component mask matches, unregisters otherwise.
        /// </summary>
        /// <param name="entity">Entity to register/unregister</param>
        /// <param name="componentMask">The entity's component mask</param>
        internal void UpdateEntity(Entity entity, ComponentMask componentMask)
        {
            if (Matches(componentMask))
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

        /// <summary>
        /// Sets whether the singleton component mask matches the system.
        /// The system is disabled if the singleton component mask doesn't match.
        /// </summary>
        /// <param name="singletonComponentMask">Singleton entity's component mask</param>
        internal void UpdateSingleton(ComponentMask singletonComponentMask)
        {
            Signature.SingletonMatches = singletonComponentMask.Contains(Signature.SingletonComponentMask);
        }

        /// <summary>
        /// Determines whether a component mask matches the system.
        /// </summary>
        /// <param name="componentMask">Component mask to check if contained in the system</param>
        /// <returns>
        /// <c>true</c> if component mask matches the system,
        /// <c>false otherwise</c>
        /// </returns>
        private bool Matches(ComponentMask componentMask)
        {
            return componentMask.Contains(Signature.ComponentMask);
        }

        #endregion Signature

        #region Components

        /// <summary>
        /// Gets an entity's component
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Returned entity's component</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise.</returns>
        public bool Unpack<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            return World.Unpack(entity, out outComponent);
        }

        /// <summary>
        /// Gets a singleton's entity component
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="outComponent">Returned singleton component</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise.</returns>
        public bool UnpackSingleton<T>(out T outComponent) where T : AComponent<T>
        {
            return UnpackSingleton<T>(out outComponent);
        }

        /// <summary>
        /// Gets an entity's component in its previous state
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Returned component in its previous state</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise.</returns>
        public bool UnpackPrevious<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            return World.UnpackPrevious(entity, out outComponent);
        }

        /// <summary>
        /// Assigns a component to an entity
        /// </summary>
        /// <typeparam name="T">Type of the component added</typeparam>
        /// <param name="entity">Entity to assign the component to</param>
        /// <param name="component">Component to add to the entity</param>
        public void AddComponent<T>(Entity entity, T component) where T : AComponent<T>
        {
            World.AddComponent(entity, component);
        }

        /// <summary>
        /// Adds a component to the globally accessible singleton entity.
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="component">The component to add</param>
        public void AddSingletonComponent<T>(T component) where T : AComponent<T>
        {
            World.AddSingletonComponent(component);
        }

        /// <summary>
        /// Removes a component from an entity
        /// </summary>
        /// <typeparam name="T">Type of the component to remove</typeparam>
        /// <param name="entity">Entity to remove the component from</param>
        public void RemoveComponent<T>(Entity entity) where T : AComponent<T>
        {
            World.RemoveComponent<T>(entity);
        }

        // TODO: Add remove singleton component ?

        public Entity CreateEntity()
        {
            return World.CreateEntity();
        }

        public void DestroyEntity(Entity entity)
        {
            World.DestroyEntity(entity);
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Called before an entity is registered to the system.
        /// </summary>
        /// <param name="entity">Entity to unregister</param>
        protected virtual void OnRegisterEntity(Entity entity) { }
        protected void RunOnRegisterEntity(Entity entity)
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            OnRegisterEntity(entity);
        }


        /// <summary>
        /// Called after an entity is unregistered from the system.
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>
        /// TODO: Change this to OnRemovedComponent that will be called when a
        /// required component is removed from the system. This has the benefit
        /// of letting you know which component caused the removal.
        /// See: https://github.com/SanderMertens/flecs/blob/master/Manual.md#ecsonremove-event
        /// </remarks>
        protected virtual void OnUnregisterEntity(Entity entity) { }
        protected void RunOnUnregisterEntity(Entity entity)
        {
            if (!Signature.SingletonMatches)
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

            if (!Signature.SingletonMatches)
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

            if (!Signature.SingletonMatches)
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

            if (!Signature.SingletonMatches)
            {
                return;
            }

            LoadContent();
        }

        /// <summary>
        /// Use this to save state that will be used for interpolation later.
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Integrate() { }
        public void RunIntegrate()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            Integrate();
        }

        /// <summary>
        /// First phase to be called each frame.
        /// Use this to load external state (e.g. from disk, network, IO)
        /// that doesnn't 
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void LoadFrame() { }
        public void RunLoadFrame()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            LoadFrame();
        }

        /// <summary>
        /// Called before Update
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void PreUpdate() { }
        public void RunPreUpdate()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            PreUpdate();
        }

        /// <summary>
        /// Called each frame to update the game.
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Update() { }
        public void RunUpdate()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            Update();
        }

        /// <summary>
        /// Called after Update
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void PostUpdate() { }
        public void RunPostUpdate()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            PostUpdate();
        }

        /// <summary>
        /// First draw phase that is called every frame.
        /// Prepare state that PreDraw needs here.
        /// </summary>
        protected virtual void LoadDraw() { }
        public void RunLoadDraw()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            LoadDraw();
        }

        /// <summary>
        /// Called every frame before the game is ready to draw to the screen.
        /// Prepare draw related state here (update animations, etc.)
        /// </summary>
        protected virtual void PreDraw() { }
        public void RunPreDraw()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            PreDraw();
        }

        /// <summary>
        /// Called every frame when the game is ready to draw to the screen.
        /// </summary>
        protected virtual void Draw() { }
        public void RunDraw()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            Draw();
        }

        /// <summary>
        /// Called every frame after Draw.
        /// Use this for cleanup (e.g. SpriteBatch.End)
        /// </summary>
        protected virtual void PostDraw() { }
        public void RunPostDraw()
        {
            if (!Signature.SingletonMatches)
            {
                return;
            }

            PostDraw();
        }

        #endregion
    }
}
