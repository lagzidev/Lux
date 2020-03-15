using System;
namespace LuxEngine
{
    public enum ComponentType
    {
        TransformComponent,
        DebugNameComponent,
    }

    public abstract class BaseComponent<T>
    {
        public Entity Entity { get; set; }
        public static ComponentType ComponentType { get; private set; }

        public BaseComponent(ComponentType componentType)
        {
            ComponentType = componentType;
        }
    }
}
