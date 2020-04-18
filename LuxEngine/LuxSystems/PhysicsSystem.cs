using System;
using Microsoft.Xna.Framework;
using LuxEngine.ECS;

namespace LuxEngine.LuxSystems
{
    public class Transform : AComponent<Transform>
    {
        public float X;
        public float Y;

        public Transform(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class WindResistent : AComponent<Transform>
    {
    }

    public static class WindSystemEx
    {
        //[ExcludeFilter(typeof(WindResistent)))]
        public static void Whoosh(Transform transform)
        {

        }

        //// Systems can ask for context to do things like 
        //public static void CreateLara(this Context context, Singleton<Map> map)
        //{
        //    Entity entity = context.CreateEntity();
        //    entity.Add(entity, new Transform(map.SpawnPoints[3].X, map.SpawnPoints[3].Y);

        //}
    }
}
