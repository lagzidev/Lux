using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class ECS
    {
        private readonly List<World> _worlds;

        public ECS()
        {
            _worlds = new List<World>();
        }

        /// <summary>
        /// Creates a new ECS world
        /// </summary>
        /// <returns>The newly created ECS world</returns>
        public World CreateWorld()
        {
            World newWorld = new World();
            _worlds.Add(newWorld);

            return newWorld;
        }

        public void Initialize()
        {
            _worlds.ForEach(x => x.Init());
        }

        public void LoadContent()
        {
            _worlds.ForEach(x => x.LoadContent());
        }

        public void Update()
        {
            _worlds.ForEach(x => x.Update());
        }

        public void Draw()
        {
            _worlds.ForEach(x => x.Draw());
        }
    }
}