using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class TransformComponent : BaseComponent<TransformComponent>
    {
        // TODO: Consider removing Position and just use Size with the
        // disadvantage of having X, Y as integers instead of floats

        // We create these types here so we don't have to do create a new
        // instance every time the Render system draws.
        private Rectangle _size;
        private Vector2 _position;

        public Rectangle Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                _position.X = value.X;
                _position.Y = value.Y;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                // TODO: Check if this works, seems like because it's a struct it might not mutate Size at all. See: https://stackoverflow.com/questions/1747654/error-cannot-modify-the-return-value-c-sharp
                _size.X = (int)value.X;
                _size.Y = (int)value.Y;
            }
        }


        public TransformComponent(float x, float y , int width, int height)
        {
            _size = new Rectangle((int)x, (int)y, width, height);
            _position = new Vector2(x, y);
        }
    }

    //public class PhysicsSystem
    //{
    //    public PhysicsSystem()
    //    {
    //    }
    //}
}
