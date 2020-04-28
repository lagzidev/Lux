using System;
namespace LuxEngine.ECS
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
}
