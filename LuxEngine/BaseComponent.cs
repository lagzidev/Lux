using System;
namespace LuxEngine
{
    public enum ComponentType : int
    {
        DebugNameComponent,
        TransformComponent,
        SpriteComponent,

        // Always last
        ComponentTypeCount,
        MaxComponentTypes = HardCodedConfig.MAX_COMPONENT_TYPES
    }

    public abstract class BaseComponent<T>
    {
        public static ComponentType ComponentType { get; set; }
        public Entity Entity { get; set; }
    }
}
