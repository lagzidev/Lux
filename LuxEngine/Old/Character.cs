//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace LuxEngine
//{
//    public enum MoveDirection
//    {
//        Up,
//        Down,
//        Left,
//        Right
//    }

//    public enum MoveSpeed
//    {
//        Walk = 4,
//        Run = 10,
//    }

//    public class Character : AnimatedObject
//    {
//        protected enum Axis
//        {
//            X, Y
//        }

//        public Vector2 Velocity;
//        MoveSpeed currentSpeed;

//        protected float decel = 1.2f;
//        protected float accel = .78f;

//        public override void Initialize()
//        {
//            Velocity = Vector2.Zero;
//            base.Initialize();
//        }

//        public override void Update(List<GameObject> objects, Map map)
//        {
//            UpdateMovement(objects, map);
//            base.Update(objects, map);
//        }

//        private void UpdateMovement(List<GameObject> objects, Map map)
//        {
//            Rectangle collision_x = CheckCollisions(map, objects, Axis.X);

//            // X Axis collision
//            if (collision_x != Rectangle.Empty)
//            {
//                // Moving right
//                if (Velocity.X > 0)
//                {
//                    Position.X = collision_x.Left - boundingBoxWidth;
//                }

//                // Moving left
//                if (Velocity.X < 0)
//                {
//                    Position.X = collision_x.Right;
//                }

//                Velocity.X = 0;
//            }

//            Rectangle collision_y = CheckCollisions(map, objects, Axis.Y);

//            // Y Axis collision
//            if (Velocity.Y != 0 && collision_y != Rectangle.Empty)
//            {
//                // Moving down
//                if (Velocity.Y > 0)
//                {
//                    Position.Y = collision_y.Top - boundingBoxHeight;
//                }

//                // Moving up
//                if (Velocity.Y < 0)
//                {
//                    Position.Y = collision_y.Bottom;
//                }

//                Velocity.Y = 0;
//            }

//            // Move
//            Position.Y += Velocity.Y;
//            Position.X += Velocity.X;

//            // Deaccelerate
//            Velocity.X = TendToZero(Velocity.X, decel);
//            Velocity.Y = TendToZero(Velocity.Y, decel);
//        }

//        protected override void UpdateAnimations()
//        {
//            if (currentAnimation == null)
//            {
//                return;
//            }

//            base.UpdateAnimations();

//            // Standing still
//            if (Velocity == Vector2.Zero)
//            {
//                if (Velocity.X == 0)
//                {
//                    // Facing right
//                    if (direction.X > 0)
//                    {
//                        ChangeAnimation(AnimationType.IdleRight);
//                    }

//                    // Facing left
//                    if (direction.X < 0)
//                    {
//                        ChangeAnimation(AnimationType.IdleLeft);
//                    }
//                }

//                if (Velocity.Y == 0)
//                {
//                    // Facing down
//                    if (direction.Y > 0)
//                    {
//                        ChangeAnimation(AnimationType.IdleDown);
//                    }

//                    // Facing up
//                    if (direction.Y < 0)
//                    {
//                        ChangeAnimation(AnimationType.IdleUp);
//                    }
//                }
//            }
//            else
//            {
//                // Moving
//                if (Velocity.X != 0)
//                {
//                    // Walking right
//                    if (direction.X > 0)
//                    {
//                        ChangeAnimation(AnimationType.WalkRight);
//                    }

//                    // Walking left
//                    if (direction.X < 0)
//                    {
//                        ChangeAnimation(AnimationType.WalkLeft);
//                    }
//                }

//                if (Velocity.Y != 0)
//                {
//                    // Walking down
//                    if (direction.Y > 0)
//                    {
//                        ChangeAnimation(AnimationType.WalkDown);
//                    }

//                    // Walking up
//                    if (direction.Y < 0)
//                    {
//                        ChangeAnimation(AnimationType.WalkUp);
//                    }
//                }
//            }
//        }

//        public void Move(MoveDirection direction, MoveSpeed speed)
//        {
//            currentSpeed = speed;
//            switch (direction)
//            {
//                case MoveDirection.Up:
//                    MoveUp(speed);
//                    break;
//                case MoveDirection.Down:
//                    MoveDown(speed);
//                    break;
//                case MoveDirection.Left:
//                    MoveLeft(speed);
//                    break;
//                case MoveDirection.Right:
//                    MoveRight(speed);
//                    break;
//                default:
//                    // TODO: Log error
//                    break;
//            }
//        }

//        protected void MoveRight(MoveSpeed speed)
//        {
//            Velocity.X += accel + decel;

//            if (Velocity.X > (int)speed)
//            {
//                Velocity.X = (int)speed;
//            }

//            direction.X = 1;
//            direction.Y = 0;
//        }

//        protected void MoveLeft(MoveSpeed speed)
//        {
//            Velocity.X -= accel + decel;

//            if (Velocity.X < -(int)speed)
//            {
//                Velocity.X = -(int)speed;
//            }

//            direction.X = -1;
//            direction.Y = 0;
//        }

//        protected void MoveDown(MoveSpeed speed)
//        {
//            Velocity.Y += accel + decel;

//            if (Velocity.Y > (int)speed)
//            {
//                Velocity.Y = (int)speed;
//            }

//            direction.Y = 1;
//        }

//        protected void MoveUp(MoveSpeed speed)
//        {
//            Velocity.Y -= accel + decel;

//            if (Velocity.Y < -(int)speed)
//            {
//                Velocity.Y = -(int)speed;
//            }

//            direction.Y = -1;
//        }

//        protected virtual Rectangle CheckCollisions(Map map, List<GameObject> objects, Axis axis)
//        {
//            Rectangle futureBoundingBox = BoundingBox;

//            if (axis == Axis.X && Velocity.X != 0)
//            {
//                futureBoundingBox.X += (int)direction.X * (int)currentSpeed;
//            }
//            else if (axis == Axis.Y && Velocity.Y != 0)
//            {
//                futureBoundingBox.Y += (int)direction.Y * (int)currentSpeed;
//            }

//            Rectangle wallCollision = map.CheckCollision(futureBoundingBox);

//            // Wall collision detected
//            if (wallCollision != Rectangle.Empty)
//            {
//                return wallCollision;
//            }

//            // Check for object coliision
//            for (int i = 0; i < objects.Count; i++)
//            {
//                if (objects[i] != this && objects[i].Active && objects[i].Collidable && objects[i].CheckCollision(futureBoundingBox))
//                {
//                    return objects[i].BoundingBox;
//                }
//            }

//            return Rectangle.Empty;
//        }

//        protected float TendToZero(float val, float amount)
//        {
//            if (val > 0f && (val -= amount) < 0f) return 0f;
//            if (val < 0f && (val += amount) > 0f) return 0f;
//            return val;
//        }
//    }
//}
