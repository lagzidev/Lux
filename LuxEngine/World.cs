using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class World
    {
        private bool _paused;
        public bool Paused
        {
            get
            {
                return _paused;
            }
            set
            {
                if (null != _systems)
                {
                    _systems.Paused = value;
                }

                _paused = value;
            }
        }

        public Entity SingletonEntity { get; private set; }

        private EntityGenerator _entityGenerator;

        private Dictionary<Entity, ComponentMask> _entityMasks;
        private LuxIterator<InternalBaseSystem> _systems;
        private Dictionary<int, BaseComponentManager> _componentManagers;

        private List<InternalBaseSystem> _tempSystemList;

        /// <summary>
        /// A list of actions to execute after done iterating over systems;
        /// These are actions that cannot be executed while iterating systems.
        /// </summary>
        private Queue<Action> _postponedSystemActions;

        private GraphicsDeviceManager _graphicsDeviceManager;
        private ContentManager _contentManager;

        private bool _initialized;

        public World(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            _paused = false;

            _entityGenerator = new EntityGenerator();
            _entityMasks = new Dictionary<Entity, ComponentMask>();
            _systems = null;
            _componentManagers = new Dictionary<int, BaseComponentManager>();

            _tempSystemList = new List<InternalBaseSystem>();

            _postponedSystemActions = new Queue<Action>();

            _graphicsDeviceManager = graphicsDeviceManager;
            _contentManager = contentManager;

            _initialized = false;
        }

        public void InitWorld()
        {
            // Only initialize world if not already initialized manually by user
            if (!_initialized)
            {
                _systems = new LuxIterator<InternalBaseSystem>(_tempSystemList.ToArray(), _postponedSystemActions);
                SingletonEntity = CreateEntity().Entity;
            }

            _initialized = true;
        }

        /// <summary>
        /// Deserializes a world from a reader and initializes it
        /// Should be called after all systems are registered and before
        /// any entities that should remain are created.
        /// </summary>
        public void InitWorld(BinaryReader reader)
        {
            _systems = new LuxIterator<InternalBaseSystem>(_tempSystemList.ToArray(), _postponedSystemActions);
            _componentManagers = DeserializeToComponentManagers(reader);
            SingletonEntity = DeserializeSingletonEntity(reader);

            // If the game is already running, call the new world's systems
            // instead of the game calling them
            if (_initialized)
            {
                foreach (var system in _systems)
                {
                    system.LoadContent(_graphicsDeviceManager.GraphicsDevice, _contentManager);
                }

                foreach (var system in _systems)
                {
                    system.Init(_graphicsDeviceManager);
                }
            }

            _initialized = true;
        }

        public EntityHandle CreateEntity()
        {
            EntityHandle entityHandle = new EntityHandle(_entityGenerator.CreateEntity(), this);
            _entityMasks.Add(entityHandle.Entity, new ComponentMask());

            return entityHandle;
        }

        public void DestroyEntity(Entity entity)
        {
            // Remove entity from all systems
            foreach (var system in _systems)
            {
                system.OnDestroyEntity(entity);
            }

            // Remove entity's components
            foreach (var componentManager in _componentManagers)
            {
                componentManager.Value.RemoveComponent(entity);
            }

            // Remove entity
            _entityMasks.Remove(entity);
            _entityGenerator.DestroyEntity(entity);
        }

        public bool TryUnpack<T>(Entity entity, out T outComponent)
        {
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                outComponent = default;
                return false;
            }

            BaseComponent<T> component;
            if (!foundComponentManager.GetComponent(entity, out component))
            {
                outComponent = default;
                return false;
            }

            // Return the component without the ugly BaseComponent<T> wrapper
            outComponent = (T)Convert.ChangeType(component, typeof(T));
            return true;
        }

        public T Unpack<T>(Entity entity)
        {
            T component;
            bool unpackSuccess = TryUnpack(entity, out component);

            // Always expecting success
            LuxCommon.Assert(unpackSuccess);

            return component;
        }

        public void AddSingletonComponent<T>(BaseComponent<T> component) where T : BaseComponent<T>
        {
            AddComponent(SingletonEntity, component);
        }

        public void AddComponent<T>(Entity entity, BaseComponent<T> component) where T : BaseComponent<T>
        {
            // If iterating systems, add the component afterwards instead of now
            if (_systems.IsIterating)
            {
                _postponedSystemActions.Enqueue(() => AddComponent(entity, component));
                return;
            }

            // Set the entity for the component
            component.Entity = entity;

            // Update the component manager
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            foundComponentManager.AddComponent(entity, component);

            // Update the entity's component mask
            _entityMasks[entity].AddComponent<T>();

            foreach (var system in _systems)
            {
                system.UpdateEntity(entity, _entityMasks[entity]);
            }
        }

        /// <summary>
        /// Remove a component from an entity.
        /// </summary>
        /// <typeparam name="T">A component that belongs to an entity</typeparam>
        /// <param name="entity">The entity that owns the component</param>
        /// <remarks>
        /// If called by a system, the removal will take effect only after the
        /// current stage completed.
        /// </remarks>
        /// <example>
        /// If a component is removed inside the system's Update method, it will
        /// only be removed after all other systems called Update for this tick.
        /// PostUpdate will see this component as removed.
        /// </example>
        public void RemoveComponent<T>(Entity entity) where T : BaseComponent<T>
        {
            // If iterating systems, remove the component afterwards instead of now
            if (_systems.IsIterating)
            {
                _postponedSystemActions.Enqueue(() => RemoveComponent<T>(entity));
                return;
            }

            // Update the component manager
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            foundComponentManager.RemoveComponent(entity);

            // Update the entity's component mask
            if (_entityMasks.TryGetValue(entity, out ComponentMask oldMask))
            {
                _entityMasks[entity].RemoveComponent<T>();

                foreach (var system in _systems)
                {
                    system.UpdateEntity(entity, _entityMasks[entity]);
                }
            }
            else
            {
                LuxCommon.Assert(false);
            }
        }

        public void RegisterSystem<T>() where T : BaseSystem<T>, new()
        {
            T system = new T { World = this };
            system.ApplySignature();
            _tempSystemList.Add(system);
        }

        /// <summary>
        /// Registers a component type to a world.
        /// </summary>
        /// <typeparam name="T">Component type to register to the world</typeparam>
        /// <returns>The component type's ID for the world</returns>
        public void RegisterComponent<T>() where T : BaseComponent<T>
        {
            // Set the ComponentType for the component class
            BaseComponent<T>.SetComponentType();

            // Add a component manager for the type if doesn't exist
            if (!_componentManagers.ContainsKey(BaseComponent<T>.ComponentType))
            {
                var componentManager = new ComponentManager<T>();
                _componentManagers[BaseComponent<T>.ComponentType] = componentManager;
            }
        }

        /// <summary>
        /// Serializes all of the component managers and writes them into a
        /// TextWriter instance
        /// <para>All components (and their members' types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a member from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <param name="writer">Writer to write the serialized data into</param>
        public void Serialize(BinaryWriter writer)
        {
            // Serialize component managers
            writer.Write(_componentManagers.Count);

            foreach (var componentManager in _componentManagers)
            {
                writer.Write(componentManager.Key);
                componentManager.Value.Serialize(writer);
            }

            // Serialize singleton entity
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writer.BaseStream, SingletonEntity);
        }

        /// <summary>
        /// Deserializes a world from a reader and loads it
        /// </summary>
        /// <param name="reader">Reader to read the world data from</param>
        /// <returns>An array of component managers with entity data</returns>
        private Dictionary<int, BaseComponentManager> DeserializeToComponentManagers(BinaryReader reader)
        {
            // Get amount of component managers
            int componentManagersCount = reader.ReadInt32();
            var componentManagers = new Dictionary<int, BaseComponentManager>(componentManagersCount);

            // Populate component manager array
            for (int i = 0; i < componentManagers.Count; i++)
            {
                // Get component type ID
                int componentTypeID = reader.ReadInt32();

                // Find the component manager type
                string typeName = reader.ReadString();

                Type componentType = Type.GetType(typeName);
                Type componentManagerType = typeof(ComponentManager<>).MakeGenericType(componentType);

                // Deserialize the component data
                IFormatter formatter = new BinaryFormatter();
                BaseSparseSet components = (BaseSparseSet)formatter.Deserialize(reader.BaseStream);

                // Create a component manager
                componentManagers[componentTypeID] =
                    (BaseComponentManager)Activator.CreateInstance(
                        componentManagerType,
                        components,
                        componentTypeID);
            }

            return componentManagers;
        }

        /// <summary>
        /// Deserializes a singleton entity from a world reader.
        /// Must be called after DeserializeToComponentManagers(reader)
        /// </summary>
        /// <param name="reader">Reader to read the world data from</param>
        /// <returns></returns>
        private Entity DeserializeSingletonEntity(BinaryReader reader)
        {
            IFormatter formatter = new BinaryFormatter();
            return (Entity)formatter.Deserialize(reader.BaseStream);
        }

        private ComponentManager<T> _getComponentManager<T>()
        {
            return (ComponentManager<T>)_componentManagers[BaseComponent<T>.ComponentType];
        }

        #region Phases

        public virtual void Init()
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.Init(_graphicsDeviceManager);
            }
        }

        public virtual void LoadContent()
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.LoadContent(_graphicsDeviceManager.GraphicsDevice, _contentManager);
            }
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.PreUpdate(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.Update(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.PostUpdate(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.Draw(gameTime);
            }
        }

        #endregion
    }
}
