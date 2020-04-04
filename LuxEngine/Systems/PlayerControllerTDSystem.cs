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

    public class Moveable : BaseComponent<Moveable>
    {
        public float MaxSpeedX;
        public float MaxSpeedY;
        public Vector2 Velocity;
        public Vector2 Direction;

        public Moveable(float maxSpeedX, float maxSpeedY)
        {
            MaxSpeedX = maxSpeedX;
            MaxSpeedY = maxSpeedY;
            Velocity = Vector2.Zero;
            Direction = Vector2.Zero;
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
            signature.Require<Moveable>();
            signature.RequireSingleton<InputSingleton>();
        }

        protected override void Update(GameTime gameTime)
        {
            var input = World.UnpackSingleton<InputSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var transform = World.Unpack<Transform>(entity);
                var moveable = World.Unpack<Moveable>(entity);
                float time = 1; //(float)gameTime.ElapsedGameTime.TotalMilliseconds / 10f;

                moveable.Velocity = Vector2.Zero;

                if (input.Up)
                {
                    moveable.Velocity.Y = -1;
                    moveable.Direction = new Vector2(0, -1);
                    transform.Y -= (int)(moveable.MaxSpeedY * time);
                }
                else if (input.Down)
                {
                    moveable.Velocity.Y = 1;
                    moveable.Direction = new Vector2(0, 1);
                    transform.Y += (int)(moveable.MaxSpeedY * time);
                }

                if (input.Right)
                {
                    moveable.Velocity.X = 1;
                    moveable.Direction = new Vector2(1, 0);
                    transform.X += (int)(moveable.MaxSpeedX * time);
                }
                else if (input.Left)
                {
                    moveable.Velocity.X = -1;
                    moveable.Direction = new Vector2(-1, 0);
                    transform.X -= (int)(moveable.MaxSpeedX * time);
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

        protected override void PrePreDraw(GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<Sprite>(entity);
                var moveable = World.Unpack<Moveable>(entity);

                // Walking animations

                if (moveable.Velocity.X == 0)
                {
                    if (moveable.Velocity.Y < 0)
                    {
                        SetAnimation(sprite, "WalkUp");
                    }
                    else if (moveable.Velocity.Y > 0)
                    {
                        SetAnimation(sprite, "WalkDown");
                    }
                }
                else if (moveable.Velocity.X > 0)
                {
                    SetAnimation(sprite, "WalkRight");
                }
                else if (moveable.Velocity.X < 0)
                {
                    SetAnimation(sprite, "WalkLeft");
                }

                // Idle animations
                if (moveable.Velocity.X == 0 && moveable.Velocity.Y == 0)
                {
                    if (moveable.Direction.X == 0)
                    {
                        if (moveable.Direction.Y < 0)
                        {
                            SetAnimation(sprite, "IdleUp");
                        }
                        else if (moveable.Direction.Y > 0)
                        {
                            SetAnimation(sprite, "IdleDown");
                        }
                    }
                    else if (moveable.Direction.X > 0)
                    {
                        SetAnimation(sprite, "IdleRight");
                    }
                    else if (moveable.Direction.X < 0)
                    {
                        SetAnimation(sprite, "IdleLeft");
                    }
                }
            }
        }
    }
}
