using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class Transform : BaseComponent<Transform>
    {
        public int X;
        public int Y;

        public Transform(int x, int y)
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
