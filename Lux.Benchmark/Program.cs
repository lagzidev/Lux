using System;
using Lux.ECS;
using BenchmarkDotNet.Running;

namespace Lux.Benchmark
{
    class MainFeature : IFeature, IInitFeature
    {
        public void Init(Systems systems)
        {
            systems.Add(() =>
            {
                Console.WriteLine("Init");
            });
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[]
            {
                //typeof(DefaultEcs.CreateEntity),
                //typeof(DefaultEcs.EntitySetFilter),
                //typeof(DefaultEcs.MultipleFilterImpact),
                //typeof(DefaultEcs.EntitySetEnumeration),
                //typeof(DefaultEcs.EntitySetWithComponentEnumeration),
                //typeof(DefaultEcs.System),
                typeof(ComponentAccess),
                //typeof(DefaultEcs.Serialization),
                //typeof(Performance.SingleComponentEntityEnumeration),
                //typeof(Performance.DoubleComponentEntityEnumeration),
                //typeof(Message.Publish),
            }).RunAll();


            ECS.ECS ecs = new ECS.ECS();

            WorldHandle world = ecs.CreateWorld();
            world.AddFeature(new MainFeature());

            ecs.Initialize();
            ecs.Update();
        }
    }
}
