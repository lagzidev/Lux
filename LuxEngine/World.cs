using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class World
    {
        private EntityManager _entityManager;
        private SortedDictionary<Entity, ComponentMask> _entityMasks;
        private BaseSystem[] _systems;
        public BaseSystem[] Systems
        {
            private get
            {
                return _systems;
            }
            set
            {
                foreach (BaseSystem system in value)
                {
                    system.World = this;
                }

                _systems = value;
            }
        }

        public BaseComponentManager[] ComponentManagers { get; set; }

        public World()
        {
            _entityManager = new EntityManager();
            _entityMasks = new SortedDictionary<Entity, ComponentMask>();
            Systems = new BaseSystem[0];
            ComponentManagers = new BaseComponentManager[0];
        }

        public EntityHandle CreateEntity()
        {
            EntityHandle entityHandle = new EntityHandle(_entityManager.CreateEntity(), this);
            _entityMasks.Add(entityHandle.Entity, new ComponentMask(new int[0]));

            return entityHandle;
        }

        public ComponentManager<T> GetComponents<T>()
        {
            // TODO: Optimization - give each component/component manager an 
            // index that can be used to instantly access the correct 
            // componentmanager by index.
            return _getComponentManager<T>();
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

        /// <summary>
        /// Add or remove the entity for each system it belongs to
        /// </summary>
        private void updateEntitySystems(Entity entity, ComponentMask oldMask)
        {
            foreach (var system in Systems)
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
            foreach (BaseSystem system in Systems)
            {
                system.Init();
            }
        }

        public virtual void LoadContent()
        {
            foreach (BaseSystem system in Systems)
            {
                system.LoadContent();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (BaseSystem system in Systems)
            {
                system.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (BaseSystem system in Systems)
            {
                system.Draw(gameTime);
            }
        }

        private ComponentManager<T> _getComponentManager<T>()
        {
            ComponentManager<T> foundComponentManager = null;
            foreach (var componentManager in ComponentManagers)
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
