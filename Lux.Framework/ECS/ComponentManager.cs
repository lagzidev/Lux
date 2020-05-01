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
        private readonly SparseSet<T> _components;

        public ComponentsData(int maxComponents)
        {
            _components = new SparseSet<T>(maxComponents);
        }

        public ComponentsData(SparseSet<T> components)
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
            _components.Add(entity.Index, component);
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

            _components.Remove(entity.Index);
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

        /// <summary>
        /// The problem with this is that it's a lot of work to copy all of these
        /// every update. So we have to do this only when the component is changed.
        /// How will we know when the component is changed? We should expose a
        /// SetComponent method that will trigger the state preservation. This could
        /// also be useful for supporting events like OnComponentSet. The downside
        /// of SetComponent is first that it's inconvenient to have to call SetComponent
        /// every time we mutate a component. bleh. We COULD track which components
        /// are unpacked in a system call and SetComponent them ourselves, but how do
        /// we know if they were mutated? We wouldn't want to set them if they weren't, right?
        /// We COULD add another unpack method: UnpackToMutate, which will tell us
        /// if the changes to the unpacked components are meant to be mutated or not.
        /// But then how do we make sure "Unpack"-ed components don't mutate?
        /// C# doesn't support const. We COULD save the original state aside when using Unpack
        /// and restore it at the end of the system call. But that seems a bit backwards
        /// and would require copying data for no good reason. What we COULD do though is
        /// wrap the component returned from Unpack in an immutable struct. Something like:
        /// Immutable<Transform> that contains { public readonly Transform obj; }
        /// The advantage of this is that it's super lightweight - we're only
        /// creating a struct that contains a reference to our component. And now
        /// the component is readonly, so no worries about any system accidentally mutating it.
        /// If the system does end up wanting to mutate it, it will call UnpackMutate instead.
        /// And now we're aware of every time a component is mutated, so we can automatically copy
        /// the previous state of that component before returning from UnpackMutate. Yay!
        /// We don't need this ugly deep copy now.
        /// TODO: Delete this lol
        /// </summary>
    }
}
