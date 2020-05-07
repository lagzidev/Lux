﻿using System;

namespace Lux.Framework.ECS
{
    public abstract class ASystemAttribute : Attribute
    {
    }

    /// <summary>
    /// Use this for components that are Unpacked rather than required as
    /// a parameter.
    /// </summary>
    public class Using : ASystemAttribute
    {
        public Type[] ComponentTypes;

        public Using(params Type[] componentTypes)
        {
            ComponentTypes = componentTypes;
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
