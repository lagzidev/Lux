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
        public static int ComponentType { get; set; }
        public Entity Entity { get; set; }
    }
}
