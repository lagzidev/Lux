using System.Collections.Generic;
namespace LuxEngine.ECS
{
    /// <summary>
    /// A proxy to all of the ECS worlds.
    /// </summary>
    public class ECS
    {
        private readonly List<InternalWorld> _worlds;

        public ECS()
        {
            _worlds = new List<InternalWorld>();
        }

        /// <summary>
        /// Creates a new ECS world
        /// </summary>
        /// <returns>The newly created ECS world</returns>
        public WorldHandle CreateWorld()
        {
            InternalWorld newWorld = new InternalWorld();
            _worlds.Add(newWorld);

            return newWorld.WorldHandle;
        }

        public void Initialize()
        {
            _worlds.ForEach(x => x.Init());
        }

        public void Update()
        {
            _worlds.ForEach(x => x.Update());
        }

        public void UpdateFixed()
        {
            _worlds.ForEach(x => x.UpdateFixed());
        }

        public void Draw()
        {
            _worlds.ForEach(x => x.Draw());
        }

        // TODO: Deinitialize ?
    }
}