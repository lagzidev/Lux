﻿using System.Collections.Generic;

namespace LuxEngine.ECS
{
    internal class InternalWorld
    {
        /// <summary>
        /// A handle to the world for the user to interact with
        /// </summary>
        public readonly WorldHandle WorldHandle;

        /// <summary>
        /// Manages entities' IDs
        /// </summary>
        private readonly EntityGenerator _entityGenerator;

        /// <summary>
        /// An array of all entities in the world
        /// </summary>
        private readonly Entity[] _entities;

        /// <summary>
        /// Component managers that contains all component data
        /// </summary>
        private readonly Dictionary<int, BaseComponentManager> _componentManagers;

        /// <summary>
        /// An entity that is globally accessible from any system.
        /// To access it a system should signature.RequireSingleton<T>
        /// </summary>
        private readonly Entity _singletonEntity;


        public InternalWorld()
        {
            WorldHandle = new WorldHandle();
            _entityGenerator = new EntityGenerator();
            _entities = new Entity[HardCodedConfig.MAX_ENTITIES_PER_WORLD];
            _componentManagers = new Dictionary<int, BaseComponentManager>();
            _singletonEntity = CreateEntity();
        }

        /// <summary>
        /// Creates a new entity in the world
        /// </summary>
        /// <returns>The created entity</returns>
        public Entity CreateEntity()
        {
            Entity entity = _entityGenerator.CreateEntity();
            _entities[entity.Index] = entity;

            return entity;
        }

        /// <summary>
        /// Destroys an existing entity that belongs to the world
        /// </summary>
        /// <param name="entity">Entity to destroy</param>
        public void DestroyEntity(Entity entity)
        {
            // Signal systems that this entity is about to be destroyed
            WorldHandle.OnDestroyEntitySystems.Invoke(this, entity, WorldHandle.EntityFilter);

            // Remove entity's components
            foreach (var componentManager in _componentManagers)
            {
                componentManager.Value.RemoveComponent(entity);
            }

            // Destroy entity
            _entityGenerator.DestroyEntity(entity);
        }

        #region Unpacking

        /// <summary>
        /// Gets a component that belongs to an entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Returned component</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise</returns>
        public bool Unpack<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            // TODO: Support IExclude and IWrapper
            // If component implements IExclude, make sure it doesn't exist
            //if (typeof(T).IsAssignableFrom(typeof(IExclude)))
            //{

            //}

            // If component is a wrapper, extract the real component
            //if (typeof(T).IsAssignableFrom(typeof(IWrapper<>)))
            //{
            //}

            // Get component manager for the relevant component type
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                outComponent = null;
                return false;
            }

            // Get component if it exists
            if (!foundComponentManager.GetComponent(entity, out outComponent))
            {
                outComponent = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a component that belongs to the global singleton entity.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="outComponent">Returned component</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise</returns>
        public bool UnpackSingleton<T>(out T outComponent) where T : AComponent<T>
        {
            return Unpack(_singletonEntity, out outComponent);
        }

        #endregion Unpacking

        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="component">Component to return</param>
        public void AddComponent<T>(Entity entity, T component) where T : AComponent<T>
        {
            // Set the entity for the component
            component._entity = entity;

            // Get component manager for the relevant component type
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                LuxCommon.Assert(false); // The component wasn't included in any system
                return;
            }

            // If component already exists
            if (foundComponentManager.GetComponent(entity, out _))
            {
                LuxCommon.Assert(false); // Shouldn't happen, use RemoveComponent first
                RemoveComponent<T>(entity);
            }


            // Add component to system
            foundComponentManager.AddComponent(entity, component);

            // Call systems that subscribed to the OnAddComponent event
            WorldHandle.OnAddComponentSystems.Invoke(this, entity, WorldHandle.EntityFilter);
        }

        /// <summary>
        /// Adds a component to the singleton entity.
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="component">The component to add</param>
        public void AddSingletonComponent<T>(T component) where T : AComponent<T>
        {
            AddComponent(_singletonEntity, component);
        }

        // TODO: SET COMPONENT
        //public void SetComponent<T>(Entity entity, T component) where T : AComponent<T>
        //{

        //}

        /// <summary>
        /// Remove a component that belongs to an entity.
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="entity">The entity that owns the component</param>
        public void RemoveComponent<T>(Entity entity) where T : AComponent<T>
        {
            // Update the component manager
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                LuxCommon.Assert(false); // No component manager found!
                return;
            }

            foundComponentManager.RemoveComponent(entity);

