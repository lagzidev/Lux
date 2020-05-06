using System;

namespace Lux.Framework.ECS
{
    public abstract class ASystemAttribute : Attribute
    {
    }

    /// <summary>
    /// Tells the system not to run if this component exists for the entity
    /// </summary>
    public class Exclude : ASystemAttribute
    {
        public Type ComponentType;

        public Exclude(Type excludedComponentType)
        {
            ComponentType = excludedComponentType;
        }
    }

    public class Optional : ASystemAttribute
    {
        public Type ComponentType;

        public Optional(Type optionalComponentType)
        {
            ComponentType = optionalComponentType;
        }
    }

    public class OnAddComponent : ASystemAttribute, ISystemFilter
    {
        public Type AddedComponentType;

        public OnAddComponent(Type addedType)
        {
            AddedComponentType = addedType;
        }

        public bool Filter(ASystem system)
        {
            for (int i = 0; i < system.SystemAttributes.Length; i++)
            {
                if (system.SystemAttributes[i] is OnAddComponent onAddComponent)
                {
                    if (onAddComponent.AddedComponentType == AddedComponentType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class OnDestroyEntity : ASystemAttribute, ISystemFilter
    {
        public bool Filter(ASystem system)
        {
            for (int i = 0; i < system.SystemAttributes.Length; i++)
            {
                if (system.SystemAttributes[i] is OnDestroyEntity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
