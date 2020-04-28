using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using LuxEngine.ECS;

namespace LuxEngine.ECS
{
    [Serializable]
    public class LoadedTexturesSingleton : AComponent<LoadedTexturesSingleton>, ISingleton
    {
        [NonSerialized]
        public readonly Dictionary<string, Texture2D> Textures;

        public LoadedTexturesSingleton()
        {
            Textures = new Dictionary<string, Texture2D>();
        }
    }

    public class TextureComponent : AComponent<TextureComponent>
    {
        public readonly string Name;

        public TextureComponent(string textureName)
        {
            Name = textureName;
        }
    }

    /// <summary>
    /// Responsible for loading textures (.png files)
    /// </summary>
    public static class TextureLoaderSystem
    {
        public static void CreateLoadedTexturesSingleton(Context context)
        {
            context.AddSingleton(new LoadedTexturesSingleton());
        }

        [OnAddComponent(typeof(TextureComponent))]
        public static void LoadTextureIntoSingleton(TextureComponent texture, LoadedTexturesSingleton loadedTextures)
        {
            // If texture is already loaded, no need to load it again
            if (loadedTextures.Textures.ContainsKey(texture.Name))
            {
                return;
            }

            // Invalid texture name
            if (texture.Name.Length == 0)
            {
                LuxCommon.Assert(false);
                return;
            }

            // Textures that start with "_" are reserved for creation in game
            if (texture.Name[0] == '_')
            {
                LuxCommon.Assert(false);
            }

            string texturePath = $"{HardCodedConfig.DEFAULT_TEXTURES_FOLDER_NAME}/{texture.Name}.png";
            Texture2D textureObj = TextureLoader.Load(texturePath, LuxGame.Instance.Content);

            loadedTextures.Textures.Add(texture.Name, textureObj);
        }
    }
}
