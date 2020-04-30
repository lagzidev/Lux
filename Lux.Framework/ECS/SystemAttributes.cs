using System;

namespace Lux.Framework.ECS
{
    public abstract class ASystemAttribute : Attribute
    {
    }

    public class Optional : ASystemAttribute
    {
        public Guid ComponentTypeGuid;
        public int ComponentTypeId;

        public Optional(Type optionalComponentType)
        {
            ComponentTypeGuid = optionalComponentType.GUID;
        }
    }

    public class OnAddComponent : ASystemAttribute, IEntityFilter
    {
        private static readonly DefaultEntityFilter _defaultEntity = new DefaultEntityFilter();
        public Type AddedComponentType;

        public OnAddComponent(Type addedType)
        {
            AddedComponentType = addedType;
        }

        public bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, Guid[] componentTypes, params AInternalComponent[] components)
        {
            // Run the default filter first
            if (!_defaultEntity.Filter(world, entity, systemAttributes, componentTypes, components))
            {
                return false;
            }

            // Look for the OnAddComponent system attribute
            for (int i = 0; i < systemAttributes.Length; i++)
            {
                if (systemAttributes[i] is OnAddComponent onAddComponent)
                {
                    // If the added component type matches the one the system is
                    // looking for, let the system run
                    if (onAddComponent.AddedComponentType == AddedComponentType)
                    {
                        return true;
                    }
                }
            }

            // If system doesn't have OnAddComponent attribute, don't run it
            return false;
        }
    }
}
