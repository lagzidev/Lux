using System;
namespace LuxEngine
{
    public class EntityManager
    {
        ulong _lastId;

        public EntityManager()
        {
            _lastId = 0;
        }

        public Entity CreateEntity()
        {
            Entity entity;
            entity.Id = _lastId;

            _lastId++;

            return entity;
        }
    }
}
