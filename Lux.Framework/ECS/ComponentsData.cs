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
        private static SparseSet<T, Entity> Components
        {
            get
            {
                if (_components == null)
                {
                    int maxComponents = HardCodedConfig.MAX_ENTITIES_PER_WORLD;
                    if (typeof(IUnique).IsAssignableFrom(typeof(T)))
                    {
                        maxComponents = 1;
                    }

                    _components = new SparseSet<T, Entity>(maxComponents, HardCodedConfig.MAX_ENTITIES_PER_WORLD);
                }

                return _components;
            }
            set
            {
                _components = value;
            }
        }

        public static int Count
        {
            get
            {
                return Components.Count;
            }
        }

        public static ReadOnlySpan<Entity> Entities
        {
            get
            {
                return Components.Keys;
            }
        }

        //public static void Init(SparseSet<T, Entity> components)
        //{
        //    Components = components;
        //}

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
            return Components.GetValue(entity, out outComponent);
        }

        /// <summary>
        /// Get all components
        /// </summary>
        /// <returns>All components</returns>
        public static Span<T> GetAll()
        {
            return Components.GetAll();
        }

        /// <summary>
        /// Gets all components as readonly
        /// </summary>
        /// <returns>All components</returns>
        public static ReadOnlySpan<T> GetAllReadonly()
        {
            return Components.GetAllReadonly();
        }

        /// <summary>
        /// Adds a component for a given entity.
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="component">Component to add</param>
        public static void Add(Entity entity, T component)
        {
            Components.Add(entity, component);
        }

        /// <summary>
        /// Removes a component from the dataset if exists for the given entity
        /// </summary>
        /// <param name="entity">Entity that corresponds to the component</param>
        public static void Remove(Entity entity)
        {
            Components.Remove(entity);
        }
    }
}
