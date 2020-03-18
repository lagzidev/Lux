using System;
using System.Collections.Generic;
using System.Threading;

namespace LuxEngine
{
    public struct Entity : IComparable
    {
        public int Id;

        // To use Entity in a SortedDictionary
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Entity otherEntity = (Entity)obj;
            return Id.CompareTo(otherEntity.Id);
        }
    }
}
