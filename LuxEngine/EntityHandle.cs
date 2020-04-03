using System;
namespace LuxEngine
{
    /// <summary>
    /// Provides an interface for interacting with an entity.
    /// Even though the entities' components are stored in completely different
    /// places, this class lets them all be accessed as if they were part of
    /// the entity itself.
    /// </summary>
    public class EntityHandle
    {
        public Entity Entity;
        public World World { private get; set; }

        public EntityHandle(Entity entity, World world)
        {
            Entity = entity;
            World = world;
        }

        public void AddComponent<T>(T component) where T : BaseComponent<T>
        {
            World.AddComponent(Entity, component);
        }

        public void RemoveComponent<T>() where T : BaseComponent<T>
        {
            World.RemoveComponent<T>(Entity);
        }

        public T Unpack<T>() where T : BaseComponent<T>
        {
            return World.Unpack<T>(Entity);
        }

        public bool TryUnpack<T>(out T outComponent) where T : BaseComponent<T>
        {
            return World.TryUnpack<T>(Entity, out outComponent);
        }

        public void Destroy()
        {
            World.DestroyEntity(Entity);
        }
    }
}
