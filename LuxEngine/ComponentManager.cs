using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace LuxEngine
{
    // This exists so we can create an array of component managers without
    // specifying the type <T>
    public abstract class BaseComponentManager
    {
        /// <summary>
        /// Deserializes a component manager from a stream
        /// </summary>
        /// <param name="reader">Reader to read the serialized data from</param>
        /// <returns>A component manager of the given type</returns>
        //public static BaseComponentManager Deserialize(BinaryReader reader)
        //{
        //}

        public abstract void RemoveComponent(Entity entity);
        public abstract void Serialize(BinaryWriter writer);
    }

    public class ComponentManager<T> : BaseComponentManager
    {
        private SparseSet<AComponent<T>> _components;

        public ComponentManager()
        {
            _components = new SparseSet<AComponent<T>>(HardCodedConfig.MAX_COMPONENTS_PER_TYPE);
        }

        public ComponentManager(BaseSparseSet components)
        {
            _components = (SparseSet<AComponent<T>>)components;
        }

        /// <summary>
        /// Gets the given entity's component
        /// </summary>
        /// <param name="entity">Entity that owns the component</param>
        /// <param name="outComponent">Component to return</param>
        /// <returns>
        /// <c>true</c> if component exists for the entity; <c>false</c> otherwise.
        /// </returns>
        public bool GetComponent(Entity entity, out AComponent<T> outComponent)
        {
            // Get the entity's component; if it doesn't exist return false
            if (!_components.GetValue(entity.Index, out outComponent))
            {
                return false;
            }

            // If the entity is not of the correct generation (meaning it's not the same entity)
            if (outComponent.Entity.Generation != entity.Generation)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a component to the dataset.
        /// The component can later be queried with the entity as the key.
        /// </summary>
        /// <param name="entity">Entity that corresponds to the given component</param>
        /// <param name="component">Component to add</param>
        public void AddComponent(Entity entity, AComponent<T> component)
        {
            _components.Add(entity.Index, component);
        }

        /// <summary>
        /// Removes a component from the dataset if exists for the given entity
        /// </summary>
        /// <param name="entity">Entity that corresponds to the component</param>
        public override void RemoveComponent(Entity entity)
        {
            // If component doesn't exist for the entity, do nothing
            if (!GetComponent(entity, out _))
            {
                return;
            }

            _components.Remove(entity.Index);
        }

        /// <summary>
        /// Serialize the component manager and write it into the stream
        /// </summary>
        /// <param name="writer">Writer to write the data into</param>
        public override void Serialize(BinaryWriter writer)
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
