using System;
namespace LuxEngine
{
    [Serializable]
    public abstract class InternalBaseComponent
    {
        protected static int ComponentTypesCount = 0;
    }

    [Serializable]
    public abstract class BaseComponent<T> : InternalBaseComponent
    {
        public Entity Entity { get; set; }
        public static int ComponentType = -1;

        /// <summary>
        /// Sets the component class' type. Does nothing if already set.
        /// </summary>
        public static void SetComponentType()
        {
            // If component type not already set
            if (ComponentType == -1)
            {
                ComponentType = ComponentTypesCount;
                ComponentTypesCount++;
            }
        }
    }
}
