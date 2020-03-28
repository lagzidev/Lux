using System;
using System.Runtime.InteropServices;

namespace LuxEngine
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Entity : IComparable
    {
        [FieldOffset(0)] public Int32 Id;
        [FieldOffset(0)] public Int16 Index;
        [FieldOffset(2)] public Int16 Generation;

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
