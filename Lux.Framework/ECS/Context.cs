using System;

namespace Lux.Framework.ECS
{
    /// <summary>
    /// The way systems interact with the world and create entities.
    /// </summary>
    /// TODO: Support Context for creating entities from inside systems
    /// TODO: Try extending Context with functions like CreateRandomMob(params MobTypes[] types)!
    public class Context : IComponent, ISingleton
    {
        private readonly World _world;

        internal Context(World world)
        {
            _world = world;
        }

        public Entity CreateEntity()
        {
            return _world.CreateEntity();
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            _world.AddComponent(entity, component);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            _world.RemoveComponent<T>(entity);
        }

        public void AddSingleton<T>(T component) where T : IComponent, ISingleton
        {
            _world.AddSingletonComponent(component);
        }

        // TODO: Delete this. it's flawed. The returned components are not ordered by entity
        public Span<T> GetAll<T>() where T : IComponent
        {
            return _world.GetAll<T>();
        }

        public ReadOnlySpan<T> GetAllReadonly<T>() where T : IComponent
        {
            return _world.GetAllReadonly<T>();
        }
    }
}
