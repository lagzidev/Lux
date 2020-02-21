using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    static class Global
    {
        public static LuxGame game;
        public static Random random = new Random();
        public static string levelName;

        public static void Initialize(LuxGame inputGame)
        {
            game = inputGame;
        }
    }
}
