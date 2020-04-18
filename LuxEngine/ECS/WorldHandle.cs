using System;

namespace LuxEngine.ECS
{
    public class WorldHandle
    {
        internal readonly Systems InitSystems;
        internal readonly Systems UpdateSystems;
        internal readonly Systems UpdateFixedSystems;
        internal readonly Systems DrawSystems;

        internal readonly Systems OnDestroyEntitySystems;
        internal readonly Systems OnAddComponentSystems;
        internal readonly Systems OnRemoveComponentSystems;

        internal DefaultEntityFilter EntityFilter { get; set; }

        internal WorldHandle()
        {
            InitSystems = new Systems();
            UpdateSystems = new Systems();
            UpdateFixedSystems = new Systems();
            DrawSystems = new Systems();

            OnDestroyEntitySystems = new Systems();
            OnAddComponentSystems = new Systems();
            OnRemoveComponentSystems = new Systems();

            EntityFilter = new DefaultEntityFilter();
        }

        internal void RegisterAllComponents(InternalWorld world)
        {
            InitSystems.Register(world);
            UpdateSystems.Register(world);
            UpdateFixedSystems.Register(world);
            DrawSystems.Register(world);

            OnDestroyEntitySystems.Register(world);
            OnAddComponentSystems.Register(world);
            OnRemoveComponentSystems.Register(world);
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
                castFeature?.Init(InitSystems);
            }

            {
                // Add the feature's update systems to the updateSystems list
                var castFeature = feature as IUpdateFeature;
                castFeature?.Update(UpdateSystems);
            }

            {
                // Add the feature's update systems to the updateSystems list
                var castFeature = feature as IUpdateFixedFeature;
                castFeature?.UpdateFixed(UpdateFixedSystems);
            }

            {
                // Add the feature's draw systems to the drawSystems list
                var castFeature = feature as IDrawFeature;
                castFeature?.Draw(DrawSystems);
            }

            {
                // Add systems subscribed to OnEntityDestroy
                var castFeature = feature as IOnDestroyEntityFeature;
                castFeature?.OnDestroyEntity(OnDestroyEntitySystems);
            }

            {
                // Add systems subscribed to OnAddComponent
                var castFeature = feature as IOnAddComponentFeature;
                castFeature?.OnAddComponent(OnAddComponentSystems);
            }

            {
                // Add systems subscribed to OnRemoveComponent
                var castFeature = feature as IOnRemoveComponentFeature;
                castFeature?.OnRemoveComponent(OnRemoveComponentSystems);
            }
        }
    }
}