            // Call systems that subscribed to the OnRemoveComponent event
            WorldHandle.OnRemoveComponentSystems.Invoke(this, entity, WorldHandle.EntityFilter);
        }

        /// <summary>
        /// Registers a component type to a world. Does nothing if already registered.
        /// </summary>
        /// <typeparam name="T">Component type to register to the world</typeparam>
        /// <returns>The component type's ID for the world</returns>
        public void Register<T>() where T : AComponent<T>
        {
            // Set the ComponentType for the component class
            AComponent<T>.SetComponentType();

            // Add a component manager for the type if doesn't exist
            if (!_componentManagers.ContainsKey(AComponent<T>.ComponentType))
            {
                var componentManager = new ComponentManager<T>();
                _componentManagers[AComponent<T>.ComponentType] = componentManager;
            }
        }

        ///// <summary>
        ///// Tells the world to keep track of the components' previous state.
        ///// </summary>
        ///// <typeparam name="T">The type of component of which state we keep</typeparam>
        //public void KeepPreviousState<T>() where T : AComponent<T>
        //{
        //    // Set the ComponentType in case it wasn't set already
        //    AComponent<T>.SetComponentType();

        //    // Add a component manager for the type if doesn't exist
        //    if (!_previousComponentManagers.ContainsKey(AComponent<T>.ComponentType))
        //    {
        //        var componentManager = new ComponentManager<T>();
        //        _previousComponentManagers[AComponent<T>.ComponentType] = componentManager;
        //    }
        //}

        /// <summary>
        /// Saves the current state of the components into a seperate dataset.
        /// Only copies components for which KeepPreviousState was called.
        /// </summary>
        //private void SavePreviousState<T>(T component) where T : AComponent<T>
        //{
        //    // TODO: Save the component into _previousComponentManager or alternatively,
        //    // find a more lightweight data storage solution. Something dynamic like
        //    // a Dictionary<Entity, Dictionary<ComponentType, Component> might be lighter
        //    // then a ComponentManager, but might be more demanding performance wise.
        //}

        private ComponentManager<T> _getComponentManager<T>() where T : AComponent<T>
        {
            if (AComponent<T>.ComponentType == -1)
            {
                return null;
            }

            return (ComponentManager<T>)_componentManagers[AComponent<T>.ComponentType];
        }

        #region Serialization

        /// <summary>
        /// Serializes all of the component managers and writes them into a
        /// TextWriter instance
        /// <para>All components (and their members' types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a member from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <param name="writer">Writer to write the serialized data into</param>
        //public void Serialize(BinaryWriter writer)
        //{
        //    // Serialize component managers
        //    writer.Write(_componentManagers.Count);

        //    foreach (var componentManager in _componentManagers)
        //    {
        //        writer.Write(componentManager.Key);
        //        componentManager.Value.Serialize(writer);
        //    }

        //    // TODO: DITCH BinaryFormatter for Protobuf or XML serializer.
        //    // It's not cross platform, weighs a lot, break through versions, etc. https://stackoverflow.com/questions/7964280/c-sharp-serialize-generic-listcustomobject-to-file

        //    // Serialize singleton entity
        //    IFormatter formatter = new BinaryFormatter();
        //    formatter.Serialize(writer.BaseStream, _singletonEntity);
        //}

        /// <summary>
        /// Deserializes a world from a reader and loads it
        /// </summary>
        /// <param name="reader">Reader to read the world data from</param>
        /// <returns>An array of component managers with entity data</returns>
        //private Dictionary<int, BaseComponentManager> DeserializeToComponentManagers(BinaryReader reader)
        //{
        //    // Get amount of component managers
        //    int componentManagersCount = reader.ReadInt32();
        //    var componentManagers = new Dictionary<int, BaseComponentManager>(componentManagersCount);

        //    // Populate component manager array
        //    for (int i = 0; i < componentManagers.Count; i++)
        //    {
        //        // Get component type ID
        //        int componentTypeID = reader.ReadInt32();

        //        // Find the component manager type
        //        string typeName = reader.ReadString();

        //        Type componentType = Type.GetType(typeName);
        //        Type componentManagerType = typeof(ComponentManager<>).MakeGenericType(componentType);

        //        // Deserialize the component data
        //        IFormatter formatter = new BinaryFormatter();
        //        ISparseSet components = (ISparseSet)formatter.Deserialize(reader.BaseStream);

        //        // Create a component manager
        //        componentManagers[componentTypeID] =
        //            (BaseComponentManager)Activator.CreateInstance(
        //                componentManagerType,
        //                components,
        //                componentTypeID);
        //    }

        //    return componentManagers;
        //}

        /// <summary>
        /// Deserializes a singleton entity from a world reader.
        /// Must be called after DeserializeToComponentManagers(reader)
        /// </summary>
        /// <param name="reader">Reader to read the world data from</param>
        /// <returns></returns>
        //private Entity DeserializeSingletonEntity(BinaryReader reader)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    return (Entity)formatter.Deserialize(reader.BaseStream);
        //}

        #endregion Serialization

        #region Phases

        internal virtual void Init()
        {
            // Register all systems' components
            WorldHandle.RegisterAllComponents(this);

            for (int i = 0; i < _entities.Length; i++)
            {
                WorldHandle.InitSystems.Invoke(this, _entities[i], WorldHandle.EntityFilter);
            }
        }

        internal virtual void Update()
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                WorldHandle.UpdateSystems.Invoke(this, _entities[i], WorldHandle.EntityFilter);
            }
        }

        internal virtual void UpdateFixed()
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                WorldHandle.UpdateFixedSystems.Invoke(this, _entities[i], WorldHandle.EntityFilter);
            }
        }

        internal virtual void Draw()
        {
            // TODO: Disable draw if the world is just a server
            // Important to make sure no state is being mutated in Draw
            // so the game logic doesn't get affected

            for (int i = 0; i < _entities.Length; i++)
            {
                WorldHandle.DrawSystems.Invoke(this, _entities[i], WorldHandle.EntityFilter);
            }
        }

#endregion
    }
}
