using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
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

    public class AnimationSystem : BaseSystem<AnimationSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Sprite>();
        }

        protected override void PreDraw(GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<Sprite>(entity);

                Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
                AnimationFrame currentFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

                // If frame still has time to stay, do nothing
                if (sprite.CurrentTimeInFrameMs < currentFrame.Duration)
                {
                    sprite.CurrentTimeInFrameMs += gameTime.ElapsedGameTime.Milliseconds;
                    continue;
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
}
