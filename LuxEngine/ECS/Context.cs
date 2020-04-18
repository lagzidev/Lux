namespace LuxEngine.ECS
{
    /// <summary>
    /// The way systems interact with the world and create entities.
    /// </summary>
    /// TODO: Support Context for creating entities from inside systems
    /// TODO: Try extending Context with functions like CreateRandomMob(params MobTypes[] types)!
    public class Context : AComponent<Context>
    {
        private InternalWorld _world;

        internal Context(InternalWorld world)
        {
            _world = world;
        }

        public Entity CreateEntity()
        {
            return _world.CreateEntity();
        }
    }
}
