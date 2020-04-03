using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System.Text;

namespace LuxEngine
{
    [Serializable]
    public class Text : BaseComponent<Text>
    {
        public string TextStr;
        public string FontName;
        public int FontSize;
        public Color Color;

        public Text(string text, string fontName, int fontSize, Color color)
        {
            FontName = fontName;
            FontSize = fontSize;
            TextStr = text;
            Color = color;
        }
    }

    [Serializable]
    public class FontSingleton : BaseComponent<FontSingleton>
    {
        [NonSerialized]
        public Dictionary<string, DynamicSpriteFont> Fonts;

        public FontSingleton()
        {
            Fonts = new Dictionary<string, DynamicSpriteFont>();
        }
    }

    public class FontSystem : BaseSystem<FontSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Text>();
            signature.Require<Transform>();
            signature.RequireSingleton<FontSingleton>();
        }

        protected override void InitSingleton()
        {
            World.AddSingletonComponent(new FontSingleton());
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var fonts = World.UnpackSingleton<FontSingleton>();
            var text = World.Unpack<Text>(entity);

            using (var stream = File.OpenRead($"{World.ContentManager.RootDirectory}/Fonts/{text.FontName}"))
            {
                DynamicSpriteFont font = DynamicSpriteFont.FromTtf(stream, text.FontSize);
                fonts.Fonts.Add(text.FontName, font);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var fonts = World.UnpackSingleton<FontSingleton>();
            var spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;

            foreach (var entity in RegisteredEntities)
            {
                var text = World.Unpack<Text>(entity);
                var transform = World.Unpack<Transform>(entity);

                fonts.Fonts[text.FontName].DrawString(spriteBatch, text.TextStr, new Vector2(transform.X, transform.Y), text.Color);
            }
        }
    }
}
