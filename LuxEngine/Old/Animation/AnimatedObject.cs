using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class AnimatedObject : GameObject
    {
        protected int currentAnimationFrame;
        protected float animationTimer;
        protected int currentAnimationX = -1;
        protected int currentAnimationY = -1;
        protected AnimationSet animationSet;
        protected AnimationL currentAnimation;

        protected bool flipRightFrames = false;
        protected bool flipLeftFrames = false;
        protected bool flipUpFrames = false;
        protected bool flipDownFrames = false;

        protected SpriteEffects spriteEffect = SpriteEffects.None;

        protected enum AnimationType
        {
            IdleLeft,
            IdleRight,
            IdleUp,
            IdleDown,
            WalkLeft,
            WalkRight,
            WalkUp,
            WalkDown,
        }

        protected void LoadAnimation(string path, ContentManager content)
        {
            AnimationData animationData = AnimationLoader.Load(path);
            animationSet = animationData.Animation;

            // Set up initial values
            center.X = animationSet.Width / 2;
            center.Y = animationSet.Height / 2;

            if (animationSet.AnimationList.Count != 0)
            {
                currentAnimation = animationSet.AnimationList[0];
                currentAnimationFrame = 0;
                animationTimer = 0f;
                CalculateFramePosition();
            }
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            base.Update(objects, map);
            if (currentAnimation != null)
            {
                UpdateAnimations();
            }
        }

        protected virtual void UpdateAnimations()
        {
            if (currentAnimation.AnimationOrder.Count == 0)
            {
                return;
            }

            animationTimer -= 1;

            // When timer has finished
            if (animationTimer <= 0)
            {
                animationTimer = currentAnimation.Speed;

                if (AnimationComplete())
                {
                    currentAnimationFrame = 0;
                }
                else
                {
                    currentAnimationFrame++;
                }

                CalculateFramePosition();
            }
        }

        private AnimationL GetAnimation(AnimationType animationType)
        {
            string name = GetAnimationName(animationType);

            for (int i = 0; i < animationSet.AnimationList.Count; i++)
            {
                if (animationSet.AnimationList[i].Name == name)
                {
                    return animationSet.AnimationList[i];
                }
            }

            return null;
        }

        protected virtual void ChangeAnimation(AnimationType newAnimationType)
        {
            if (currentAnimation.Name == GetAnimationName(newAnimationType))
            {
                return;
            }

            currentAnimation = GetAnimation(newAnimationType);

            if (currentAnimation == null)
            {
                // TODO: Log this as an error
                return;
            }

            currentAnimationFrame = 0;
            animationTimer = currentAnimation.Speed;

            CalculateFramePosition();

            if (flipRightFrames && currentAnimation.Name.Contains("Right") ||
                flipLeftFrames && currentAnimation.Name.Contains("Left"))
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }

            if (flipUpFrames && currentAnimation.Name.Contains("Up") ||
                flipDownFrames && currentAnimation.Name.Contains("Down"))
            {
                spriteEffect = SpriteEffects.FlipVertically;
            }
        }

        protected void CalculateFramePosition()
        {
            int coordinate = currentAnimation.AnimationOrder[currentAnimationFrame];
            currentAnimationX = (coordinate % animationSet.GridX) * animationSet.Width;
            currentAnimationY = (coordinate / animationSet.GridX) * animationSet.Height;
        }

        public bool AnimationComplete()
        {
            return currentAnimationFrame >= currentAnimation.AnimationOrder.Count - 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            if (currentAnimationX == -1 || currentAnimationY == -1)
            {
                base.Draw(spriteBatch);
                return;
            }

            spriteBatch.Draw(
                image,
                Position,
                new Rectangle(currentAnimationX, currentAnimationY, animationSet.Width, animationSet.Height),
                DrawColor,
                Rotation,
                Vector2.Zero,
                Scale,
                spriteEffect,
                LayerDepth);
        }

        protected string GetAnimationName(AnimationType animation)
        {
            return animation.ToString();
        }

        protected bool AnimationIsNot(AnimationType animation)
        {
            return currentAnimation != null && GetAnimationName(animation) != currentAnimation.Name;
        }

        public AnimatedObject()
        {
        }
    }
}
