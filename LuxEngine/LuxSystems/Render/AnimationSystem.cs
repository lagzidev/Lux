//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using LuxEngine.ECS;

//namespace LuxEngine.Systems
//{
//    public class AnimationFrame
//    {
//        public int Width;
//        public int Height;
//        public int TexturePositionX;
//        public int TexturePositionY;
//        public Color Color;
//        public float Rotation;
//        public Vector2 Scale;
//        public SpriteEffects SpriteEffects;
//        public SpriteDepth SpriteDepth;
//        public int Duration;
//    }

//    /// <summary>
//    /// A collection of animation frames
//    /// </summary>
//    [Serializable]
//    public class Animation
//    {
//        public List<AnimationFrame> Frames;
//    }

//    public class AnimationSystem : ASystem<AnimationSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Sprite>();
//        }

//        public override void PreDraw()
//        {
//            foreach (var entity in RegisteredEntities)
//            {
//                Unpack(entity, out Sprite sprite);

//                Animation currentAnimation = sprite.SpriteData.Animations[sprite.CurrentAnimationName];
//                AnimationFrame currentFrame = currentAnimation.Frames[sprite.CurrentAnimationFrame];

//                // If frame still has time to stay, do nothing
//                if (sprite.CurrentTimeInFrameMs < currentFrame.Duration)
//                {
//                    sprite.CurrentTimeInFrameMs += Time.DeltaTime * 1000f;
//                    continue;
//                }

//                sprite.CurrentTimeInFrameMs = 0;

//                if (sprite.CurrentAnimationFrame == currentAnimation.Frames.Count - 1)
//                {
//                    sprite.CurrentAnimationFrame = 0;
//                }
//                else
//                {
//                    sprite.CurrentAnimationFrame++;
//                }
//            }
//        }
//    }
//}
