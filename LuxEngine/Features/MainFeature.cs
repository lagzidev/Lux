using System;
using LuxEngine.ECS;

namespace LuxEngine
{
    public class MainFeature : IFeature, IUpdateFeature
    {
        private static void Init(Context context)
        {
            Entity player = context.CreateEntity();
            context.AddComponent(player, new Transform(440, 255));
            context.AddComponent(player, new Moveable(60, 60));
            context.AddComponent(player, new Sprite("Lara"));
            context.AddComponent(player, new PlayerControlledTD());

            //EntityHandle background = World.CreateEntity();
            //background.AddComponent(new Transform(0, 0));
            //background.AddComponent(new Sprite("Background"));
        }

        private static void Update()
        {
            Console.WriteLine("Update");
        }

        public void Update(Systems systems)
        {
            systems.Add(Update);
        }
    }
}
