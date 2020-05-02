using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lux.Framework.ECS
{
    // This exists so we can create an array of components datas without
    // specifying the type <T>
    public interface IComponentsData
    {
        void RemoveComponent(Entity entity);
        void Serialize(BinaryWriter writer);
    }

    public class ComponentsData<T> : IComponentsData where T : AComponent<T>
    {
        private readonly SparseSet<T, Entity> _components;

        public ComponentsData(int maxComponents)
        {
            _components = new SparseSet<T, Entity>(maxComponents);
        }

        public ComponentsData(SparseSet<T, Entity> components)
        {
            _components = components;
        }

        /// <summary>
        /// Gets the given entity's component
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Component to return</param>
        /// <returns>
        /// <c>true</c> if component exists for the entity; <c>false</c> otherwise.
        /// </returns>
        public bool GetComponent(Entity entity, out T outComponent)
        {
            // Get the entity's component; if it doesn't exist return false
            if (!_components.GetValue(entity, out outComponent))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get all components
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> GetAllReadonly()
        {
            return _components.GetAllReadonly();
        }

        /// <summary>
        /// Adds a component to the dataset.
        /// The component can later be queried with the entity as the key.
        /// </summary>
        /// <param name="entity">Entity that corresponds to the given component</param>
        /// <param name="component">Component to add</param>
        public void AddComponent(Entity entity, T component)
        {
            _components.Add(entity, component);
        }

        /// <summary>
        /// Removes a component from the dataset if exists for the given entity
        /// </summary>
        /// <param name="entity">Entity that corresponds to the component</param>
        public void RemoveComponent(Entity entity)
        {
            // If component doesn't exist for the entity, do nothing
            if (!GetComponent(entity, out _))
            {
                return;
            }

            _components.Remove(entity);
        }

        /// <summary>
        /// Serialize the components data and write it into the stream
        /// </summary>
        /// <param name="writer">Writer to write the data into</param>
        public void Serialize(BinaryWriter writer)
        {
            // Save type name for later deserialization
            string typeName = typeof(T).AssemblyQualifiedName;
            writer.Write(typeName);

            // Serialize the actual components data
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writer.BaseStream, _components);
        }
    }
}
