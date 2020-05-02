using System;
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
        /// Component mask for every entity in the world
        /// </summary>
        private readonly ComponentMask[] _entityMasks;

        /// <summary>
        /// The handle correlated with this world.
        /// </summary>
        private readonly WorldHandle _worldHandle;

        /// <summary>
        /// Manages entities' IDs
        /// </summary>
        private readonly EntityGenerator _entityGenerator;

        /// <summary>
        /// All of the components data
        /// </summary>
        private readonly Dictionary<int, IComponentsData> _componentsData;

        /// <summary>
        /// An entity that is globally accessible from any system.
        /// To access it a system should signature.RequireSingleton<T>
        /// </summary>
        private readonly Entity _singletonEntity;


        public World(WorldHandle worldHandle)
        {
            _entities = new Entity[HardCodedConfig.MAX_ENTITIES_PER_WORLD];
            _entityMasks = new ComponentMask[HardCodedConfig.MAX_ENTITIES_PER_WORLD];
            _worldHandle = worldHandle;
            _entityGenerator = new EntityGenerator();
            _componentsData = new Dictionary<int, IComponentsData>();
            _singletonEntity = CreateSingletonEntity();
        }

        /// <summary>
        /// Creates the singleton entity that owns all singleton components
        /// </summary>
        /// <returns></returns>
        private Entity CreateSingletonEntity()
        {
            Entity entity = CreateEntity();

            RegisterSingleton<Context>();
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
            _entityMasks[entity.Index] = new ComponentMask(HardCodedConfig.MAX_GAME_COMPONENT_TYPES);

            // Add default components
            Register<EntityInfo>(); // TODO: Try removing registration (because of auto registration on get component) See if there are side effects
            AddComponent(entity, new EntityInfo());

            return entity;
        }

        /// <summary>
        /// Destroys an existing entity from the world
        /// </summary>
        /// <param name="entity">Entity to destroy</param>
        public void DestroyEntity(Entity entity)
        {
            Run(_worldHandle.OnDestroyEntitySystems, entity, new OnDestroyEntity());

            // Remove entity's components
            foreach (IComponentsData componentsData in _componentsData.Values)
            {
                RemoveComponent(entity, componentsData);
            }

            _entityMasks[entity.Index].Reset();

            // Destroy entity
            _entityGenerator.DestroyEntity(entity);
        }

        public bool Unpack<T>(Entity entity, out T outComponent) where T : AComponent<T>
        {
            // Get components data for the relevant component type
            ComponentsData<T> foundcomponentsData = GetComponentsData<T>();

            // Get component if it exists
            if (!foundcomponentsData.GetComponent(entity, out outComponent))
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
        /// Get all components of a certain type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>A view of all components</returns>
        public ReadOnlySpan<T> GetAll<T>() where T : AComponent<T>
        {
            ComponentsData<T> componentsData = GetComponentsData<T>();
            return componentsData.GetAllReadonly();
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
            if (entity != _singletonEntity && component is ISingleton)
            {
                LuxCommon.Assert(false);
                return;
            }

            // Get components data for the relevant component type
            ComponentsData<T> foundcomponentsData = GetComponentsData<T>();

            // If component already exists
            if (foundcomponentsData.GetComponent(entity, out _))
            {
                LuxCommon.Assert(false); // Shouldn't happen, use RemoveComponent first
                RemoveComponent<T>(entity);
            }

            // Add component to system
            foundcomponentsData.AddComponent(entity, component);

            _entityMasks[entity.Index].AddComponent<T>();

            // Call systems that subscribed to the OnAddComponent event
            // TODO: Maybe make OnAddComponent a struct to improve performance when calling new this often
            Run(_worldHandle.OnAddComponentSystems, entity, new OnAddComponent(typeof(T)));
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
            ComponentsData<T> foundcomponentsData = GetComponentsData<T>();
            _entityMasks[entity.Index].RemoveComponent<T>();
            RemoveComponent(entity, foundcomponentsData);
        }

        private void RemoveComponent(Entity entity, IComponentsData componentsData)
        {
            componentsData.RemoveComponent(entity);

            // Call systems that subscribed to the OnRemoveComponent event
            //Run(_worldHandle.OnRemoveComponentSystems);
        }

        /// <summary>
        /// Registers a component type to a world. Does nothing if already registered.
        /// </summary>
        /// <typeparam name="T">Component type to register to the world</typeparam>
        /// <param name="system">The system that is registering this component</param>
        /// <returns>The component type's ID for the world</returns>
        public void Register<T>(int maxComponentsPerType = HardCodedConfig.MAX_ENTITIES_PER_WORLD) where T : AComponent<T>
        {
            // Set the ComponentType for the component class
            AComponent<T>.SetComponentType();

            // Add a components data for the type if doesn't exist
            if (!_componentsData.ContainsKey(AComponent<T>.ComponentType))
            {
                _componentsData[AComponent<T>.ComponentType] = new ComponentsData<T>(maxComponentsPerType);
            }
        }

        public void RegisterSingleton<T>() where T : AComponent<T>
        {
            Register<T>(1);
        }

        ///// <summary>
        ///// Tells the world to keep track of the components' previous state.
        ///// </summary>
        ///// <typeparam name="T">The type of component of which state we keep</typeparam>
        //public void KeepPreviousState<T>() where T : AComponent<T>
        //{
        //    // Set the ComponentType in case it wasn't set already
        //    AComponent<T>.SetComponentType();

            //    // Add a components data for the type if doesn't exist
            //    if (!_previouscomponentsDatas.ContainsKey(AComponent<T>.ComponentType))
            //    {
            //        var componentsData = new componentsData<T>();
            //        _previouscomponentsDatas[AComponent<T>.ComponentType] = componentsData;
            //    }
            //}

            /// <summary>
            /// Saves the current state of the components into a seperate dataset.
            /// Only copies components for which KeepPreviousState was called.
            /// </summary>
            //private void SavePreviousState<T>(T component) where T : AComponent<T>
            //{
            //    // TODO: Save the component into _previouscomponentsData or alternatively,
            //    // find a more lightweight data storage solution. Something dynamic like
            //    // a Dictionary<Entity, Dictionary<ComponentType, Component> might be lighter
            //    // then a componentsData, but might be more demanding performance wise.
            //}

        private ComponentsData<T> GetComponentsData<T>() where T : AComponent<T>
        {
            // Try getting the components data
            if (!_componentsData.TryGetValue(AComponent<T>.ComponentType, out IComponentsData componentsData))
            {
                Register<T>();
                componentsData = _componentsData[AComponent<T>.ComponentType];
            }

            return (ComponentsData<T>)componentsData;
        }

        #region Serialization

        /// <summary>
        /// Serializes all of the components datas and writes them into a
        /// TextWriter instance
        /// <para>All components (and their members' types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a member from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <param name="writer">Writer to write the serialized data into</param>
        //public void Serialize(BinaryWriter writer)
        //{
        //    // Serialize components datas
        //    writer.Write(_componentsDatas.Count);

        //    foreach (var componentsData in _componentsDatas)
        //    {
        //        writer.Write(componentsData.Key);
        //        componentsData.Value.Serialize(writer);
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
        /// <returns>An array of components datas with entity data</returns>
        //private Dictionary<int, BasecomponentsData> DeserializeTocomponentsDatas(BinaryReader reader)
        //{
        //    // Get amount of components datas
        //    int componentsDatasCount = reader.ReadInt32();
        //    var componentsDatas = new Dictionary<int, BasecomponentsData>(componentsDatasCount);

        //    // Populate components data array
        //    for (int i = 0; i < componentsDatas.Count; i++)
        //    {
        //        // Get component type ID
        //        int componentTypeID = reader.ReadInt32();

        //        // Find the components data type
        //        string typeName = reader.ReadString();

        //        Type componentType = Type.GetType(typeName);
        //        Type componentsDataType = typeof(componentsData<>).MakeGenericType(componentType);

        //        // Deserialize the component data
        //        IFormatter formatter = new BinaryFormatter();
        //        ISparseSet components = (ISparseSet)formatter.Deserialize(reader.BaseStream);

        //        // Create a components data
        //        componentsDatas[componentTypeID] =
        //            (BasecomponentsData)Activator.CreateInstance(
        //                componentsDataType,
        //                components,
        //                componentTypeID);
        //    }

        //    return componentsDatas;
        //}

        /// <summary>
        /// Deserializes a singleton entity from a world reader.
        /// Must be called after DeserializeTocomponentsDatas(reader)
        /// </summary>
        /// <param name="reader">Reader to read the world data from</param>
        /// <returns></returns>
        //private Entity DeserializeSingletonEntity(BinaryReader reader)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    return (Entity)formatter.Deserialize(reader.BaseStream);
        //}

        #endregion Serialization

        public void Run(Systems systems, Entity? entity, IEntityFilter filter)
        {
            // For every system
            for (int i = 0; i < systems.Count; i++)
            {
                // Should run the system
                if (filter != null && !filter.Filter(systems[i]))
                {
                    continue;
                }

                if (entity != null)
                {
                    // TODO
                }

                systems[i].Invoke(this);
            }
        }

        public void Run(Systems systems)
        {
            Run(systems, null, null);
        }

        public ComponentMask GetEntityMask(Entity entity)
        {
            return _entityMasks[entity.Index];
        }

        public ComponentMask GetSingletonEntityMask()
        {
            return GetEntityMask(_singletonEntity);
        }
    }
}
