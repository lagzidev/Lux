using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class InputSingleton : BaseComponent<InputSingleton>
    {
        public bool Up;
        public bool Down;
        public bool Right;
        public bool Left;
    }

    /// <summary>
    /// Populates the InputSingleton component
    /// </summary>
    public class InputSystem : BaseSystem<InputSystem>
    {
        public InputSystem() : base(InputSingleton.ComponentType)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();
            //MouseState mouse = Mouse.GetState();

            foreach (var entity in RegisteredEntities)
            {
                var input = World.Unpack<InputSingleton>(entity);

                input.Up = keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up);
                input.Down = keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down);
                input.Right = keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right);
                input.Left = keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left);
            }
        }

        ///// <summary>
        ///// Checks if key was just pressed.
        ///// </summary>
        //public static bool KeyPressed(Keys input)
        //{
        //    if (keyboardState.IsKeyDown(input) == true && lastKeyboardState.IsKeyDown(input) == false)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Returns whether or not the left mouse button is being pressed.
        ///// </summary>
        //public static bool MouseLeftDown()
        //{
        //    if (mouseState.LeftButton == ButtonState.Pressed)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Returns whether or not the right mouse button is being pressed.
        ///// </summary>
        //public static bool MouseRightDown()
        //{
        //    if (mouseState.RightButton == ButtonState.Pressed)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Checks if the left mouse button was clicked.
        ///// </summary>
        //public static bool MouseLeftClicked()
        //{
        //    if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Checks if the right mouse button was clicked.
        ///// </summary>
        //public static bool MouseRightClicked()
        //{
        //    if (mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// Gets mouse coordinates adjusted for virtual resolution and camera position.
        ///// </summary>
        //public static Vector2 MousePositionCamera()
        //{
        //    Vector2 mousePosition = Vector2.Zero;
        //    mousePosition.X = mouseState.X;
        //    mousePosition.Y = mouseState.Y;

        //    return ScreenToWorld(mousePosition);
        //}

        ///// <summary>
        ///// Gets the last mouse coordinates adjusted for virtual resolution and camera position.
        ///// </summary>
        //public static Vector2 LastMousePositionCamera()
        //{
        //    Vector2 mousePosition = Vector2.Zero;
        //    mousePosition.X = lastMouseState.X;
        //    mousePosition.Y = lastMouseState.Y;

        //    return ScreenToWorld(mousePosition);
        //}

        ///// <summary>
        ///// Takes screen coordinates (2D position like where the mouse is on screen) then converts it to world position (where we clicked at in the world). 
        ///// </summary>
        //private static Vector2 ScreenToWorld(Vector2 input)
        //{
        //    input.X -= Resolution.VirtualViewportX;
        //    input.Y -= Resolution.VirtualViewportY;

        //    return Vector2.Transform(input, Matrix.Invert(Camera.GetTransformMatrix()));
        //}
    }
}
