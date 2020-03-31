//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Xml.Serialization;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

//namespace LuxEngine
//{
//    /// <summary>
//    /// Rendering depth of the sprite.
//    /// The actual enum int value will be converted to float and divided by 10
//    /// when passed to the sprite batch.
//    /// </summary>
//    public enum SpriteDepth : int
//    {
//        Min = 0, // Front-most

//        Character = 5,
//        Wall = 6,

//        Max = 10 // Deepest
//    }

//    [Serializable]
//    public class SpriteData
//    {
//        public AnimationSet Animation { get; set; }
//        public string TextureName { get; set; }
//        public Rectangle PositionInTexture;
//        public Color Color;
//        public float Rotation;
//        public Vector2 Scale;
//        public SpriteEffects SpriteEffects;
//        public SpriteDepth SpriteDepth;

//        public static void Serialize(TextWriter writer, SpriteData spriteData)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(SpriteData));
//            serializer.Serialize(writer, spriteData);
//        }

//        public static SpriteData Deserialize(TextReader reader)
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(SpriteData));
//            return (SpriteData)serializer.Deserialize(reader);
//        }
//    }

//    [Serializable]
//    public class Sprite : BaseComponent<Sprite>
//    {
//        public string Name;

//        [NonSerialized] // Prevents the save system from saving this data
//        public SpriteData SpriteData;

//        public Sprite(string animationName)
//        {
//            Name = animationName;
//            SpriteData = null;
//        }
//    }

//    [Serializable]
//    public class LoadedTexturesSingleton : BaseComponent<LoadedTexturesSingleton>
//    {
//        [NonSerialized]
//        public Dictionary<string, Texture2D> Textures;

//        public LoadedTexturesSingleton()
//        {
//            Textures = new Dictionary<string, Texture2D>();
//        }
//    }

//    public class SpriteLoaderSystem : BaseSystem<SpriteLoaderSystem>
//    {
//        public static string DEFAULT_ANIMATIONS_FOLDER_NAME = "Animations";

//        public override Type[] GetRequiredComponents()
//        {
//            return new Type[]
//            {
//                typeof(Sprite)
//            };
//        }

//        // TODO: Move logic from load content into here
//        //public override void RegisterEntity(Entity entity)
//        //{
//        //    base.RegisterEntity(entity);

//        //}


//        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
//        {
//            base.LoadContent(graphicsDevice, contentManager);

//            foreach (var entity in RegisteredEntities)
//            {
//                var sprite = World.Unpack<Sprite>(entity);

//                // TODO: Make path more mod friendly
//                // Get animation data from XML file
//                var animationPath =
//                    $"{contentManager.RootDirectory}/{DEFAULT_ANIMATIONS_FOLDER_NAME}/{sprite.Name}.xml";
//                using (TextReader reader = new StreamReader(animationPath))
//                {
//                    sprite.SpriteData = SpriteData.Deserialize(reader);
//                }

//                // Load texture if not already loaded
//                var loadedTextures = World.Unpack<LoadedTexturesSingleton>(World.SingletonEntity);
//                if (!loadedTextures.Textures.ContainsKey(sprite.SpriteData.TextureName))
//                {
//                    Texture2D texture = TextureLoader.Load(sprite.SpriteData.TextureName, contentManager);
//                    loadedTextures.Textures.Add(sprite.SpriteData.TextureName, texture);
//                }
//            }
//        }
//    }
//}
