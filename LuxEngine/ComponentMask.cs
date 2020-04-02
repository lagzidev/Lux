using System;
using System.Collections;

namespace LuxEngine
{

    /// <summary>
    /// For optimizing, see https://gamedev.stackexchange.com/questions/71767/how-can-i-efficiently-implement-a-bitmask-larger-than-64-bits-for-component-exis
    /// </summary>
    public class ComponentMask
    {
        private BitArray _mask;

        public ComponentMask(int[] componentTypes)
        {
            _mask = new BitArray(HardCodedConfig.MAX_GAME_COMPONENT_TYPES, false);

            foreach (var componentType in componentTypes)
            {
                _mask[componentType] = true;
            }
        }

        public ComponentMask()
        {
            _mask = new BitArray(HardCodedConfig.MAX_GAME_COMPONENT_TYPES, false);
        }

        /// <summary>
        /// Add a component to the mask
        /// </summary>
        /// <typeparam name="T">A component type (not wrapped in BaseComponent)</typeparam>
        public void AddComponent<T>() where T : BaseComponent<T>
        {
            _mask[BaseComponent<T>.ComponentType] = true;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">A component type (not wrapped in BaseComponent)</typeparam>
        public void RemoveComponent<T>() where T : BaseComponent<T>
        {
            _mask[BaseComponent<T>.ComponentType] = false;
        }

        public bool Contains(ComponentMask otherMask)
        {
            // [0, 1, 1, 1] _mask
            // [0, 1, 0, 1] otherMask
            // [0, 1, 0, 1] (_mask & otherMask) == otherMask

            // TODO: Implement BitArray yourself. This one sucks.

            // We have to save the mask because .And mutates it for some reason
            BitArray savedMask = (BitArray)_mask.Clone();
            BitArray andResult = _mask.And(otherMask._mask);
            _mask = savedMask;

            // Check if andResult and otherMask are equal (for lack of .Equals)
            for (int i = 0; i < andResult.Length; i++)
            {
                if (andResult[i] != otherMask._mask[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
