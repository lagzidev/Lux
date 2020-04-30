using NUnit.Framework;
using Lux.ECS;

namespace Lux.Tests
{
    public class EngineTests
    {
        private ECS.ECS _engine;

        [SetUp]
        public void Setup()
        {
            _engine = new ECS.ECS();
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