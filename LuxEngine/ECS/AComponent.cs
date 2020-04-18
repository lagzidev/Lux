using System;
namespace LuxEngine.ECS
{
    /// <summary>
    /// Wrap this in a component to get their previous state.
    /// The system won't run if there's no previous state available
    /// (e.g. if the component was only just created.)
    /// </summary>
    /// <typeparam name="T">The component</typeparam>
    public class Previous<T> : AComponent<Previous<T>> where T : AComponent<T>
    {
        public T Value;
    }

    /// <summary>
    /// You shouldn't inherit from this. Instead use AComponent<T> where T is
    /// your component class. (e.g. class Transform : AComponent<Transform>)    
    /// </summary>
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
