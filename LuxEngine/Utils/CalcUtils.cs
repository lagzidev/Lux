using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public static class CalcUtils
    {
        /// <summary>
        /// Rounds a float to the biggest integer value.
        /// (e.g. 1.5f will round to 2)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int Floor(float number)
        {
            return (int)Math.Floor(number);
            //return (int)Math.Floor(number);
        }

        /// <summary>
        /// Rounds a two coordinates into the biggest integer values and returns
        /// them in a vector.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>A vector of both rounded coordinates</returns>
        public static Vector2 Floor(float x, float y)
        {
            return new Vector2(Floor(x), Floor(y));
        }

        /// <summary>
        /// Rounds a float to the nearest integer value.
        /// (e.g. 1.5f will round to 2)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int Round(float number)
        {
            return (int)Math.Round(number, MidpointRounding.AwayFromZero);
            //return (int)Math.Floor(number);
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

        /// <summary>
        /// Rounds a float to the biggest integer value.
        /// (e.g. 1.5f will round to 2)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int Ceiling(float number)
        {
            return (int)Math.Ceiling(number);
            //return (int)Math.Floor(number);
        }

        /// <summary>
        /// Rounds a two coordinates into the biggest integer values and returns
        /// them in a vector.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>A vector of both rounded coordinates</returns>
        public static Vector2 Ceiling(float x, float y)
        {
            return new Vector2(Ceiling(x), Ceiling(y));
        }
    }
}
