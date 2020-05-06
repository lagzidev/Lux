using System;

namespace Lux.Framework.ECS
{
    /// <summary>
    /// Contains the components data. This class is static for easy access
    /// like so: ComponentData<Transform>.GetAll()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class ComponentsData<T> where T : IComponent
    {
        private static SparseSet<T, Entity> _components;

        public static int Count
        {
            get
            {
                return _components.Count;
            }
        }

        public static ReadOnlySpan<Entity> Entities
        {
            get
            {
                return _components.Keys;
            }
        }

        public static void Init(int maxComponents)
        {
            if (_components == null)
            {
                _components = new SparseSet<T, Entity>(maxComponents);
            }
        }

        public static void Init(SparseSet<T, Entity> components)
        {
            _components = components;
        }

        /// <summary>
        /// Gets the component associated with the given entity
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Component to return</param>
        /// <returns>
        /// <c>true</c> if component exists for the entity; <c>false</c> otherwise.
        /// </returns>
        public static bool Get(Entity entity, out T outComponent)
        {
            return _components.GetValue(entity, out outComponent);
        }

        /// <summary>
        /// Get all components
        /// </summary>
        /// <returns>All components</returns>
        public static Span<T> GetAll()
        {
            return _components.GetAll();
        }

        /// <summary>
        /// Gets all components as readonly
        /// </summary>
        /// <returns>All components</returns>
        public static ReadOnlySpan<T> GetAllReadonly()
        {
            return _components.GetAllReadonly();
        }

        /// <summary>
        /// Adds a component for a given entity.
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="component">Component to add</param>
        public static void Add(Entity entity, T component)
        {
            _components.Add(entity, component);
        }

        /// <summary>
        /// Removes a component from the dataset if exists for the given entity
        /// </summary>
        /// <param name="entity">Entity that corresponds to the component</param>
        public static void Remove(Entity entity)
        {
            _components.Remove(entity);
        }
    }
}
