using System;
namespace LuxEngine
{
    public static class DrawUtils
    {
        public static float CalculateSpriteDepth(SpriteDepth spriteDepth)
        {
            return (float)spriteDepth / 10f;
        }
    }
}
