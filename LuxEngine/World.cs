using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class World
    {
        bool Paused;

        private EntityManager _entityManager;
        private SortedDictionary<Entity, ComponentMask> _entityMasks;
        private InternalBaseSystem[] _systems;
        private BaseComponentManager[] _componentManagers;
        private List<Type> _systemTypes;
        private List<Type> _componentTypes;

        public EntityHandle SingletonEntity { get; private set; }

        public World()
        {
            Paused = false;

            _entityManager = new EntityManager();
            _entityMasks = new SortedDictionary<Entity, ComponentMask>();
            _systems = null;
            _componentManagers = null;
            _systemTypes = new List<Type>();
            _componentTypes = new List<Type>();

            SingletonEntity = CreateEntity();
        }

        /// <summary>
        /// Should be called after all systems and component types are registered,
        /// and before any entities are created.
        /// </summary>
        public void InitWorld()
        {
            // TODO: Find a better way to number systems/managers while creating a fixed-size array
            // We use reflection (Activator) which is slow because this happens rarely.
            _systems = new InternalBaseSystem[_systemTypes.Count];
            for (int i = 0; i < _systems.Length; i++)
            {
                _systems[i] = (InternalBaseSystem)Activator.CreateInstance(_systemTypes[i]);
                _systems[i].World = this;
            }

            _componentManagers = new BaseComponentManager[_componentTypes.Count];
            for (int i = 0; i < _componentManagers.Length; i++)
            {
                Type type = typeof(ComponentManager<>).MakeGenericType(_componentTypes[i]);
                _componentManagers[i] = (BaseComponentManager)Activator.CreateInstance(type);
            }
        }

        public EntityHandle CreateEntity()
        {
            EntityHandle entityHandle = new EntityHandle(_entityManager.CreateEntity(), this);
            _entityMasks.Add(entityHandle.Entity, new ComponentMask(new int[0]));

            return entityHandle;
        }

        public bool TryUnpack<T>(Entity entity, out T outComponent)
        {
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                // This is worrying on a development level, assert
                Debug.Assert(false);
                outComponent = default;
                return false;
            }

            BaseComponent<T> component;
            if (!foundComponentManager.TryGetComponent(entity, out component))
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
            Debug.Assert(unpackSuccess);

            return component;
        }

        public void AddComponent<T>(Entity entity, BaseComponent<T> component)
        {
            // Update the component manager
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            foundComponentManager.AddComponent(entity, component);

            // Update the entity's component mask
            var oldMask = (ComponentMask)_entityMasks[entity].Clone();
            _entityMasks[entity].AddComponent<T>();

            updateEntitySystems(entity, oldMask);
        }

        public void RemoveComponent<T>(Entity entity)
        {
            // Update the component manager
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            foundComponentManager.RemoveComponent(entity);

            // Update the entity's component mask
            var oldMask = (ComponentMask)_entityMasks[entity].Clone();
            _entityMasks[entity].RemoveComponent<T>();

            updateEntitySystems(entity, oldMask);
        }

        public void RegisterSystem<T>() where T : BaseSystem<T>, new()
        {
            // Set the ID for the appropriate system class
            BaseSystem<T>.SystemId = _systemTypes.Count;
            _systemTypes.Add(typeof(T));
        }

        /// <summary>
        /// Applies a ComponentType to a Component class and instantiates
        /// a component manager for the component type.
        /// </summary>
        /// <typeparam name="T">Component class</typeparam>
        public void RegisterComponentType<T>()
        {
            // Set the type for the appropriate component class
            BaseComponent<T>.ComponentType = _componentTypes.Count;

            // Add the type to the componentTypes for later initialization of the
            // componentManager array
            _componentTypes.Add(typeof(T));
        }

        /// <summary>
        /// Serializes all of the component managers and writes them into a
        /// TextWriter instance
        /// </summary>
        /// <param name="writer">Writer to write the serialized data into</param>
        public void Serialize(TextWriter writer)
        {
            var toSerialize = _componentManagers;

            XmlSerializer serializer = new XmlSerializer(toSerialize.GetType());
            serializer.Serialize(writer, toSerialize);
        }

        /// <summary>
        /// Deserializes a world from a reader and loads it
        /// </summary>
        /// <param name="reader">Reader to get the world data from</param>
        public void Deserialize(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(_componentManagers.GetType());
            _componentManagers = (BaseComponentManager[])serializer.Deserialize(reader);
        }

        /// <summary>
        /// Add or remove the entity for each system it belongs to
        /// </summary>
        private void updateEntitySystems(Entity entity, ComponentMask oldMask)
        {
            foreach (var system in _systems)
            {
                bool entityMatchesSystem = _entityMasks[entity].Matches(system.ComponentMask);
                bool entityWasInSystem = oldMask.Matches(system.ComponentMask);
                if (entityMatchesSystem && !entityWasInSystem)
                {
                    system.RegisterEntity(entity);
                }
                else if (!entityMatchesSystem && entityWasInSystem)
                {
                    system.UnregisterEntity(entity);
                }
            }
        }

        // System calling functions
        public virtual void Init(GraphicsDeviceManager graphicsDeviceManager)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.Init(graphicsDeviceManager);
            }
        }

        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.LoadContent(graphicsDevice, contentManager);
            }
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
            if (!Paused)
            {
                foreach (InternalBaseSystem system in _systems)
                {
                    system.PreUpdate(gameTime);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Paused)
            {
                foreach (InternalBaseSystem system in _systems)
                {
                    system.Update(gameTime);
                }
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            if (!Paused)
            {
                foreach (InternalBaseSystem system in _systems)
                {
                    system.PostUpdate(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!Paused)
            {
                foreach (InternalBaseSystem system in _systems)
                {
                    system.Draw(gameTime);
                }
            }
        }

        private ComponentManager<T> _getComponentManager<T>()
        {
            // TODO: Optimization - give each component/component manager an 
            // index that can be used to instantly access the correct 
            // componentmanager by index.

            ComponentManager<T> foundComponentManager = null;
            foreach (var componentManager in _componentManagers)
            {
                // Find the component manager of the appropriate component type
                if (componentManager.GetType() == typeof(ComponentManager<T>))
                {
                    foundComponentManager = (ComponentManager<T>)componentManager;
                    break;
                }
            }

            // If component manager not found
            if (null == foundComponentManager)
            {
                // TODO: Log error
                return null;
            }

            return foundComponentManager;
        }
    }
}
