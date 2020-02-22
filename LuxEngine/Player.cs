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
            position = inputPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            image = TextureLoader.Load("sprite", content);
            base.Load(content);
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            CheckInput(objects, map);
            base.Update(objects, map);
        }

        private void CheckInput(List<GameObject> objects, Map map)
        {
            if (Input.IsKeyDown(Keys.Right))
            {
                MoveRight();
            }
            else if (Input.IsKeyDown(Keys.Left))
            {
                MoveLeft();
            }

            if (Character.applyGravity)
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    Jump(map);
                }
            }
            else
            {
                if (Input.IsKeyDown(Keys.Down))
                {
                    MoveDown();
                }
                else if (Input.IsKeyDown(Keys.Up))
                {
                    MoveUp();
                }
            }
        }
    }
}
