using System;
using BenchmarkDotNet.Attributes;
using Lux.Framework.ECS;

namespace Lux.Benchmark
{
    [MemoryDiagnoser]
    public class Playground
    {
        private ECS _ecs;
        private WorldHandle _directAccess;
        private WorldHandle _spanAccess;

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

        public class DirectAccess : IFeature, IUpdateFeature, IInitFeature
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

        public class SpanAccess : IFeature, IUpdateFeature, IInitFeature
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

        [IterationSetup]
        public void Setup()
        {
            _ecs = new ECS();

            _directAccess = _ecs.CreateWorld();
            _directAccess.AddFeature(new DirectAccess());
            _directAccess.Init();

            _spanAccess = _ecs.CreateWorld();
            _spanAccess.AddFeature(new SpanAccess());
            _spanAccess.Init();
        }

        [Benchmark]
        public void UpdateDirectAccess()
        {
            for (int i = 0; i < 1000; i++)
            {
                _directAccess.Update();
            }
        }

        [Benchmark]
        public void UpdateSpanAccess()
        {
            for (int i = 0; i < 1000; i++)
            {
                _spanAccess.Update();
            }
        }
    }
}
