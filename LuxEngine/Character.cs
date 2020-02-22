using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class Character : GameObject
    {
        protected enum Axis
        {
            X, Y
        }

        public Vector2 velocity;

        protected float decel = 1.2f;
        protected float accel = .78f;
        protected float maxSpeed = 5f;

        const float gravity = 1f;
        const float jumpVelocity = 16f;
        const float maxFallVelocity = 32f;

        protected bool jumping;

        public override void Initialize()
        {
            velocity = Vector2.Zero;
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
            if (velocity.X != 0 && CheckCollisions(map, objects, Axis.X))
            {
                velocity.X = 0;
            }

            position.X += velocity.X;

            // Y Axis
            if (velocity.Y != 0 && CheckCollisions(map, objects, Axis.Y))
            {
                velocity.Y = 0;
            }

            position.Y += velocity.Y;

            // Deaccelerate
            velocity.X = TendToZero(velocity.X, decel);
            velocity.Y = TendToZero(velocity.Y, decel);
        }

        private void ApplyGravity(Map map)
        {
            if (jumping || OnGround(map) == Rectangle.Empty)
            {
                velocity.Y += gravity;
            }

            if (velocity.Y > maxFallVelocity)
            {
                velocity.Y = maxFallVelocity;
            }
        }

        protected void MoveRight()
        {
            velocity.X += accel + decel;

            if (velocity.X > maxSpeed)
            {
                velocity.X = maxSpeed;
            }

            direction.X = 1;
        }

        protected void MoveLeft()
        {
            velocity.X -= accel + decel;

            if (velocity.X < -maxSpeed)
            {
                velocity.X = -maxSpeed;
            }

            direction.X = -1;
        }

        protected void MoveDown()
        {
            velocity.Y += accel + decel;

            if (velocity.Y > maxSpeed)
            {
                velocity.Y = maxSpeed;
            }

            direction.Y = 1;
        }

        protected void MoveUp()
        {
            velocity.Y -= accel + decel;

            if (velocity.Y < -maxSpeed)
            {
                velocity.Y = -maxSpeed;
            }

            direction.Y = -1;
        }

        protected bool Jump(Map map)
        {
            if (jumping)
            {
                return false;
            }

            // Are we on ground
            if (0 == velocity.Y && OnGround(map) != Rectangle.Empty)
            {
                velocity.Y -= jumpVelocity;
                jumping = true;
            }

            return false;
        }

        protected virtual bool CheckCollisions(Map map, List<GameObject> objects, Axis axis)
        {
            Rectangle futureBoundingBox = BoundingBox;

            int maxX = (int)maxSpeed;
            int maxY = (int)maxSpeed;

            if (axis == Axis.X)
            {
                if (velocity.X > 0)
                {
                    futureBoundingBox.X += maxX;
                }

                if (velocity.X < 0)
                {
                    futureBoundingBox.X -= maxX;
                }
            }
            else if (axis == Axis.Y)
            {
                if (velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                }

                if (velocity.Y < 0)
                {
                    futureBoundingBox.Y -= maxY;
                }
            }

            Rectangle wallCollision = map.CheckCollision(futureBoundingBox);

            // Wall collision detected
            if (wallCollision != Rectangle.Empty)
            {
                return true;
            }

            // Check for object coliision
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != this && objects[i].active && objects[i].collidable && objects[i].CheckCollision(futureBoundingBox))
                {
                    return true;
                }
            }

            return false;
        }

        public void LandResponse(Rectangle wallCollision)
        {
            position.Y = wallCollision.Top - (boundingBoxHeight + boundingBoxOffset.Y);
            velocity.Y = 0;
            jumping = false;
        }

        protected float TendToZero(float val, float amount)
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }

        protected Rectangle OnGround(Map map)
        {
            Rectangle futureBoundingBox = new Rectangle(
                (int)(position.X + boundingBoxOffset.X),
                (int)(position.Y + boundingBoxOffset.Y + (velocity.Y + gravity)),
                boundingBoxWidth, boundingBoxHeight);

            return map.CheckCollision(futureBoundingBox);
        }
    }
}
