using System;
using System.Diagnostics;

namespace Lux.Framework.ECS
{
    /// <summary>
    /// A handle to an ECS world, through which you can add features (systems)
    /// and execute them.
    /// </summary>
    public class WorldHandle
    {
        public readonly  Systems OnAddComponentSystems;
        public readonly  Systems OnDestroyEntitySystems;

        private readonly  Systems _initSystems;
        private readonly  Systems _updateSystems;
        private readonly  Systems _updateFixedSystems;
        private readonly  Systems _drawSystems;

        private readonly World _world;


        internal WorldHandle()
        {
            OnAddComponentSystems = new Systems();
            OnDestroyEntitySystems = new Systems();
            _initSystems = new  Systems();
            _updateSystems = new  Systems();
            _updateFixedSystems = new  Systems();
            _drawSystems = new  Systems();

            _world = new World(this);
        }

        /// <summary>
        /// Asserts that the systems have the required attribute
        /// </summary>
        /// <typeparam name="T">The required system attribute</typeparam>
        /// <param name="systems">Systems that must have the attribute</param>
        [Conditional("DEBUG")]
        private static void AssertAttribute<T>(Systems systems) where T : ASystemAttribute
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if (!systems[i].HasAttribute<OnAddComponent>())
                {
                    LuxCommon.Assert(false); // The system doesn't have the required attribute
                    break;
                }
            }
        }

        private void RegisterAllComponents()
        {
            _initSystems.Register(_world);
            _updateSystems.Register(_world);
            _updateFixedSystems.Register(_world);
            _drawSystems.Register(_world);

            OnAddComponentSystems.Register(_world);
            AssertAttribute<OnAddComponent>(OnAddComponentSystems);

            OnDestroyEntitySystems.Register(_world);
            AssertAttribute<OnDestroyEntity>(OnDestroyEntitySystems);
        }

        /// <summary>
        /// Adds a feature to the world.
        /// </summary>
        /// <param name="feature">Feature to add</param>
        public void AddFeature(IFeature feature)
        {
            {
                // Add the feature's init systems to the init list
                var castFeature = feature as IInitFeature;
                castFeature?.Init(_initSystems);
            }

            {
                // Add the feature's update systems to the updateSystems list
                var castFeature = feature as IUpdateFeature;
                castFeature?.Update(_updateSystems);
            }

            {
                // Add the feature's updatefixed systems to the updateSystems list
                var castFeature = feature as IUpdateFixedFeature;
                castFeature?.UpdateFixed(_updateFixedSystems);
            }

            {
                // Add the feature's draw systems to the drawSystems list
                var castFeature = feature as IDrawFeature;
                castFeature?.Draw(_drawSystems);
            }

            {
                // Add systems subscribed to an event
                var castFeature = feature as IOnAddComponent;
                castFeature?.OnAddComponent(OnAddComponentSystems);
            }

            
        }

        #region Phases

        public void Init()
        {
            // Register all the components used by the systems
            RegisterAllComponents();

            _world.Run(_initSystems);
        }

        public void Update()
        {
            _world.Run(_updateSystems);
        }

        public void UpdateFixed()
        {
            _world.Run(_updateFixedSystems);
        }

        public void Draw()
        {
            // TODO: Disable draw if the world is just a server
            // Important to make sure no state is being mutated in Draw
            // so the game logic doesn't get affected

            _world.Run(_drawSystems);
        }

        #endregion
    }
}
