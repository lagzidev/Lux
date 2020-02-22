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
            ChangeAnimation(AnimationType.IdleRight);

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
    }
}
