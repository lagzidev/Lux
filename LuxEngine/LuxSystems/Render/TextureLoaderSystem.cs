//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework.Graphics;
//using LuxEngine.ECS;

//namespace LuxEngine.Systems
//{
//    [Serializable]
//    public class LoadedTexturesSingleton : AComponent<LoadedTexturesSingleton>
//    {
//        [NonSerialized]
//        public readonly Dictionary<string, Texture2D> Textures;

//        public LoadedTexturesSingleton()
//        {
//            Textures = new Dictionary<string, Texture2D>();
//        }
//    }

//    public class TextureComponent : AComponent<TextureComponent>
//    {
//        public string Name;

//        public TextureComponent(string textureName)
//        {
//            Name = textureName;
//        }
//    }

//    /// <summary>
//    /// Responsible for loading textures (.png files)
//    /// </summary>
//    public class TextureLoaderSystem : ASystem<TextureLoaderSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<TextureComponent>();
//            signature.RequireSingleton<LoadedTexturesSingleton>();
//        }

//        public override void InitSingleton()
//        {
//            AddSingletonComponent(new LoadedTexturesSingleton());
//        }

//        public override void LoadContent()
//        {
//            foreach (var entity in RegisteredEntities)
//            {
//                AddTexture(entity);
//            }
//        }

//        protected override void OnRegisterEntity(Entity entity)
//        {
//            // If LoadContent was already called, load the texture.
//            // Otherwise LoadContent will load it for us
//            if (IsReadyToLoadContent)
//            {
//                AddTexture(entity);
//            }
//        }

//        private void AddTexture(Entity entity)
//        {
//            UnpackSingleton(out LoadedTexturesSingleton loadedTextures);
//            Unpack(entity, out TextureComponent texture);

//            // If texture is already loaded, no need to load it again
//            if (loadedTextures.Textures.ContainsKey(texture.Name))
//            {
//                return;
//            }

//            // Invalid texture name
//            if (texture.Name.Length == 0)
//            {
//                LuxCommon.Assert(false);
//                return;
//            }

//            // Textures that start with "_" are reserved for creation in game
//            if (texture.Name[0] == '_')
//            {
//                LuxCommon.Assert(false);
//            }

//            string texturePath = $"{HardCodedConfig.DEFAULT_TEXTURES_FOLDER_NAME}/{texture.Name}.png";
//            Texture2D textureObj = TextureLoader.Load(texturePath, LuxGame.Instance.Content);

//            loadedTextures.Textures.Add(texture.Name, textureObj);
//        }
//    }
//}
