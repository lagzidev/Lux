using System;
namespace Lux.Framework.ECS
{
    public class Previous<T> : IComponent where T : IComponent
    {
        public readonly T Value;

        public Previous(T value)
        {
            Value = value;
        }
    }
}
