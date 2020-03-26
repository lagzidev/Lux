using System;
namespace LuxEngine
{
    public class InternalBaseComponent
    {
    }

    public abstract class BaseComponent<T> : InternalBaseComponent
    {
        public static int ComponentType { get; set; }
        public Entity Entity { get; set; }
    }
}
