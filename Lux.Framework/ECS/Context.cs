namespace Lux.Framework.ECS
{
    /// <summary>
    /// The way systems interact with the world and create entities.
    /// </summary>
    /// TODO: Support Context for creating entities from inside systems
    /// TODO: Try extending Context with functions like CreateRandomMob(params MobTypes[] types)!
    public class Context : AComponent<Context>, ISingleton
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

        public void AddComponent<T>(Entity entity, T component) where T : AComponent<T>
        {
            _world.AddComponent(entity, component);
        }

        public void AddSingleton<T>(T component) where T : AComponent<T>, ISingleton
        {
            _world.AddSingletonComponent(component);
        }
    }
}
