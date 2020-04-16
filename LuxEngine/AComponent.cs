using System;
namespace LuxEngine
{
    [Serializable]
    public abstract class AInternalComponent
    {
        protected static int ComponentTypesCount = 0;
    }

    [Serializable]
    public abstract class AComponent<T> : AInternalComponent
    {
        internal Entity _entity { get; set; }
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
