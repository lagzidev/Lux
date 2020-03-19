using System;
namespace LuxEngine
{
    /// <summary>
    /// Provides an interface for interacting with an entity.
    /// Even though the entities' components are stored in completely different
    /// places, this class lets them all be accessed as if they were part of
    /// the entity itself.
    /// </summary>
    /// TODO: Swap EntityHandle and Entity's names - it's confusing, a handle is more data oriented not an interface
    public class EntityHandle
    {
        public Entity Entity;
        public World World { private get; set; }

        public EntityHandle(Entity entity, World world)
        {
            Entity = entity;
            World = world;
        }

        public void AddComponent<T>(BaseComponent<T> component)
        {
            component.Entity = Entity;
            World.AddComponent(Entity, component);
        }

        public void RemoveComponent<T>()
        {
            World.RemoveComponent<T>(Entity);
        }

        public T Unpack<T>()
        {
            return World.Unpack<T>(Entity);
        }
    }
}
