using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum MoveSpeed
    {
        Walk = 4,
        Run = 10,

        Max // Always last
    }

    public class Character : AnimatedObject
    {
        protected enum Axis
        {
            X, Y
        }

        public Vector2 Velocity;
        MoveSpeed currentSpeed;

        protected float decel = 1.2f;
        protected float accel = .78f;

        const float gravity = 1f;
        const float jumpVelocity = 16f;
        const float maxFallVelocity = 32f;

        protected bool jumping;

        public override void Initialize()
        {
            Velocity = Vector2.Zero;
            jumping = false;
            base.Initialize();
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            UpdateMovement(objects, map);
            base.Update(objects, map);
        }

        private void UpdateMovement(List<GameObject> objects, Map map)
        {
            // X Axis
            if (Velocity.X != 0 && CheckCollisions(map, objects, Axis.X))
            {
                Velocity.X = 0;
            }

            Position.X += Velocity.X;

            // Y Axis
            if (Velocity.Y != 0 && CheckCollisions(map, objects, Axis.Y))
            {
                Velocity.Y = 0;
            }

            Position.Y += Velocity.Y;

            // Deaccelerate
            Velocity.X = TendToZero(Velocity.X, decel);
            Velocity.Y = TendToZero(Velocity.Y, decel);
        }

        public void Move(MoveDirection direction, MoveSpeed speed)
        {
            currentSpeed = speed;
            switch (direction)
            {
                case MoveDirection.Up:
                    MoveUp(speed);
                    break;
                case MoveDirection.Down:
                    MoveDown(speed);
                    break;
                case MoveDirection.Left:
                    MoveLeft(speed);
                    break;
                case MoveDirection.Right:
                    MoveRight(speed);
                    break;
                default:
                    // TODO: Log error
                    break;
            }
        }

        protected void MoveRight(MoveSpeed speed)
        {
            Velocity.X += accel + decel;

            if (Velocity.X > (int)speed)
            {
                Velocity.X = (int)speed;
            }

            direction.X = 1;
        }

        protected void MoveLeft(MoveSpeed speed)
        {
            Velocity.X -= accel + decel;

            if (Velocity.X < -(int)speed)
            {
                Velocity.X = -(int)speed;
            }

            direction.X = -1;
        }

        protected void MoveDown(MoveSpeed speed)
        {
            Velocity.Y += accel + decel;

            if (Velocity.Y > (int)speed)
            {
                Velocity.Y = (int)speed;
            }

            direction.Y = 1;
        }

        protected void MoveUp(MoveSpeed speed)
        {
            Velocity.Y -= accel + decel;

            if (Velocity.Y < -(int)speed)
            {
                Velocity.Y = -(int)speed;
            }

            direction.Y = -1;
        }

        protected virtual bool CheckCollisions(Map map, List<GameObject> objects, Axis axis)
        {
            Rectangle futureBoundingBox = BoundingBox;

            int maxX = (int)currentSpeed;
            int maxY = (int)currentSpeed;

            if (axis == Axis.X)
            {
                if (Velocity.X > 0)
                {
                    futureBoundingBox.X += maxX;
                }

                if (Velocity.X < 0)
                {
                    futureBoundingBox.X -= maxX;
                }
            }
            else if (axis == Axis.Y)
            {
                if (Velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                }

                if (Velocity.Y < 0)
                {
                    futureBoundingBox.Y -= maxY;
                }
            }

            Rectangle wallCollision = map.CheckCollision(futureBoundingBox);

            // Wall collision detected
            if (wallCollision != Rectangle.Empty)
            {
                // Moving right
                if (Velocity.X > 0 && wallCollision.Left > futureBoundingBox.Right)
                {
                    Position.X = wallCollision.Left;
                }
                else if (Velocity.X < 0 && wallCollision.Right > futureBoundingBox.Left) // Left
                {
                    Position.X = wallCollision.Right;
                }
                    
                // Moving down
                if (Velocity.Y > 0 && wallCollision.Top > futureBoundingBox.Bottom)
                {
                    Position.Y = wallCollision.Top;
                }
                else if (Velocity.Y < 0 && wallCollision.Bottom > futureBoundingBox.Top) // Up
                {
                    Position.Y = wallCollision.Bottom;
                }

                return true;
            }

            // Check for object coliision
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != this && objects[i].Active && objects[i].Collidable && objects[i].CheckCollision(futureBoundingBox))
                {
                    return true;
                }
            }

            return false;
        }

        protected float TendToZero(float val, float amount)
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }
    }
}
