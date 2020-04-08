using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace LuxEngine
{
    public class Mapold
    {
        public List<Wall> Walls = new List<Wall>();
        Texture2D wallImage;

        public int MapWidth = 15;
        public int MapHeight = 9;
        public int TileSize = 32;

        public Mapold()
        {
        }

        public void Load(ContentManager content)
        {
            wallImage = TextureLoader.Load("pixel", content);
        }

        public Rectangle CheckCollision(Rectangle input)
        {
            for (int i = 0; i < Walls.Count; i++)
            {
                if (Walls[i] != null && Walls[i].rectangle.Intersects(input))
                {
                    return Walls[i].rectangle;
                }
            }

            return Rectangle.Empty;
        }

        public void DrawWalls(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Walls.Count; i++)
            {
                if (Walls[i] != null && Walls[i].active == true)
                {
                    spriteBatch.Draw(wallImage, new Vector2(Walls[i].rectangle.X, Walls[i].rectangle.Y), Walls[i].rectangle, Color.Green, 0f, Vector2.Zero, 1f, SpriteEffects.None, .7f);
                }
            }
        }
    }

    public class Wall
    {
        public Rectangle rectangle;
        public bool active = true;

        public Wall()
        {

        }

        public Wall(Rectangle inputRectangle)
        {
            rectangle = inputRectangle;
        }
    }
}
