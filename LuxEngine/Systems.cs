using System;
using System.Collections;
using System.Collections.Generic;

namespace LuxEngine
{
    public class LuxIterator<T> : IEnumerable<T>
    {
        private T[] _elements;
        private List<T> _tempElements;

        public bool IsIterating { get; protected set; }
        public bool Paused { get; set; }
        public Queue<Action> PostIterationCallbacks;

        /// <summary>
        /// </summary>
        /// <param name="elements">Elements to iterate over</param>
        /// <param name="postIterationCallbacks">
        /// A callback function that is called after finished iterating
        /// over all of the elements.
        /// </param>
        public LuxIterator(Queue<Action> postIterationCallbacks)
        {
            _elements = null;
            _tempElements = new List<T>();

            IsIterating = false;
            Paused = false;
            PostIterationCallbacks = postIterationCallbacks;
        }

        public void Add(T element)
        {
            // If elements are already initialized you can't add more elements
            if (_tempElements == null)
            {
                LuxCommon.Assert(false);
                return;
            }

            _tempElements.Add(element);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_elements == null)
            {
                _elements = _tempElements.ToArray();
                _tempElements = null;
            }

            IsIterating = true;

            if (!Paused)
            {
                var i = 0;
                while (i < _elements.Length)
                {
                    yield return _elements[i];
                    i++;
                }
            }

            IsIterating = false;

            // Invoke the callbacks
            while (PostIterationCallbacks.Count > 0)
            {
                PostIterationCallbacks.Dequeue().Invoke();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
