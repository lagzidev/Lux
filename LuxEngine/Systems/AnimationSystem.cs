using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class AnimationFrame
    {
        public int Width;
        public int Height;
        public int TexturePositionX;
        public int TexturePositionY;
        public Color Color;
        public float Rotation;
        public Vector2 Scale;
        public SpriteEffects SpriteEffects;
        public SpriteDepth SpriteDepth;
    }

    /// <summary>
    /// A collection of animation frames
    /// </summary>
    [Serializable]
    public class Animation
    {
        public List<AnimationFrame> Frames; // todo readonly
    }

    //public class AnimationSystem : BaseSystem<AnimationSystem>
    //{
    //    protected override void SetSignature(SystemSignature signature)
    //    {
    //        signature.Require<Animation>();
    //    }
    //}
}
