using System;
using System.Collections.Generic;

namespace LuxEngine
{
    public class EntityManager
    {
        Int16 _nextIndex;
        Stack<Entity> _destroyed_entities;

        public EntityManager()
        {
            _nextIndex = 0;
            _destroyed_entities = new Stack<Entity>();
        }

        public Entity CreateEntity()
        {
            Entity entity;

            // If there are destroyed entites to recycle
            if (0 != _destroyed_entities.Count)
            {
                // Recycle entity
                entity = _destroyed_entities.Pop();
                entity.Generation++;
            }
            else
            {
                // Create new entity
                entity = new Entity
                {
                    Index = _nextIndex,
                    Generation = 0
                };

                _nextIndex++;
            }

            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            _destroyed_entities.Push(entity);
        }
    }
}
