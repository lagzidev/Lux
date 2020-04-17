using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class ECS
    {
        public readonly List<World> Worlds;

        public ECS()
        {
            Worlds = new List<World>();
        }

        /// <summary>
        /// Creates a new ECS world
        /// </summary>
        /// <returns>The newly created ECS world</returns>
        public World CreateWorld()
        {
            World newWorld = new World();
            Worlds.Add(newWorld);

            return newWorld;
        }

        public void Initialize()
        {
            Worlds.ForEach(x => x.Init());
        }

        public void LoadContent()
        {
            Worlds.ForEach(x => x.LoadContent());
        }

        public void Update()
        {
            Worlds.ForEach(x => x.Update());
        }

        public void UpdateFixed()
        {
            Worlds.ForEach(x => x.UpdateFixed());
        }

        public void Draw()
        {
            Worlds.ForEach(x => x.Draw());
        }
    }
}