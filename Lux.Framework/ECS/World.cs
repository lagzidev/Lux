using System;
using System.Collections.Generic;

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
        /// Actions an entity invokes when destroyed
        /// </summary>
        private readonly Dictionary<int, List<Action>> _entitiesOnDestroy;

        /// <summary>
        /// The handle correlated with this world.
        /// </summary>
        private readonly WorldHandle _worldHandle;

        /// <summary>
        /// Manages entities' IDs
        /// </summary>
        private readonly EntityGenerator _entityGenerator;

        /// <summary>
        /// An entity that is globally accessible from any system.
        /// To access it a system should signature.RequireSingleton<T>
        /// </summary>
        private readonly Entity _singletonEntity;


        public World(WorldHandle worldHandle)
        {
            _entities = new Entity[HardCodedConfig.MAX_ENTITIES_PER_WORLD];
            _entityMasks = new ComponentMask[HardCodedConfig.MAX_ENTITIES_PER_WORLD];
            _entitiesOnDestroy = new Dictionary<int, List<Action>>(HardCodedConfig.MAX_ENTITIES_PER_WORLD);
            _worldHandle = worldHandle;
            _entityGenerator = new EntityGenerator();
            _singletonEntity = CreateSingletonEntity();
        }

        /// <summary>
        /// Creates the singleton entity that owns all singleton components
        /// </summary>
        /// <returns></returns>
        private Entity CreateSingletonEntity()
        {
            return CreateEntity();
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
            _entitiesOnDestroy.Add(entity.Index, new List<Action>());

            // Add default components
            AddComponent(entity, new Context(this, entity));

            return entity;
        }

        /// <summary>
        /// Destroys an existing entity from the world
        /// </summary>
        /// <param name="entity">Entity to destroy</param>
        public void DestroyEntity(Entity entity)
        {
            Run(_worldHandle.OnDestroyEntitySystems, entity, new OnDestroyEntity());

            // Execute cleanup actions (removing components, etc.)
            for (int i = 0; i < _entitiesOnDestroy[entity.Index].Count; i++)
            {
                _entitiesOnDestroy[entity.Index][i].Invoke();
            }

            _entitiesOnDestroy.Remove(entity.Index);
            _entityMasks[entity.Index].Reset();

            _entityGenerator.DestroyEntity(entity);
        }

        public T Unpack<T>(Entity entity) where T : IComponent
        {
            ComponentsData<T>.Get(entity, out T outComponent);
            return outComponent;
        }

        public bool Unpack<T>(Entity entity, out T outComponent) where T : IComponent
        {
            return ComponentsData<T>.Get(entity, out outComponent);
        }

        /// <summary>
        /// Get a component that belongs to the global singleton entity.
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="outComponent">Returned component</param>
        /// <returns><c>true</c> if the component exists, <c>false</c> otherwise</returns>
        public bool UnpackSingleton<T>(out T outComponent) where T : IComponent
        {
            return Unpack(_singletonEntity, out outComponent);
        }

        /// <summary>
        /// Get all components of a certain type
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>A view of all components</returns>
        public Span<T> GetAll<T>() where T : IComponent
        {
            return ComponentsData<T>.GetAll();
        }

        /// <summary>
        /// Get all components of a certain type, readonly
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>A view of all components</returns>
        public ReadOnlySpan<T> GetAllReadonly<T>() where T : IComponent
        {
            return ComponentsData<T>.GetAllReadonly();
        }

        public ReadOnlySpan<T> GetAllReadonly<T>(out ReadOnlySpan<Entity> entities) where T : IComponent
        {
            entities = ComponentsData<T>.Entities;
            return ComponentsData<T>.GetAllReadonly();
        }

        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="component">Component to return</param>
        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            // If trying to add a singleton component to an entity that isn't the singleton entity
            if (entity != _singletonEntity && component is ISingleton)
            {
                LuxCommon.Assert(false);
                return;
            }

            // If component already exists
            if (ComponentsData<T>.Get(entity, out _))
            {
                LuxCommon.Assert(false); // Shouldn't happen, use RemoveComponent first
                RemoveComponent<T>(entity);
            }

            // Add component to system
            ComponentsData<T>.Add(entity, component);

            // Update mask and systems
            _entityMasks[entity.Index].AddComponent<T>();
            _worldHandle.AddComponent<T>(entity, _entityMasks[entity.Index]);

            // Add destroy action for the component
            // TODO: Find a better solution. This gets invoked even if the component was already destroyed with RemoveComponent
            _entitiesOnDestroy[entity.Index].Add(() => RemoveComponent<T>(entity));

            // Call systems that subscribed to the OnAddComponent event
            // TODO: new OnAddComponent is called too often, try finding a solution
            Run(_worldHandle.OnAddComponentSystems, entity, new OnAddComponent(typeof(T)));
        }

        /// <summary>
        /// Adds a component to the singleton entity.
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="component">The component to add</param>
        public void AddSingletonComponent<T>(T component) where T : IComponent
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
        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            ComponentsData<T>.Remove(entity);

            _entityMasks[entity.Index].RemoveComponent<T>();
            _worldHandle.RemoveComponent<T>(entity, _entityMasks[entity.Index]);
        }

        public List<Entity> GetEntities<T1, T2>()
            where T1 : IComponent
            where T2 : IComponent
        {
            ReadOnlySpan<Entity> t1Entities = ComponentsData<T1>.Entities;
            ReadOnlySpan<Entity> t2Entities = ComponentsData<T2>.Entities;

            int minSize = LuxCommon.Min(
                t1Entities.Length,
                t2Entities.Length
            );

            List<Entity> entities = new List<Entity>(minSize);

            if (ComponentsData<T1>.Count == minSize)
            {
                for (int i = 0; i < minSize; i++)
                {
                    if (0 != t2Entities.BinarySearch(t1Entities[i]))
                    {
                        continue;
                    }

                    entities.Add(t1Entities[i]);
                }
            }
            else // T2
            {
                for (int i = 0; i < minSize; i++)
                {
                    if (0 != t1Entities.BinarySearch(t2Entities[i]))
                    {
                        continue;
                    }

                    entities.Add(t2Entities[i]);
                }
            }

            return entities;
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

        public void Run(Systems systems, Entity? entity, ISystemFilter filter)
        {
            // For every system
            for (int i = 0; i < systems.Count; i++)
            {
                // Should run the system
                if (filter != null && !filter.Filter(systems[i]))
                {
                    continue;
                }

                systems[i].Invoke(this, entity);
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
