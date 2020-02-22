using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class Animation
    {
        public string Name;
        public List<int> AnimationOrder = new List<int>();
        public int Speed;

        public Animation()
        {
        }

        public Animation(string inputName, int inputSpeed, List<int> inputAnmationOrder)
        {
            Name = inputName;
            Speed = inputSpeed;
            AnimationOrder = inputAnmationOrder;
        }
    }

    public class AnimationSet
    {
        public int Width; // Width of each frame
        public int Height; // Height of each frame
        public int GridX; // How many frames are in the X axis
        public int GridY; // How many frames are in the Y axis
        public List<Animation> AnimationList = new List<Animation>();

        public AnimationSet()
        {
        }

        public AnimationSet(int inputWidth, int inputHeight, int inputGridX, int inputGridY)
        {
            Width = inputWidth;
            Height = inputHeight;
            GridX = inputGridX;
            GridY = inputGridY;
        }
    }

    public class AnimationData
    {
        public AnimationSet Animation { get; set; }
        public string TexturePath { get; set; }
    }
}
