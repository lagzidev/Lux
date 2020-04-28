using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine.ECS
{
    public class AnimationFrame
    {
        public int Width;
        public int Height;
        public int TexturePositionX;
        public int TexturePositionY;
        public Color Color;
        public float Rotation;
        public Vector2 Scale;
        public SpriteEffects SpriteEffects;
        public SpriteDepth SpriteDepth;
        public int Duration;
    }

    /// <summary>
    /// A collection of animation frames
    /// </summary>
    [Serializable]
    public class Animation
    {
        public List<AnimationFrame> Frames;
    }

    public static class AnimationSystem
    {
        public static void UpdateAnimation(Sprite sprite)
        {
            Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
            AnimationFrame currentFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

            // If frame still has time to stay, do nothing
            if (sprite.CurrentTimeInFrameMs < currentFrame.Duration)
            {
                sprite.CurrentTimeInFrameMs += Time.DeltaTime * 1000f;
                return;
            }

            sprite.CurrentTimeInFrameMs = 0;

            if (sprite.CurrentAnimationFrame == currentAnimation.Frames.Count - 1)
            {
                sprite.CurrentAnimationFrame = 0;
            }
            else
            {
                sprite.CurrentAnimationFrame++;
            }
        }
    }
}
