using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LuxEngine
{
    /// <summary>
    /// Rendering depth of the sprite.
    /// The actual enum int value will be converted to float and divided by 10
    /// when passed to the sprite batch.
    /// </summary>
    public enum SpriteDepth : int
    {
        Min = 0, // Front-most

        Character = 5,
        Wall = 6,

        Max = 10 // Deepest
    }

    /// <summary>
    /// Represents a file corresponding to a texture (e.g. FireSword.json)
    /// </summary>
    [Serializable]
    public class SpriteData
    {
        public Dictionary<string, Animation> Animations; // todo readonly
    }

    [Serializable]
    public class Sprite : BaseComponent<Sprite>
    {
        public readonly string TextureName;

        public SpriteData SpriteData;
        public string CurrentAnimationName;
        public int CurrentAnimationFrame;

        public Sprite(string textureName)
        {
            TextureName = textureName;
        }
    }

    /// <summary>
    /// Responsible for loading sprite data (e.g. FireSword.json)
    /// </summary>
    public class SpriteLoaderSystem : BaseSystem<SpriteLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Sprite>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var sprite = World.Unpack<Sprite>(entity);

            var path = $"{World.ContentManager.RootDirectory}/Textures/{sprite.TextureName}.json";

            // TODO: Instead of saving a Sprite, save a Sprite[][], with each having a string that labels
            // the animation

            JsonSerializer jsonSerializer = new JsonSerializer();

            //using (StreamWriter sw = new StreamWriter(path))
            //using (JsonWriter writer = new JsonTextWriter(sw))
            //{
            //    // 11, 25, 0, 0, Color.White, 0, new Vector2(20, 20), SpriteEffects.None, SpriteDepth.Character
            //    var sd = new SpriteData();
            //    var anim = new Animation();
            //    var frame = new AnimationFrame();

            //    anim.Frames = new List<AnimationFrame> { frame };
            //    sd.Animations = new Dictionary<string, Animation>();
            //    sd.Animations.Add("Fly", anim);

            //    jsonSerializer.Serialize(writer, sd);
            //}

            // Load sprite data into the sprite component
            using (StreamReader sw = new StreamReader(path))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                SpriteData spriteData = jsonSerializer.Deserialize<SpriteData>(reader);
                sprite.SpriteData = spriteData;
                sprite.CurrentAnimationName = sprite.SpriteData.Animations.Keys.First();
            }
        }
    }
}