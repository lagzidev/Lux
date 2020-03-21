using System;
namespace LuxEngine
{
    public enum ComponentType : int
    {
        DebugNameComponent,
        InputSingleton,
        ResolutionSingleton,
        TransformComponent,
        SpriteComponent,
        RelationshipComponent,
        PlatformerPlayerControlled,
        Camera,
        PlatformerLens,

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
