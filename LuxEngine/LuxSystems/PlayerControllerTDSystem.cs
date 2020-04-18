//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using LuxEngine.ECS;

//namespace LuxEngine
//{
//    [Serializable()]
//    public class PlayerControlledTD : AComponent<PlayerControlledTD>
//    {
//        public PlayerControlledTD()
//        {
//        }
//    }

//    public class Moveable : AComponent<Moveable>
//    {
//        public float MaxSpeedX;
//        public float MaxSpeedY;
//        public Vector2 Velocity;
//        public Vector2 Direction;

//        public Moveable(float maxSpeedX, float maxSpeedY)
//        {
//            MaxSpeedX = maxSpeedX;
//            MaxSpeedY = maxSpeedY;
//            Velocity = Vector2.Zero;
//            Direction = Vector2.Zero;
//        }
//    }

//    public class PlayerControllerTDSystem : ASystem<PlayerControllerTDSystem>
//    {
//        //TODO: Components to implement:
//        // PhysicsWorld, Grounded, GameInput (DynamicPhysics?) https://youtu.be/W3aieHjyNvw?t=156

//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<PlayerControlledTD>();
//            signature.Require<Transform>();
//            signature.Require<Sprite>();
//            signature.Require<Moveable>();
//            signature.RequireSingleton<InputSingleton>();
//        }

//        public override void Update()
//        {
//            UnpackSingleton(out InputSingleton input);

//            foreach (var entity in RegisteredEntities)
//            {
//                Unpack(entity, out Transform transform);
//                Unpack(entity, out Moveable moveable);

//                moveable.Velocity = Vector2.Zero;

//                if (input.Up)
//                {
//                    moveable.Velocity.Y = -1;
//                    moveable.Direction = new Vector2(0, -1);
//                    transform.Y -= 1f;//moveable.MaxSpeedY * Time.DeltaTime;
//                }
//                else if (input.Down)
//                {
//                    moveable.Velocity.Y = 1;
//                    moveable.Direction = new Vector2(0, 1);
//                    transform.Y += moveable.MaxSpeedY * Time.DeltaTime;
//                }

//                if (input.Right)
//                {
//                    moveable.Velocity.X = 1;
//                    moveable.Direction = new Vector2(1, 0);
//                    transform.X += moveable.MaxSpeedX * Time.DeltaTime;
//                }
//                else if (input.Left)
//                {
//                    moveable.Velocity.X = -1;
//                    moveable.Direction = new Vector2(-1, 0);
//                    transform.X -= moveable.MaxSpeedX * Time.DeltaTime;
//                }
//            }
//        }

//        private void SetAnimation(Sprite sprite, string animationName)
//        {
//            if (!sprite.SpriteData.Animations.ContainsKey(animationName))
//            {
//                animationName = sprite.SpriteData.Animations.Keys.First();
//                LuxCommon.Assert(false);
//            }

//            // If animation has changed, reset the current frame and set the name
//            if (animationName != sprite.CurrentAnimationName)
//            {
//                sprite.CurrentAnimationName = animationName;
//                sprite.CurrentAnimationFrame = 0;
//            }
//        }

//        public override void LoadDraw()
//        {
//            foreach (var entity in RegisteredEntities)
//            {
//                Unpack(entity, out Sprite sprite);
//                Unpack(entity, out Moveable moveable);

//                // Walking animations

//                if (moveable.Velocity.X == 0)
//                {
//                    if (moveable.Velocity.Y < 0)
//                    {
//                        SetAnimation(sprite, "WalkUp");
//                    }
//                    else if (moveable.Velocity.Y > 0)
//                    {
//                        SetAnimation(sprite, "WalkDown");
//                    }
//                }
//                else if (moveable.Velocity.X > 0)
//                {
//                    SetAnimation(sprite, "WalkRight");
//                }
//                else if (moveable.Velocity.X < 0)
//                {
//                    SetAnimation(sprite, "WalkLeft");
//                }

//                // Idle animations
//                if (moveable.Velocity.X == 0 && moveable.Velocity.Y == 0)
//                {
//                    if (moveable.Direction.X == 0)
//                    {
//                        if (moveable.Direction.Y < 0)
//                        {
//                            SetAnimation(sprite, "IdleUp");
//                        }
//                        else if (moveable.Direction.Y > 0)
//                        {
//                            SetAnimation(sprite, "IdleDown");
//                        }
//                    }
//                    else if (moveable.Direction.X > 0)
//                    {
//                        SetAnimation(sprite, "IdleRight");
//                    }
//                    else if (moveable.Direction.X < 0)
//                    {
//                        SetAnimation(sprite, "IdleLeft");
//                    }
//                }
//            }
//        }
//    }
//}
