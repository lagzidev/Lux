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

        public ComponentInstance GetComponentInstance(Entity entity)
        {
            return entityToComponent[entity];
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
