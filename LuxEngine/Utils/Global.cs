using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    static class Global
    {
        public static LuxEngine game;
        public static Random random = new Random();
        public static string levelName;

        public static void Initialize(LuxEngine inputGame)
        {
            game = inputGame;
        }
    }
}
