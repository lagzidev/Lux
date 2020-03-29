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

        private EntityManager _entityManager;
        private SortedDictionary<Entity, ComponentMask> _entityMasks;
        private LuxIterator<InternalBaseSystem> _systems;
        private BaseComponentManager[] _componentManagers;

        private List<InternalBaseSystem> _tempSystemList;
        private List<BaseComponentManager> _tempComponentManagerList;

        /// <summary>
        /// A list of actions to execute after done iterating over systems;
        /// These are actions that cannot be executed while iterating systems.
        /// </summary>
        private Queue<Action> _postponedSystemActions;

        private GraphicsDeviceManager _graphicsDeviceManager;
        private ContentManager _contentManager;

        public World(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            _paused = false;

            // TODO: Think about a way to move this out of the constructor (InitWorld is no good because
            // it's called before AddSingletonComponent which can be called whenever. Thus the components aren't registered
            // properly because the component managers are already initialized in InitWorld.
            SingletonEntity = CreateEntity().Entity;

            _entityManager = new EntityManager();
            _entityMasks = new SortedDictionary<Entity, ComponentMask>();
            _systems = null;
            _componentManagers = null;

            _tempSystemList = new List<InternalBaseSystem>();
            _tempComponentManagerList = new List<BaseComponentManager>();

            _postponedSystemActions = new Queue<Action>();

            _graphicsDeviceManager = graphicsDeviceManager;
            _contentManager = contentManager;
        }

        /// <summary>
        /// Should be called after all systems and component types are registered,
        /// and before any entities are created.
        /// </summary>
        public void InitWorld()
        {
            _systems = new LuxIterator<InternalBaseSystem>(_tempSystemList.ToArray(), _postponedSystemActions);
            _componentManagers = _tempComponentManagerList.ToArray();
        }

        /// <summary>
        /// Deserializes a world from a reader and initializes it
        /// Should be called after all systems and component types are registered,
        /// and before any entities are created.
        /// </summary>
        public void InitWorld(BinaryReader reader)
        {
            _systems = new LuxIterator<InternalBaseSystem>(_tempSystemList.ToArray(), _postponedSystemActions);
            _componentManagers = DeserializeToComponentManagers(reader);
            SingletonEntity = DeserializeSingletonEntity(reader);

            foreach (var system in _systems)
            {
                system.Init(_graphicsDeviceManager);
            }

            foreach (var system in _systems)
            {
                system.LoadContent(_graphicsDeviceManager.GraphicsDevice, _contentManager);
            }
        }

        public EntityHandle CreateEntity()
        {
            EntityHandle entityHandle = new EntityHandle(_entityManager.CreateEntity(), this);
            _entityMasks.Add(entityHandle.Entity, new ComponentMask(new int[0]));

            return entityHandle;
        }

        public void DestroyEntity(Entity entity)
        {
            // Remove entity from all systems
            foreach (var system in _systems)
            {
                // Notify systems
                system.PreDestroyEntity(entity);

                // Unregister if entity exists in the system
                system.UnregisterEntity(entity);
            }

            // Remove entity's components
            foreach (var componentManager in _componentManagers)
            {
                componentManager.RemoveComponent(entity);
            }

            // Remove entity
            _entityMasks.Remove(entity);
            _entityManager.DestroyEntity(entity);
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

        public void AddSingletonComponent<T>(BaseComponent<T> component)
        {
            // TODO: Register component type

            AddComponent(SingletonEntity, component);
        }

        public void AddComponent<T>(Entity entity, BaseComponent<T> component)
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
            var oldMask = (ComponentMask)_entityMasks[entity].Clone();
            _entityMasks[entity].AddComponent<T>();

            UpdateEntitySystems(entity, oldMask);
        }

        public void RemoveComponent<T>(Entity entity)
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
                oldMask = (ComponentMask)oldMask.Clone();
                _entityMasks[entity].RemoveComponent<T>();
                UpdateEntitySystems(entity, oldMask);
            }
            else
            {
                // Entity doesn't have a mask
                LuxCommon.Assert(false);
            }
        }

        public void RegisterSystem<T>() where T : BaseSystem<T>, new()
        {
            T system = new T { World = this };

            // Register required component types if they're not already registered
            foreach (var componentType in system.GetRequiredComponents())
            {
                // We use reflection so that the user won't have to register
                // component types manually.

                Type genericComponentType = typeof(BaseComponent<>).MakeGenericType(componentType);

                // TODO: Find a way to get rid of these ugly hardcoded strings
                if (-1 == (int)genericComponentType.GetProperty("ComponentType").GetValue(null))
                {
                    // Get the method for registering component types
                    MethodInfo method = typeof(World).GetMethod(
                        "RegisterComponentType",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    MethodInfo registerComponentType = method.MakeGenericMethod(componentType);

                    // Register this componentType
                    registerComponentType.Invoke(this, null);
                }
            }

            _tempSystemList.Add(system);
        }

        /// <summary>
        /// Applies a ComponentType to a Component class and instantiates
        /// a component manager for the component type.
        /// </summary>
        /// <typeparam name="T">Component class</typeparam>
        private void RegisterComponentType<T>() where T : BaseComponent<T>
        {
            // Set the ComponentType for the component's class
            BaseComponent<T>.ComponentType = _tempComponentManagerList.Count;
            _tempComponentManagerList.Add(new ComponentManager<T>());
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
            writer.Write(_componentManagers.Length);

            foreach (var componentManager in _componentManagers)
            {
                componentManager.Serialize(writer);
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
        private BaseComponentManager[] DeserializeToComponentManagers(BinaryReader reader)
        {
            // Get amount of component managers
            int componentManagersCount = reader.ReadInt32();
            BaseComponentManager[] componentManagers = new BaseComponentManager[componentManagersCount];

            // Populate component manager array
            for (int i = 0; i < componentManagers.Length; i++)
            {
                componentManagers[i] = BaseComponentManager.Deserialize(reader);
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

        /// <summary>
        /// Add or remove the entity for each system it belongs to
        /// </summary>
        private void UpdateEntitySystems(Entity entity, ComponentMask oldMask)
        {
            foreach (var system in _systems)
            {
                bool entityMatchesSystem = _entityMasks[entity].Matches(system.ComponentMask);
                bool entityWasInSystem = oldMask.Matches(system.ComponentMask);
                if (entityMatchesSystem && !entityWasInSystem)
                {
                    system.RegisterEntity(entity, _contentManager);
                }
                else if (!entityMatchesSystem && entityWasInSystem)
                {
                    system.UnregisterEntity(entity);
                }
            }
        }

        // System calling functions
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

        private ComponentManager<T> _getComponentManager<T>()
        {
            return (ComponentManager<T>)_componentManagers[BaseComponent<T>.ComponentType];
        }
    }
}
