using System;
namespace Lux.Framework.ECS
{
    public interface IComponent
    {
    }

    /// <summary>
    /// Limits a component type to only exist on the global Singleton Entity.
    /// </summary>
    public interface ISingleton
    {
    }

    /// <summary>
    /// You shouldn't inherit from this.   
    /// </summary>
    [Serializable]
    public abstract class AInternalComponent
    {
        protected static int ComponentTypesCount = 0;
    }

    [Serializable]
    public sealed class AComponent<T> : AInternalComponent where T : IComponent
    {
        public static int ComponentType = -1;

        /// <summary>
        /// Sets the component class' type. Does nothing if already set.
        /// </summary>
        internal static void SetComponentType()
        {
            // If component type already set
            if (ComponentType != -1)
            {
                return;
            }

            // If there are too many component types
            if (ComponentTypesCount >= HardCodedConfig.MAX_GAME_COMPONENT_TYPES)
            {
                LuxCommon.Assert(false);
                return;
            }

            ComponentType = ComponentTypesCount;
            ComponentTypesCount++;
        }
    }
}
