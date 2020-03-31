//using System;
//using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

//namespace LuxEngine
//{
//    [Serializable]
//    public class Text : BaseComponent<Text>
//    {
//        public string TextStr;

//        public Text(string text)
//        {
//            TextStr = text;
//        }
//    }

//    [Serializable]
//    public class FontSingleton : BaseComponent<FontSingleton>
//    {
//        public string FontDescriptorFile;
//        public string TexturePNGFile;

//        public FontSingleton(string fontDescriptorFile, string texturePNGFile)
//        {
//            FontDescriptorFile = fontDescriptorFile;
//            TexturePNGFile = texturePNGFile;
//        }
//    }

//    public class FontSystem : BaseSystem<FontSystem>
//    {
//        private FontRenderer _fontRenderer;

//        public void Draw(String message, Vector2 pos, SpriteBatch _spriteBatch)
//        {
//            _fontRenderer.DrawText(_spriteBatch, (int)pos.X, (int)pos.Y, message);
//        }

//        public override Type[] GetRequiredComponents()
//        {
//            return new Type[]
//            {
//                typeof(Text)
//            };
//        }

//        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
//        {
//            base.LoadContent(graphicsDevice, contentManager);

//            var font = World.Unpack<FontSingleton>(World.SingletonEntity);

//            string fontFilePath = Path.Combine(contentManager.RootDirectory, font.FontDescriptorFile);
//            Texture2D fontTexture = contentManager.Load<Texture2D>(font.TexturePNGFile);
//            FontFile fontFile = FontLoader.Load(fontFilePath);

//            _fontRenderer = new FontRenderer(fontFile, fontTexture);
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            base.Draw(gameTime);

//            // TODO: Maybe extract drawing logic from .DrawText and onto RenderSystem?
//            //_fontRenderer.DrawText()
//        }
//    }
//}
