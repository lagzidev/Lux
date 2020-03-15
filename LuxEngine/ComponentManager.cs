using System;

namespace LuxEngine
{
    public struct ComponentInstance
    {
        public int Index;

        public ComponentInstance(int index)
        {
            Index = index;
        }
    }

    public class ComponentList<T>
    {
        public int Size { get; private set; }
        private BaseComponent<T>[] components;

        public ComponentList()
        {
            Size = 0;
            components = new BaseComponent<T>[HardCodedConfig.MAX_COMPONENTS_PER_TYPE];
        }

        public BaseComponent<T> this[int i]
        {
            get { return components[i]; }
        }

        public ComponentInstance Add(BaseComponent<T> component)
        {
            ComponentInstance newInstance;
            newInstance.Index = Size;

            // Add to list
            components[newInstance.Index] = component;
            Size++;

            return newInstance;
        }

        /// <summary>
        /// Removes a component from the list by moving the last component
        /// in the list to the index of the removed component.
        /// </summary>
        /// <param name="index">Index of the component to remove</param>
        /// <returns>The last component's new instance (and thus new position)</returns>
        public ComponentInstance Remove(int index)
        {
            // Replace the removed component with the last component in the list
            // to avoid fragmentation.
            components[index] = components[Size - 1];
            Size--;

            ComponentInstance lastComponent;
            lastComponent.Index = Size;

            return lastComponent;
        }
    }

    // This exists so we can create an array of component managers without
    // specifying the type <T>
    public class BaseComponentManager
    {
    }

    public class ComponentManager<T> : BaseComponentManager
    {
        private ComponentList<T> componentList;
        private EntityMap entityMap;

        public ComponentManager()
        {
            componentList = new ComponentList<T>();
            entityMap = new EntityMap();
        }

        /// <summary>
        /// We wrap <T> with BaseComponent so that the user could pass only
        /// the component type (e.g. TransformComponent) and we would still be
        /// able to access BaseComponent's properties (e.g. Entity)
        /// </summary>
        public ComponentInstance AddComponent(Entity entity, BaseComponent<T> component)
        {
            ComponentInstance newInstance = componentList.Add(component);
            entityMap.Add(entity, newInstance);

            return newInstance;
        }

        public void RemoveComponent(Entity entityToRemove)
        {
            // ComponentList.Remove(..):
            //     v-- componentToRemove
            // [1, 2, 3, 4]
            // [1, ?, 3, 4]
            // [1, 4, 3, ?]
            //           ^-- lastComponent

            // Remove component from the component list
            ComponentInstance componentToRemove = entityMap.GetComponentInstance(entityToRemove);
            ComponentInstance lastComponent = componentList.Remove(componentToRemove.Index);

            // Update the entity map because the last component was moved
            // to [componentToRemove.Index] by componentList.Remove
            // Move the entity that corresponded with the last component
            // to the component's new location
            Entity lastEntity = entityMap.GetEntity(lastComponent);
            entityMap.Update(lastEntity, componentToRemove);

            // Remove component from the entity map
            entityMap.Remove(entityToRemove);
        }

        public BaseComponent<T> GetComponent(Entity entity)
        {
            ComponentInstance componentInstance = entityMap.GetComponentInstance(entity);
            return componentList[componentInstance.Index];
        }
    }
}
