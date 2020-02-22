using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace LuxEngine
{
    public class Map
    {
        public List<Wall> walls = new List<Wall>();
        Texture2D wallImage;

        public int mapWidth = 15;
        public int mapHeight = 9;
        public int tileSize = 32;

        public Map()
        {
        }

        public void Load(ContentManager content)
        {
            wallImage = TextureLoader.Load("pixel", content);
        }

        public Rectangle CheckCollision(Rectangle input)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].rectangle.Intersects(input))
                {
                    return walls[i].rectangle;
                }
            }

            return Rectangle.Empty;
        }

        public void DrawWalls(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].active == true)
                {
                    spriteBatch.Draw(wallImage, new Vector2(walls[i].rectangle.X, walls[i].rectangle.Y), walls[i].rectangle, Color.Green, 0f, Vector2.Zero, 1f, SpriteEffects.None, .7f);
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
