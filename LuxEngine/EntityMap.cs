using System;
using System.Collections.Generic;

namespace LuxEngine
{
    /// <summary>
    /// A bidirectional map (Entity <-> ComponentInstance)
    /// There is one EntityMap for each ComponentManager<T>
    /// </summary>
    public class EntityMap
    {
        private SortedDictionary<Entity, ComponentInstance> entityToComponent;
        private Entity[] componentToEntity;

        public EntityMap()
        {
            entityToComponent = new SortedDictionary<Entity, ComponentInstance>();
            componentToEntity = new Entity[HardCodedConfig.MAX_COMPONENTS_PER_TYPE];
        }

        public Entity GetEntity(ComponentInstance componentInstance)
        {
            return componentToEntity[componentInstance.Index];
        }

        /// <summary>
        /// Exception "safe" version of GetComponentInstance.
        /// Get the component instance of an entity
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="componentInstanceOut">Component instance to return</param>
        /// <returns>Found component instance or not</returns>
        public bool TryGetComponentInstance(Entity entity, out ComponentInstance componentInstanceOut)
        {
            ComponentInstance componentInstance;
            if (!entityToComponent.TryGetValue(entity, out componentInstance))
            {
                componentInstanceOut = default;
                return false;
            }

            componentInstanceOut = componentInstance;
            return true;
        }

        /// <summary>
        /// Get the component instance of an entity
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <returns>Component instance of the entity</returns>
        public ComponentInstance GetComponentInstance(Entity entity)
        {
            ComponentInstance componentInstance;
            if (!TryGetComponentInstance(entity, out componentInstance))
            {
                throw new LuxException(LuxStatus.ENTITYMAP_GETCOMPONENTINSTANCE_COMPONENT_DOES_NOT_EXIST_FOR_THIS_ENTITY, entity.Id);
            }

            return componentInstance;
        }

        public void Update(Entity entity, ComponentInstance componentInstance)
        {
            entityToComponent[entity] = componentInstance;
            componentToEntity[componentInstance.Index] = entity;
        }

        public void Add(Entity entity, ComponentInstance componentInstance)
        {
            Update(entity, componentInstance);
        }

        public void Remove(Entity entity)
        {
            entityToComponent.Remove(entity);
        }
    }
}
