using System;
namespace LuxEngine
{
    [Serializable]
    public class InternalBaseComponent
    {
    }

    [Serializable]
    public abstract class BaseComponent<T> : InternalBaseComponent
    {
        public static int ComponentType { get; set; } = -1;
        public Entity Entity { get; set; }
    }
}
