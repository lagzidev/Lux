using System;
namespace Lux.Framework.ECS
{
    internal interface IPrevious
    {
    }

    public class Previous<T> : IComponent, IPrevious where T : IComponent
    {
        public readonly T Value;

        public Previous(T value)
        {
            Value = value;
        }
    }
}
