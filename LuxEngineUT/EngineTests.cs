using NUnit.Framework;
using LuxEngine;

namespace LuxEngineUT
{
    public class EngineTests
    {
        private ECS _engine;

        [SetUp]
        public void Setup()
        {
            _engine = new ECS();
        }

        [Test]
        public void CreateWorldSanity()
        {
            World world = _engine.CreateWorld();

            EntityHandle entity = world.CreateEntity();
        }

        [Test]
        public void UpdateSanity()
        {
        }
    }
}