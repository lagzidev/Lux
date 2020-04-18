using NUnit.Framework;
using LuxEngine.ECS;

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
            WorldHandle world = _engine.CreateWorld();
            //Entity entity = world.CreateEntity();
        }

        [Test]
        public void UpdateSanity()
        {
        }
    }
}