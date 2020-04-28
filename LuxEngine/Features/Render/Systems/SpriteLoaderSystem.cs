using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using LuxEngine.ECS;

namespace LuxEngine.ECS
{
    /// <summary>
    /// Rendering depth of the sprite.
    /// The actual enum int value will be converted to float and divided by 10
    /// when passed to the sprite batch.
    /// </summary>
    public enum SpriteDepth : int
    {
        Min = 1, // Front-most

        OverCharacter = 3,
        Character = 4,
        BehindCharacter = 5,

        Max = 9 // Deepest
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
    public class Sprite : AComponent<Sprite>
    {
        public readonly string SpriteName;

        public SpriteData SpriteData;
        public string CurrentAnimationName;
        public int CurrentAnimationFrame;
        public float CurrentTimeInFrameMs;

        public Sprite(string spriteName)
        {
            SpriteName = spriteName;
        }
    }

    /// <summary>
    /// Responsible for loading sprite data (e.g. FireSword.json)
    /// </summary>
    public static class SpriteLoaderSystem
    {
        [OnAddComponent(typeof(Sprite))]
        public static void LoadSpriteTextureInfo(Sprite sprite, Context context)
        {
            string path = $"{LuxGame.ContentDirectory}/Textures/{sprite.SpriteName}.json";

            // Load sprite data into the sprite component
            using (StreamReader sw = new StreamReader(path))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                SpriteData spriteData = jsonSerializer.Deserialize<SpriteData>(reader);
                sprite.SpriteData = spriteData;

                // If fails here, check if your aseprite animation has tags
                sprite.CurrentAnimationName = sprite.SpriteData.Animations.Keys.First();
            }

            context.AddComponent(sprite.Entity, new TextureComponent(sprite.SpriteName));
        }
    }
}