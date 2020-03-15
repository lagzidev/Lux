using System;
using System.Collections;

namespace LuxEngine
{

    /// <summary>
    /// For optimizing, see https://gamedev.stackexchange.com/questions/71767/how-can-i-efficiently-implement-a-bitmask-larger-than-64-bits-for-component-exis
    /// </summary>
    public class ComponentMask : ICloneable
    {
        private BitArray _mask;

        public ComponentMask(int[] componentTypes)
        {
            _mask = new BitArray(HardCodedConfig.MAX_COMPONENT_TYPES, false);

            foreach (var componentType in componentTypes)
            {
                _mask[componentType] = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">A component type (not wrapped in BaseComponent)</typeparam>
        public void AddComponent<T>()
        {
            _mask[(int)BaseComponent<T>.ComponentType] = true;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">A component type (not wrapped in BaseComponent)</typeparam>
        public void RemoveComponent<T>()
        {
            _mask[(int)BaseComponent<T>.ComponentType] = false;
        }

        public bool Matches(ComponentMask otherMask)
        {
            // [0, 1, 1, 1] _mask
            // [0, 1, 0, 1] otherMask
            // [0, 1, 0, 1] (_mask & otherMask) == otherMask
            return _mask.And(otherMask._mask) == otherMask._mask;
        }

        public object Clone()
        {
            var clone = (ComponentMask)MemberwiseClone();

            clone._mask = (BitArray)_mask.Clone();

            return clone;
        }
    }
}
