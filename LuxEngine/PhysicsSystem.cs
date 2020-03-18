using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class TransformComponent : BaseComponent<TransformComponent>
    {
        public float X;
        public float Y;

        public TransformComponent(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    //public class PhysicsSystem
    //{
    //    public PhysicsSystem()
    //    {
    //    }
    //}
}
