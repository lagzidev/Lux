using System;

namespace LuxEngine
{
    // This exists so we can create an array of component managers without
    // specifying the type <T>
    public abstract class BaseComponentManager
    {
        public abstract void RemoveComponent(Entity entity);
    }

    public class ComponentManager<T> : BaseComponentManager
    {
        private SparseSet<BaseComponent<T>> _components;

        public ComponentManager()
        {
            _components = new SparseSet<BaseComponent<T>>(HardCodedConfig.MAX_COMPONENTS_PER_TYPE);
        }

        /// <summary>
        /// Adds a component to the dataset.
        /// The component can later be queried with the entity as the key.
        /// </summary>
        /// <param name="entity">Entity that corresponds to the given component</param>
        /// <param name="component">Component to add</param>
        public void AddComponent(Entity entity, BaseComponent<T> component)
        {
            _components.Add(entity.Index, component);
        }

        /// <summary>
        /// Removes a component from the dataset if exists for the given entity
        /// </summary>
        /// <param name="entity">Entity that corresponds to the component</param>
        public override void RemoveComponent(Entity entity)
        {
            // If component doesn't exist for the entity, do nothing
            if (!GetComponent(entity, out _))
            {
                return;
            }

            _components.Remove(entity.Index);
        }

        public bool GetComponent(Entity entity, out BaseComponent<T> outComponent)
        {
            // Get the entity's component; if it doesn't exist return false
            if (!_components.GetValue(entity.Index, out outComponent))
            {
                return false;
            }

            // If the entity is not of the correct generation (meaning it's not the same entity)
            if (outComponent.Entity.Generation != entity.Generation)
            {
                return false;
            }

            return true;
        }
    }
}
