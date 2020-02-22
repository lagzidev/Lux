using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    static class Global
    {
        public static LuxEngine Game;
        public static Random Random = new Random();
        public static string LevelName;

        public static void Initialize(LuxEngine inputGame)
        {
            Game = inputGame;
        }
    }
}
