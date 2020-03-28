using System;
namespace LuxEngine
{
    public class EntityManager
    {
        int _lastId;

        public EntityManager()
        {
            _lastId = 0;
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity
            {
                Id = _lastId
            };

            _lastId++;

            return entity;
        }

        public void DestroyEntity(Entity entity)
        {

        }
    }
}
