using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class World
    {
        private EntityGenerator _entityGenerator;
        private Dictionary<Entity, ComponentMask> _entityMasks;

        /// <summary>
        /// A list of actions to execute after done iterating over systems;
        /// These are actions that cannot be executed while iterating systems.
        /// </summary>
        private Queue<Action> _postponedSystemActions;

        private LuxIterator<InternalBaseSystem> _systems;
        private Dictionary<int, BaseComponentManager> _componentManagers;

        private bool _initialized;
        private bool _inInitSingleton;

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

        public readonly GraphicsDeviceManager GraphicsDeviceManager;
        public readonly ContentManager ContentManager;

        private Entity _singletonEntity;

        public World(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            _entityGenerator = new EntityGenerator();
            _entityMasks = new Dictionary<Entity, ComponentMask>();

            _postponedSystemActions = new Queue<Action>();

            _systems = new LuxIterator<InternalBaseSystem>(_postponedSystemActions);
            _componentManagers = new Dictionary<int, BaseComponentManager>();

            _initialized = false;
            _paused = false;

            GraphicsDeviceManager = graphicsDeviceManager;
            ContentManager = contentManager;
        }

        public void InitWorld()
        {
            if (_initialized)
            {
                LuxCommon.Assert(false);
                return;
            }

            _singletonEntity = CreateEntity().Entity;
            _initialized = true;
        }

        /// <summary>
        /// Deserializes a world from a reader and initializes it
        /// Should be called after all systems are registered and before
        /// any entities that should remain are created.
        /// </summary>
        public void InitWorld(BinaryReader reader)
        {
            if (_initialized)
            {
                LuxCommon.Assert(false);
                return;
            }

            // TODO: move to protobuf and Initialize everything else - entity generator, masks, etc.
            _componentManagers = DeserializeToComponentManagers(reader);
            _singletonEntity = DeserializeSingletonEntity(reader);

            foreach (var system in _systems)
            {
                system.RunInitSingleton();
            }

            foreach (var system in _systems)
            {
                system.RunInit();
            }

            foreach (var system in _systems)
            {
                system.RunLoadContent();
            }

            _initialized = true;
        }

        public ComponentMask GetSingletonMask()
        {
            return _entityMasks[_singletonEntity];
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
                system.RunOnDestroyEntity(entity);
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

        public bool TryUnpack<T>(Entity entity, out T outComponent) where T : BaseComponent<T>
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

        public bool TryUnpackSingleton<T>(out T outComponent) where T : BaseComponent<T>
        {
            return TryUnpack(_singletonEntity, out outComponent);
        }

        public T Unpack<T>(Entity entity) where T: BaseComponent<T>
        {
            T component;
            bool unpackSuccess = TryUnpack(entity, out component);

            // Always expecting success
            LuxCommon.Assert(unpackSuccess);

            return component;
        }

        public T UnpackSingleton<T>() where T : BaseComponent<T>
        {
            return Unpack<T>(_singletonEntity);
        }

        public void AddSingletonComponent<T>(BaseComponent<T> component) where T : BaseComponent<T>
        {
            // If iterating systems, add the component afterwards instead of now
            if (_systems.IsIterating)
            {
                _postponedSystemActions.Enqueue(() => AddSingletonComponent(component));
                return;
            }

            // Add component for the singleton entity
            AddComponent(_singletonEntity, component);

            // Disable system if new signature doesn't match
            foreach (var system in _systems)
            {
                system.UpdateSingleton(GetSingletonMask());
            }
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
            if (null == foundComponentManager)
            {
                LuxCommon.Assert(false); // The component wasn't included in any system
                return;
            }

            // If component already exists, remove it first
            if (foundComponentManager.GetComponent(entity, out _))
            {
                LuxCommon.Assert(false); // This shouldn't happen, better to manually remove
                RemoveComponent<T>(entity);
            }

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
                LuxCommon.Assert(_inInitSingleton);
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
            _systems.Add(system);
        }

        /// <summary>
        /// Registers a component type to a world. Does nothing if already registered.
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

            // TODO: DITCH BinaryFormatter for Protobuf or XML serializer.
            // It's not cross platform, weighs a lot, break through versions, etc. https://stackoverflow.com/questions/7964280/c-sharp-serialize-generic-listcustomobject-to-file

            // Serialize singleton entity
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writer.BaseStream, _singletonEntity);
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
            if (BaseComponent<T>.ComponentType == -1)
            {
                return null;
            }

            return (ComponentManager<T>)_componentManagers[BaseComponent<T>.ComponentType];
        }

        #region Phases

        public virtual void InitSingleton()
        {
            _inInitSingleton = true;
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunInitSingleton();
            }
            _inInitSingleton = false;
        }

        public virtual void Init()
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunInit();
            }
        }

        public virtual void LoadContent()
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunLoadContent();
            }
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunPreUpdate(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunUpdate(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunPostUpdate(gameTime);
            }
        }

        public virtual void PrePreDraw(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunPrePreDraw(gameTime);
            }
        }

        public virtual void PreDraw(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunPreDraw(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunDraw(gameTime);
            }
        }

        public virtual void PostDraw(GameTime gameTime)
        {
            foreach (InternalBaseSystem system in _systems)
            {
                system.RunPostDraw(gameTime);
            }
        }

        #endregion
    }
}
