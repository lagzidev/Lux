using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class LoadedTexturesSingleton : BaseComponent<LoadedTexturesSingleton>
    {
        [NonSerialized]
        public readonly Dictionary<string, Texture2D> Textures;

        public LoadedTexturesSingleton()
        {
            Textures = new Dictionary<string, Texture2D>();
        }
    }

    public class TextureComponent : BaseComponent<TextureComponent>
    {
        public string Name;

        public TextureComponent(string textureName)
        {
            Name = textureName;
        }
    }

    /// <summary>
    /// Responsible for loading textures (.png files)
    /// </summary>
    public class TextureLoaderSystem : BaseSystem<TextureLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<TextureComponent>();
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
            // If LoadContent was already called, load the texture.
            // Otherwise LoadContent will load it for us
            if (IsReadyToLoadContent)
            {
                AddTexture(entity);
            }
        }

        private void AddTexture(Entity entity)
        {
            var loadedTexturesSingleton = World.UnpackSingleton<LoadedTexturesSingleton>();
            string textureName = World.Unpack<TextureComponent>(entity).Name;

            // If texture is already loaded, no need to load it again
            if (loadedTexturesSingleton.Textures.ContainsKey(textureName))
            {
                return;
            }

            // Invalid texture name
            if (textureName.Length == 0)
            {
                LuxCommon.Assert(false);
                return;
            }

            // Textures that start with "_" are reserved for creation in game
            if (textureName[0] == '_')
            {
                LuxCommon.Assert(false);
            }

            string texturePath = $"{HardCodedConfig.DEFAULT_TEXTURES_FOLDER_NAME}/{textureName}.png";
            Texture2D textureObj = TextureLoader.Load(texturePath, LuxGame.Instance.Content);

            loadedTexturesSingleton.Textures.Add(textureName, textureObj);
        }
    }
}
