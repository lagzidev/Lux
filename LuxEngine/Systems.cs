using System;
using System.Collections;
using System.Collections.Generic;

namespace LuxEngine
{
    public class LuxIterator<T> : IEnumerable<T>
    {
        private T[] _elements;

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
        public LuxIterator(T[] elements, Queue<Action> postIterationCallbacks)
        {
            _elements = elements;
            IsIterating = false;
            Paused = false;
            PostIterationCallbacks = postIterationCallbacks;
        }

        public IEnumerator<T> GetEnumerator()
        {
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
