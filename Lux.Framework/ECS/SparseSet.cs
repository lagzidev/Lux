using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lux.Framework.ECS
{
    public interface ISparseSet
    {
    }

    public interface ISparseSetKey
    {
        Int16 Index { get; set; }
    }

    /// <summary>
    /// Represents an unordered sparse set of natural numbers, and provides constant-time operations on it.
    /// </summary>
    [Serializable]
    public sealed class SparseSet<T, K> : ISparseSet, IEnumerable<T> where K : ISparseSetKey, IEquatable<K>
    {
        /// <summary>
        /// Contains the actual data packed tightly in memory.
        /// Mirrors the keys array.
        /// </summary>
        private readonly T[] _valueArr;

        /// <summary>
        /// Contains keys, packed tightly in memory.
        /// Mirrors the value array.
        /// </summary>
        private readonly K[] _keyArr;
        public ReadOnlySpan<K> Keys
        {
            get
            {
                return _keyArr.AsSpan(0, Count);
            }
        }

        /// <summary>
        /// Contains a list of indexes to valid values in the keys array.
        /// The expression _keyArr[_sparseArr[x]] == x is true for every x in the current range.
        /// </summary>
        private readonly int[] _sparseArr;

        /// <summary>
        /// Maximum size of the set.
        /// </summary>
        public readonly int MaxSize;

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseSet"/> class.
        /// </summary>
        /// <param name="maxSize">The maximal size of the set.</param>
        public SparseSet(int maxSize)
        {
            MaxSize = maxSize;
            Count = 0;
            _valueArr = new T[MaxSize];
            _keyArr = new K[MaxSize];
            _sparseArr = new int[MaxSize];
        }

        /// <summary>
        /// Adds the given value.
        /// If the value already exists in the set it will be overriden.
        /// Fails silently if key is outside the valid range.
        /// </summary>
        /// <param name="key">The key of the corresponding value to add</param>
        /// <param name="outStatus">
        /// <see cref="SparseSet.Contains"/>
        /// </param>
        public void Add(K key, T value)
        {
            if (key.Index < 0 || key.Index >= MaxSize)
            {
                LuxCommon.Assert(false);
                return;
            }

            // Insert new value in the dense array
            _keyArr[Count] = key;
            _valueArr[Count] = value;

            // Link it to the sparse array
            _sparseArr[key.Index] = Count;
            Count++;
        }

        /// <summary>
        /// Removes a value from the set if exists, otherwise does nothing.
        /// </summary>
        /// <param name="key">The key that corresponds with the value to remove</param>
        /// </param>
        public void Remove(K key)
        {
            if (!Contains(key))
            {
                return;
            }

            // put the key and value from the end of their respective arrays
            // into the slot of the removed key
            _keyArr[_sparseArr[key.Index]] = _keyArr[Count - 1];
            _valueArr[_sparseArr[key.Index]] = _valueArr[Count - 1];

            // put the link to the removed key in the slot of the replaced value
            _sparseArr[_keyArr[Count - 1].Index] = _sparseArr[key.Index];

            Count--;
        }

        /// <summary>
        /// Determines whether the set has a value that corresponds to the given key.
        /// </summary>
        /// <param name="key">Key to check if exists</param>
        /// <returns>
        /// <c>true</c> if the set has a value that corresponds to the given key; 
        /// <c>false</c> otherwise.
        /// </returns>
        public bool Contains(K key)
        {
            if (key.Index >= MaxSize || key.Index < 0)
            {
                LuxCommon.Assert(false);
                return false;
            }

            // Linked key from the sparse array must point to a key within the currently used range of the keys array
            bool sparseValueInDenseArrayRange = _sparseArr[key.Index] < Count;

            // There's a valid two-way link between the sparse array and the keys array.
            bool validTwoWayLink = _keyArr[_sparseArr[key.Index]].Equals(key);

            return sparseValueInDenseArrayRange && validTwoWayLink;
        }

        /// <summary>
        /// Gets a value using the corresponding key.
        /// </summary>
        /// <param name="key">Key that corresponds to the value</param>
        /// <param name="outValue">Value to return</param>
        /// <returns><c>true</c> if successfully returned value; <c>false</c> otherwise.</returns>
        public bool GetValue(K key, out T outValue)
        {
            if (!Contains(key))
            {
                outValue = default;
                return false;
            }

            outValue = _valueArr[_sparseArr[key.Index]];
            return true;
        }

        /// <summary>
        /// Gets all of the values.
        /// </summary>
        /// <param name="key">Key that corresponds to the value</param>
        /// <param name="outValue">Value to return</param>
        /// <returns><c>true</c> if successfully returned value; <c>false</c> otherwise.</returns>
        public Span<T> GetAll()
        {
            return _valueArr.AsSpan(0, Count);
        }

        public ReadOnlySpan<T> GetAllReadonly()
        {
            return _valueArr.AsSpan(0, Count);
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all elements in the set.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var i = 0;
            while (i < Count)
            {
                yield return _valueArr[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
