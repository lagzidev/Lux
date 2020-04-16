using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public static class CalcUtils
    {
        /// <summary>
        /// Rounds a float to the nearest integer value.
        /// (e.g. 1.5f will round to 2)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int Round(float number)
        {
            return (int)Math.Round(number, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds a two coordinates into the nearest integer values and returns
        /// them in a vector.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>A vector of both rounded coordinates</returns>
        public static Vector2 Round(float x, float y)
        {
            return new Vector2(Round(x), Round(y));
        }
    }
}
