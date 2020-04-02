using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public enum TextureTheme : int
    {
        Normal = 0
    }

    // The goal of the texture pack is to enable: foreach sprite: sprite.TexturePack.currentTexture = "winter"
    // to change the entire theme of the game
    // Or if we want a weapon to have multiple textures that are stored in multiple files (maybe mod files) and can be changed based on game state.
    // sword.TexturePack.currentTexture = "fireSwordMod"

    [Serializable]
    public class LoadedTexturesSingleton : BaseComponent<LoadedTexturesSingleton>
    {
        [NonSerialized]
        public Dictionary<string, Texture2D> Textures;

        public LoadedTexturesSingleton()
        {
            Textures = new Dictionary<string, Texture2D>();
        }
    }

    [Serializable]
    public class SpriteTexture : BaseComponent<SpriteTexture>
    {
        public readonly string TextureName;

        public SpriteTexture(string textureName)
        {
            TextureName = textureName;
        }
    }

    [Serializable]
    public class TextureData
    {
        //public AnimationSet Animation { get; set; }


        public static void Serialize(TextWriter writer, TextureData spriteData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextureData));
            serializer.Serialize(writer, spriteData);
        }

        public static TextureData Deserialize(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TextureData));
            return (TextureData)serializer.Deserialize(reader);
        }
    }

    /// <summary>
    /// Represents the current sprite properties
    /// </summary>
    [Serializable]
    public class Sprite : BaseComponent<Sprite>
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


        public Sprite(int width, int height, int texturePositionX, int texturePositionY, Color color, float rotation, Vector2 scale, SpriteEffects effects, SpriteDepth depth)
        {
            Width = width;
            Height = height;
            Color = color;
            Rotation = rotation;
            Scale = scale;
            SpriteEffects = effects;
            SpriteDepth = depth;
            TexturePositionX = texturePositionX;
            TexturePositionY = texturePositionY;
        }
    }

    public class TextureLoaderSystem : BaseSystem<TextureLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<SpriteTexture>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new LoadedTexturesSingleton());
        }

        protected override void LoadContent()
        {
            foreach (var entity in RegisteredEntities)
            {
                AddTexture(entity);
            }
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            if (IsReadyToLoadContent)
            {
                AddTexture(entity);
            }
        }

        private void AddTexture(Entity entity)
        {
            var loadedTexturesSingleton = World.UnpackSingleton<LoadedTexturesSingleton>();
            string textureName = World.Unpack<SpriteTexture>(entity).TextureName;

            // If texture is already loaded, no need to load it again
            if (loadedTexturesSingleton.Textures.ContainsKey(textureName))
            {
                return;
            }

            var texturePath = $"{HardCodedConfig.DEFAULT_TEXTURES_FOLDER_NAME}/{textureName}.png";
            Texture2D textureObj = TextureLoader.Load(texturePath, World.ContentManager);

            loadedTexturesSingleton.Textures.Add(textureName, textureObj);
        }
    }
}
