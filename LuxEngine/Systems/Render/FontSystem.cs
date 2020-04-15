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
    public class Text : AComponent<Text>
    {
        public string TextStr;
        public string FontName;
        public int FontSize;
        public Color Color;
        public SpriteDepth Depth;

        public Text(string text, string fontName, int fontSize, Color color, SpriteDepth depth)
        {
            FontName = fontName;
            FontSize = fontSize;
            TextStr = text;
            Color = color;
            Depth = depth;
        }
    }

    [Serializable]
    public class FontSingleton : AComponent<FontSingleton>
    {
        [NonSerialized]
        public Dictionary<string, DynamicSpriteFont> Fonts;
        public SpriteBatch SpriteBatch;

        public FontSingleton(GraphicsDevice graphicsDevice)
        {
            Fonts = new Dictionary<string, DynamicSpriteFont>();
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }
    }

    public class FontSystem : ASystem<FontSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Text>();
            signature.Require<Transform>();
            signature.RequireSingleton<FontSingleton>();
            signature.RequireSingleton<SpriteBatchSingleton>();
        }

        protected override void InitSingleton()
        {
            _world.AddSingletonComponent(new FontSingleton(LuxGame.Graphics.GraphicsDevice));
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var fonts = _world.UnpackSingleton<FontSingleton>();
            var text = _world.Unpack<Text>(entity);

            using (var stream = File.OpenRead($"{LuxGame.ContentDirectory}/Fonts/{text.FontName}"))
            {
                DynamicSpriteFont font = DynamicSpriteFont.FromTtf(stream, 14);
                fonts.Fonts.Add(text.FontName, font);
            }
        }

        protected override void PreDraw()
        {
            var fonts = _world.UnpackSingleton<FontSingleton>();

            fonts.SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null);
        }

        protected override void Draw()
        {
            var fonts = _world.UnpackSingleton<FontSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var text = _world.Unpack<Text>(entity);
                var transform = _world.Unpack<Transform>(entity);

                int defaultFontSize = fonts.Fonts[text.FontName].Size;
                fonts.Fonts[text.FontName].Size = text.FontSize; //* resolutionSettings.WindowScale; // todo scale

                fonts.Fonts[text.FontName].DrawString(
                    fonts.SpriteBatch,
                    text.TextStr,
                    new Vector2(transform.X, transform.Y),
                    text.Color,
                    Vector2.One,
                    DrawUtils.CalculateSpriteDepth(text.Depth));

                fonts.Fonts[text.FontName].Size = defaultFontSize;
            }
        }

        protected override void PostDraw()
        {
            var fonts = _world.UnpackSingleton<FontSingleton>();
            fonts.SpriteBatch.End();
        }
    }
}
