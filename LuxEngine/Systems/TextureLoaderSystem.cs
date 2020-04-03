using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public enum TextureTheme : int
    {
        Normal = 0
    }

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

    /// <summary>
    /// Responsible for loading sprite textures (.png files)
    /// </summary>
    public class TextureLoaderSystem : BaseSystem<TextureLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Sprite>();
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
            string textureName = World.Unpack<Sprite>(entity).TextureName;

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
