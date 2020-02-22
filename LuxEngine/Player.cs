using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class Player : Character
    {
        public Player()
        {
        }

        public Player(Vector2 inputPosition)
        {
            Position = inputPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            image = TextureLoader.Load("spritesheet", content);

            LoadAnimation("Anim1.xml", content);
            ChangeAnimation(AnimationType.WalkRight);

            base.Load(content);

            boundingBoxOffset.X = 0;
            boundingBoxOffset.Y = 0;
            boundingBoxWidth = animationSet.Width;
            boundingBoxHeight = animationSet.Height;
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            CheckInput(objects, map);
            base.Update(objects, map);
        }

        private void CheckInput(List<GameObject> objects, Map map)
        {
            MoveSpeed speed = Input.IsKeyDown(Keys.LeftShift) ? MoveSpeed.Run : MoveSpeed.Walk;

            if (Input.IsKeyDown(Keys.Right))
            {
                new MoveCommand(this, MoveDirection.Right, speed).Execute();
            }
            else if (Input.IsKeyDown(Keys.Left))
            {
                new MoveCommand(this, MoveDirection.Left, speed).Execute();
            }

            if (Input.IsKeyDown(Keys.Down))
            {
                new MoveCommand(this, MoveDirection.Down, speed).Execute();
            }
            else if (Input.IsKeyDown(Keys.Up))
            {
                new MoveCommand(this, MoveDirection.Up, speed).Execute();
            }
        }

        protected override void UpdateAnimations()
        {
            if (currentAnimation == null)
            {
                return;
            }

            base.UpdateAnimations();

            // Standing still
            if (Velocity == Vector2.Zero)
            {
                if (direction.X < 0 && AnimationIsNot(AnimationType.IdleLeft))
                {
                    ChangeAnimation(AnimationType.IdleLeft);
                }
                else if (direction.X > 0 && AnimationIsNot(AnimationType.IdleRight))
                {
                    ChangeAnimation(AnimationType.IdleRight);
                }

                if (direction.Y < 0 && AnimationIsNot(AnimationType.IdleUp))
                {
                    ChangeAnimation(AnimationType.IdleUp);
                }
                else if (direction.Y > 0 && AnimationIsNot(AnimationType.IdleDown))
                {
                    ChangeAnimation(AnimationType.IdleDown);
                }
            }
            else // Moving
            {
                if (direction.X < 0 && AnimationIsNot(AnimationType.WalkLeft))
                {
                    ChangeAnimation(AnimationType.WalkLeft);
                }
                else if (direction.X > 0 && AnimationIsNot(AnimationType.WalkRight))
                {
                    ChangeAnimation(AnimationType.WalkRight);
                }
                else if (direction.Y < 0 && AnimationIsNot(AnimationType.WalkUp))
                {
                    ChangeAnimation(AnimationType.WalkUp);
                }
                else if (direction.Y > 0 && AnimationIsNot(AnimationType.WalkDown))
                {
                    ChangeAnimation(AnimationType.WalkDown);
                }
            }
        }
    }
}
