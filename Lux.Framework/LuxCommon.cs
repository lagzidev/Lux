﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace Lux.Framework
{
    public static class LuxCommon
    {
#if DEBUG
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

        [Conditional("DEBUG")]
        public static void Assert(bool statement)
        {
            if (!statement)
            {
                Debugger.Break();
            }
        }

        public static int Min(int number, params int[] otherNumbers)
        {
            int min = number;
            for (int i = 0; i < otherNumbers.Length; i++)
            {
                if (otherNumbers[i] < min)
                {
                    min = otherNumbers[i];
                }
            }

            return min;
        }
    }

    //public enum LuxStatusCode
    //{
    //    SUCCESS = 0,
    //}

    //public struct LuxStatus
    //{
    //    /// <summary>
    //    /// This should be used for success instead of declaring a new struct
    //    /// for better readability and safety.
    //    /// </summary>
    //    public static LuxStatus SUCCESS = new LuxStatus(LuxStatusCode.SUCCESS, 0);

    //    public readonly LuxStatusCode Status;
    //    public readonly int ExtraInfo;

    //    public LuxStatus(LuxStatusCode status, int extraInfo)
    //    {
    //        Status = status;
    //        ExtraInfo = extraInfo;
    //    }

    //    /// <summary>
    //    /// Determines whether a <see cref="LuxStatus"/> equals another.
    //    /// </summary>
    //    /// <param name="luxStatus">This <see cref="LuxStatus"/> to compare</param>
    //    /// <param name="other">The other <see cref="LuxStatus"/> to compare</param>
    //    /// <returns></returns>
    //    public static bool operator ==(LuxStatus luxStatus, LuxStatus other)
    //    {
    //        return luxStatus.Status == other.Status;
    //    }

    //    public static bool operator !=(LuxStatus luxStatus, LuxStatus other)
    //    {
    //        return !(luxStatus == other);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (!(obj is LuxStatus item))
    //        {
    //            return false;
    //        }

    //        return Status.Equals(item.Status);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return Status.GetHashCode();
    //    }

    //    /// <summary>
    //    /// Determines whether the <see cref="LuxStatus"/> is successful.
    //    /// </summary>
    //    /// <param name="luxStatus">The <see cref="LuxStatus"/> to check</param>
    //    /// <returns><c>true</c> if the status indicates success; <c>false</c> otherwise.</returns>
    //    public static implicit operator bool(LuxStatus luxStatus)
    //    {
    //        return luxStatus.Status == LuxStatusCode.SUCCESS;
    //    }
    //}
}
