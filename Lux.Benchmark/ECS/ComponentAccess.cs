using System;
using BenchmarkDotNet.Attributes;
using Lux.ECS;

namespace Lux.Benchmark
{
    [MemoryDiagnoser]
    public class ComponentAccess
    {
        private ECS.ECS _ecs;
        private WorldHandle _noComponents;
        private WorldHandle _singleComponent;
        private WorldHandle _twoComponents;

        [Params(100, 1000, 10000, 100000)]
        public static int EntityCount { get; set; }

        public class Position : AComponent<Position> // TODO: CHANGE TO STRUCT
        {
            public float X;
            public float Y;

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public class Speed : AComponent<Speed>
        {
            public float X;
            public float Y;

            public Speed(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public static void Init(Context context)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                Entity entity = context.CreateEntity();
                context.AddComponent(entity, new Position(0, 0));
                context.AddComponent(entity, new Speed(1, 2));
            }
        }

        public class SingleFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Position position) =>
                {
                    position.X += 1;
                    position.Y += 1;
                });
            }
        }

        public class EmptyFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add(() =>
                {
                });
            }
        }

        public class TwoFeature : IFeature, IUpdateFeature, IInitFeature
        {
            public void Init(Systems systems)
            {
                systems.Add<Context>(ComponentAccess.Init);
            }

            public void Update(Systems systems)
            {
                systems.Add((Position position, Speed speed) =>
                {
                    position.X += speed.X;
                    position.Y += speed.Y;
                });
            }
        }

        [IterationSetup]
        public void Setup()
        {
            _ecs = new ECS.ECS();

            _noComponents = _ecs.CreateWorld();
            _noComponents.AddFeature(new EmptyFeature());
            _noComponents.Init();

            _singleComponent = _ecs.CreateWorld();
            _singleComponent.AddFeature(new SingleFeature());
            _singleComponent.Init();

            _twoComponents = _ecs.CreateWorld();
            _twoComponents.AddFeature(new TwoFeature());
            _twoComponents.Init();
        }

        [Benchmark]
        public void UpdateNoComponents()
        {
            for (int i = 0; i < 1000; i++)
            {
                _noComponents.Update();
            }
        }

        [Benchmark]
        public void UpdateSingleComponent()
        {
            for (int i = 0; i < 1000; i++)
            {
                _singleComponent.Update();
            }
        }

        [Benchmark]
        public void UpdateTwoComponents()
        {
            for (int i = 0; i < 1000; i++)
            {
                _twoComponents.Update();
            }
        }
    }
}
