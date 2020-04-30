using System.Collections.Generic;

// todo : test [MethodImpl(MethodImplOptions.AggressiveInlining)]

namespace Lux.Framework.ECS
{
    /// <summary>
    /// An ECS world that stores component data and manages entities.
    /// </summary>
    public class World
    {
        /// <summary>
        /// An array of all entities in the world
        /// </summary>
        private readonly Entity[] _entities;

        /// <summary>
        /// The handle correlated with this world.
        /// </summary>
        private readonly WorldHandle _worldHandle;

        /// <summary>
        /// Manages entities' IDs
        /// </summary>
        private readonly EntityGenerator _entityGenerator;

        /// <summary>
        /// Component managers that contains all component data
        /// </summary>
        private readonly Dictionary<int, IComponentManager> _componentManagers;

        /// <summary>
        /// An entity that is globally accessible from any system.
        /// To access it a system should signature.RequireSingleton<T>
        /// </summary>
        private readonly Entity _singletonEntity;

        /// <summary>
        /// Filters entities based on if they have all the components the system
        /// requires. This filter is used in all the interval-ed calls: Update, Draw, etc.
        /// </summary>
        private readonly IEntityFilter _defaultEntityFilter;


        class Transform : AComponent<Transform>
        { }

        public World(WorldHandle worldHandle)
        {
            _entities = new Entity[HardCodedConfig.MAX_ENTITIES_PER_WORLD];

            _worldHandle = worldHandle;
            _entityGenerator = new EntityGenerator();
            _componentManagers = new Dictionary<int, IComponentManager>();
            _singletonEntity = CreateSingletonEntity();
            _defaultEntityFilter = new DefaultEntityFilter();
        }

        /// <summary>
        /// Creates the singleton entity that owns all singleton components
        /// </summary>
        /// <returns></returns>
        private Entity CreateSingletonEntity()
        {
            Entity entity = CreateEntity();

            AddComponent(entity, new Context(this));

            return entity;
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
        /// Destroys an existing entity from the world
        /// </summary>
        /// <param name="entity">Entity to destroy</param>
        public void DestroyEntity(Entity entity)
        {
            // todo: Call OnDestroyEntity systems

            // Remove entity's components
            foreach (IComponentManager componentManager in _componentManagers.Values)
            {
                RemoveComponent(entity, componentManager);
            }

            // Destroy entity
            _entityGenerator.DestroyEntity(entity);
        }

        public bool Unpack<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            // Get component manager for the relevant component type
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();

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

        public bool UnpackEntityOrSingleton<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            if (typeof(ISingleton).IsAssignableFrom(typeof(T)))
            {
                return UnpackSingleton(out outComponent);
            }

            return Unpack(entity, out outComponent);
        }

        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="component">Component to return</param>
        public void AddComponent<T>(Entity entity, T component) where T : AComponent<T>
        {
            // Set the entity for the component
            component.Entity = entity;

            // If trying to add a singleton component to an entity that isn't the singleton entity
            if (component is ISingleton && entity != _singletonEntity)
            {
                LuxCommon.Assert(false);
                return;
            }

            // Get component manager for the relevant component type
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();

            // If component already exists
            if (foundComponentManager.GetComponent(entity, out _))
            {
                LuxCommon.Assert(false); // Shouldn't happen, use RemoveComponent first
                RemoveComponent<T>(entity);
            }

            // Add component to system
            foundComponentManager.AddComponent(entity, component);

            // Call systems that subscribed to the OnAddComponent event
            // TODO: Maybe make EntityFilters a struct to improve performance when calling new this often
            Run(_worldHandle.OnAddComponentSystems, new OnAddComponent(typeof(T)));
        }

        /// <summary>
        /// Adds a component to the singleton entity.
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="component">The component to add</param>
        public void AddSingletonComponent<T>(T component) where T : AComponent<T>, ISingleton
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
            RemoveComponent(entity, foundComponentManager);
        }

        private void RemoveComponent(Entity entity, IComponentManager componentManager)
        {
            componentManager.RemoveComponent(entity);

            // Call systems that subscribed to the OnRemoveComponent event
            // TODO: Handle remove component




            //Run(_worldHandle.OnAddComponentSystems);
        }

        /// <summary>
        /// Registers a component type to a world. Does nothing if already registered.
        /// </summary>
        /// <typeparam name="T">Component type to register to the world</typeparam>
        /// <param name="system">The system that is registering this component</param>
        /// <returns>The component type's ID for the world</returns>
        public void Register<T>(ASystem system) where T : AComponent<T>
        {
            // Set the ComponentType for the component class
            AComponent<T>.SetComponentType();

            // If component is NOT a singleton
            if (system != null && !typeof(ISingleton).IsAssignableFrom(typeof(T)))
            {
                // System can't be a singleton system
                system.IsSingletonSystem = false;
            }

            // Add a component manager for the type if doesn't exist
            if (!_componentManagers.ContainsKey(AComponent<T>.ComponentType))
            {
                int maxComponents = HardCodedConfig.MAX_ENTITIES_PER_WORLD;
                if (typeof(ISingleton).IsAssignableFrom(typeof(T)))
                {
                    maxComponents = 1;
                }

                var componentManager = new ComponentManager<T>(maxComponents);
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
                Register<T>(null);
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

        public void Run(Systems systems, IEntityFilter filter)
        {
            // For every system
            for (int i = 0; i < systems.Count; i++)
            {
                // If the system is a singleton system, only run it once for the singleton entity
                if (systems[i].IsSingletonSystem)
                {
                    systems[i].Invoke(this, _singletonEntity, filter);
                    continue;
                }

                // Run the system for every entity
                for (int s = 0; s < _entities.Length; s++)
                {
                    systems[i].Invoke(this, _entities[s], filter);
                }
            }
        }

        public void Run(Systems systems)
        {
            Run(systems, _defaultEntityFilter);
        }
    }
}
