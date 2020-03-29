using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LuxEngine
{
    public class AnimationL
    {
        public string Name;
        public List<int> AnimationOrder = new List<int>();
        public int Speed;

        public AnimationL()
        {
        }

        public AnimationL(string inputName, int inputSpeed, List<int> inputAnmationOrder)
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
        public List<AnimationL> AnimationList = new List<AnimationL>();

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

        /// <summary>
        /// Fills out a AnimationData object with all the items we need for the requested animation.
        /// </summary>
        public static SpriteData Load(string name)
        {
            //Load the XML data of the animation and return it:
            XmlSerializer serializer = new XmlSerializer(typeof(SpriteData));
            TextReader reader = new StreamReader("Content/Animations/" + name);
            SpriteData obj = (SpriteData)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }
    }

}
