using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable()]
    public class PlayerControlledTD : BaseComponent<PlayerControlledTD>
    {
        public PlayerControlledTD()
        {
        }
    }

    public class Speed : BaseComponent<Speed>
    {
        public float Value;

        public Speed(int speed)
        {
            Value = speed;
        }
    }

    public class PlayerControllerTDSystem : BaseSystem<PlayerControllerTDSystem>
    {
        //TODO: Components to implement:
        // PhysicsWorld, Grounded, GameInput (DynamicPhysics?) https://youtu.be/W3aieHjyNvw?t=156

        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<PlayerControlledTD>();
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.Require<Speed>();
            signature.RequireSingleton<InputSingleton>();
        }

        protected override void Update(GameTime gameTime)
        {
            var input = World.UnpackSingleton<InputSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var transform = World.Unpack<Transform>(entity);
                var speed = World.Unpack<Speed>(entity);

                if (input.Up)
                {
                    transform.Y -= speed.Value;
                }
                else if (input.Down)
                {
                    transform.Y += speed.Value;
                }

                if (input.Right)
                {
                    transform.X += speed.Value;
                }
                else if (input.Left)
                {
                    transform.X -= speed.Value;
                }
            }
        }

        private void SetAnimation(Sprite sprite, string animationName)
        {
            if (!sprite.SpriteData.Animations.ContainsKey(animationName))
            {
                animationName = sprite.SpriteData.Animations.Keys.First();
                LuxCommon.Assert(false);
            }

            // If animation has changed, reset the current frame and set the name
            if (animationName != sprite.CurrentAnimationName)
            {
                sprite.CurrentAnimationName = animationName;
                sprite.CurrentAnimationFrame = 0;
            }
        }

        protected override void PreDraw(GameTime gameTime)
        {
            var input = World.UnpackSingleton<InputSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<Sprite>(entity);
                bool moving = false;

                if (input.Up)
                {
                    SetAnimation(sprite, "WalkUp");
                    moving = true;
                }
                else if (input.Down)
                {
                    SetAnimation(sprite, "WalkDown");
                    moving = true;
                }

                if (input.Right)
                {
                    SetAnimation(sprite, "WalkRight");
                    moving = true;
                }
                else if (input.Left)
                {
                    SetAnimation(sprite, "WalkLeft");
                    moving = true;
                }

                if (!moving)
                {
                    switch (sprite.CurrentAnimationName)
                    {
                        case "WalkUp":
                            SetAnimation(sprite, "IdleUp");
                            break;

                        case "WalkDown":
                            SetAnimation(sprite, "IdleDown");
                            break;

                        case "WalkRight":
                            SetAnimation(sprite, "IdleRight");
                            break;

                        case "WalkLeft":
                            SetAnimation(sprite, "IdleLeft");
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
