using System;
using System.Collections;
using System.Collections.Generic;

namespace LuxEngine
{
    /// <summary>
    /// Represents an unordered sparse set of natural numbers, and provides constant-time operations on it.
    /// </summary>
    public sealed class SparseSet : IEnumerable<int>
    {
        /// <summary>
        /// Contains the actual data packed tightly in memory.
        /// </summary>
        private readonly int[] _denseArr;

        /// <summary>
        /// Contains a list of indexes to valid values in the dense array.
        /// The expression _denseArr[_sparseArr[x]] == x is true for every x in the current range.
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
            _denseArr = new int[MaxSize];
            _sparseArr = new int[MaxSize];
        }

        /// <summary>
        /// Adds the given value.
        /// If the value already exists in the set it will be ignored.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="outStatus">
        /// <see cref="LuxStatusCode.SPARSESET_ADD_VALUE_OUT_OF_RANGE"/>
        /// <see cref="LuxStatusCode.SPARSESET_ADD_VALUE_ALREADY_EXISTS"/>
        /// <see cref="SparseSet.Contains"/>
        /// </param>
        /// <returns>
        ///   A <see cref="LuxStatus"/> indicating the function's success/failure.
        /// </returns>
        public void Add(int value, out LuxStatus outStatus)
        {
            if (value < 0 || value >= MaxSize)
            {
                outStatus = new LuxStatus(LuxStatusCode.SPARSESET_ADD_VALUE_OUT_OF_RANGE, value);
                return;
            }

            if (Contains(value, out LuxStatus status))
            {
                outStatus = new LuxStatus(LuxStatusCode.SPARSESET_ADD_VALUE_ALREADY_EXISTS, value);
                return;
            }

            if (!status)
            {
                outStatus = status;
                return;
            }

            _denseArr[Count] = value;     // insert new value in the dense array...
            _sparseArr[value] = Count;     // ...and link it to the sparse array
            Count++;

            outStatus = LuxStatus.SUCCESS;
        }

        /// <summary>
        /// Removes the given value in case it exists.
        /// </summary>
        /// <param name="value">The value to remove</param>
        /// <param name="outStatus">
        /// Possible status codes:
        /// <see cref="LuxStatusCode.SPARSESET_REMOVE_VALUE_DOESNT_EXIST"/>
        /// <see cref="SparseSet.Contains"/>
        /// </param>
        public void Remove(int value, out LuxStatus outStatus)
        {
            if (!Contains(value, out LuxStatus status))
            {
                outStatus = new LuxStatus(LuxStatusCode.SPARSESET_REMOVE_VALUE_DOESNT_EXIST, value);
                return;
            }

            if (!status)
            {
                outStatus = status;
                return;
            }

            // put the value at the end of the dense array into the slot of the removed value
            _denseArr[_sparseArr[value]] = _denseArr[Count - 1];

            // put the link to the removed value in the slot of the replaced value
            _sparseArr[_denseArr[Count - 1]] = _sparseArr[value];

            Count--;

            outStatus = LuxStatus.SUCCESS;
        }

        /// <summary>
        /// Determines whether the set contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outStatus">
        /// <see cref="LuxStatusCode.SPARSESET_CONTAINS_VALUE_OUT_OF_RANGE"/>
        /// </param>
        /// <returns><c>true</c> if the set contains the given value; <c>false</c> otherwise.</returns>
        public bool Contains(int value, out LuxStatus outStatus)
        {
            if (value >= MaxSize || value < 0)
            {
                outStatus = new LuxStatus(LuxStatusCode.SPARSESET_CONTAINS_VALUE_OUT_OF_RANGE, value);
                return false;
            }

            outStatus = LuxStatus.SUCCESS;

            // Linked value from the sparse array must point to a value within the currently used range of the dense array
            bool sparseValueInDenseArrayRange = _sparseArr[value] < Count;

            // There's a valid two-way link between the sparse array and the dense array.
            bool validTwoWayLink = _denseArr[_sparseArr[value]] == value;

            return sparseValueInDenseArrayRange && validTwoWayLink;
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
        public IEnumerator<int> GetEnumerator()
        {
            var i = 0;
            while (i < Count)
            {
                yield return _denseArr[i];
                i++;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all elements in the set.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
