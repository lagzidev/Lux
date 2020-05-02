using System;
namespace Lux.Framework.ECS
{
    /// <summary>
    /// Wrap this in a component to get their previous state.
    /// The system won't run if there's no previous state available
    /// (e.g. if the component was only just created.)
    /// </summary>
    /// <typeparam name="T">The component type</typeparam>
    //public interface IPrevious<T> : AComponent<IPrevious<T>> where T : AComponent<T>
    //{
    //    T Value { get; set; }
    //}

    //public interface IExclude
    //{
    //}

    /// <summary>
    /// Let's the API know that the real component is inside
    /// </summary>
    //public interface IWrapper<T>
    //{
    //    T Value { get; set; }
    //}
    //s

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
