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

    /// <summary>
    /// Limits a component type to only exist on the global Singleton Entity.
    /// </summary>
    public interface ISingleton : IUnique
    {
    }

    /// <summary>
    /// Limits a component type to only exist on one entity at a time.
    /// </summary>
    public interface IUnique
    {
    }

    /// <summary>
    /// You shouldn't inherit from this. Instead use AComponent<T> where T is
    /// your component class. (e.g. class Transform : AComponent<Transform>)    
    /// </summary>
    [Serializable]
    public abstract class AInternalComponent
    {
        public Entity Entity { get; set; }
        protected static int ComponentTypesCount = 0;
    }

    [Serializable]
    public abstract class AComponent<T> : AInternalComponent
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
