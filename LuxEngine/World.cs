using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class World
    {
        private EntityManager _entityManager;
        private SortedDictionary<Entity, ComponentMask> _entityMasks;
        private BaseSystem[] _systems;
        private BaseComponentManager[] _componentManagers { get; set; }

        public World()
        {
            _entityManager = new EntityManager();
            _entityMasks = new SortedDictionary<Entity, ComponentMask>();
            _systems = new BaseSystem[(int)SystemId.SystemsCount];
            _componentManagers = new BaseComponentManager[(int)ComponentType.ComponentTypeCount];
        }

        public EntityHandle CreateEntity()
        {
            EntityHandle entityHandle = new EntityHandle(_entityManager.CreateEntity(), this);
            _entityMasks.Add(entityHandle.Entity, new ComponentMask(new int[0]));

            return entityHandle;
        }

        public bool TryUnpack<T>(Entity entity, out T componentOut)
        {
            ComponentManager<T> foundComponentManager = _getComponentManager<T>();
            if (null == foundComponentManager)
            {
                throw new LuxException(LuxStatus.WORLD_TRYUNPACK_COMPONENT_MANAGER_NOT_FOUND, (int)BaseComponent<T>.ComponentType);
            }

            BaseComponent<T> component;
            if (!foundComponentManager.TryGetComponent(entity, out component))
            {
                componentOut = default;
                return false;
            }

            // Return the component without the ugly BaseComponent<T> wrapper
            componentOut = (T)Convert.ChangeType(component, typeof(T));
            return true;
        }

        public T Unpack<T>(Entity entity)
        {
            T component;
            if (!TryUnpack<T>(entity, out component))
            {
                throw new LuxException(LuxStatus.WORLD_UNPACK_COMPONENT_NOT_FOUND_FOR_ENTITY, entity.Id);
            }

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

        public void RegisterSystem<T>(SystemId systemId) where T: IdentifiableSystem<T>, new()
        {
            // Set the ID for the appropriate system class
            IdentifiableSystem<T>.SystemId = systemId;

            // Create the system
            T newSystem = new T();
            newSystem.World = this;

            _systems[(int)systemId] = newSystem;
        }

        /// <summary>
        /// Applies a ComponentType to a Component class and instantiates
        /// a component manager for the component type.
        /// </summary>
        /// <typeparam name="T">Component class</typeparam>
        /// <param name="componentType">Component type matching the class</param>
        public void RegisterComponentType<T>(ComponentType componentType)
        {
            // Set the type for the appropriate component class
            BaseComponent<T>.ComponentType = componentType;

            // Create a component manager for the component type
            _componentManagers[(int)componentType] = new ComponentManager<T>();
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

        public virtual void Init()
        {
            foreach (BaseSystem system in _systems)
            {
                system.Init();
            }
        }

        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManage)
        {
            foreach (BaseSystem system in _systems)
            {
                system.LoadContent(graphicsDevice, contentManage);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (BaseSystem system in _systems)
            {
                system.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (BaseSystem system in _systems)
            {
                system.Draw(gameTime);
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
