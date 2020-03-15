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
            _lastId++;

            Entity entity;
            entity.Id = _lastId;

            return entity;
        }
    }
}
