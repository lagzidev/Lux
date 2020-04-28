//using System;
//using System.Collections.Generic;
//using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using SpriteFontPlus;
//using System.Text;
//using LuxEngine.ECS;
//using LuxEngine.ECS;

//namespace LuxEngine.ECS
//{
//    [Serializable]
//    public class Text : AComponent<Text>
//    {
//        public string TextStr;
//        public string FontName;
//        public int FontSize;
//        public Color Color;
//        public SpriteDepth Depth;

//        public Text(string text, string fontName, int fontSize, Color color, SpriteDepth depth)
//        {
//            FontName = fontName;
//            FontSize = fontSize;
//            TextStr = text;
//            Color = color;
//            Depth = depth;
//        }
//    }

//    [Serializable]
//    public class FontSingleton : AComponent<FontSingleton>
//    {
//        [NonSerialized]
//        public Dictionary<string, DynamicSpriteFont> Fonts;
//        public SpriteBatch SpriteBatch;

//        public FontSingleton(GraphicsDevice graphicsDevice)
//        {
//            Fonts = new Dictionary<string, DynamicSpriteFont>();
//            SpriteBatch = new SpriteBatch(graphicsDevice);
//        }
//    }

//    public class FontSystem : ASystem<FontSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Text>();
//            signature.Require<Transform>();
//            signature.RequireSingleton<FontSingleton>();
//            signature.RequireSingleton<SpriteBatchSingleton>();
//        }

//        public override void InitSingleton()
//        {
//            AddSingletonComponent(new FontSingleton(LuxGame.Graphics.GraphicsDevice));
//        }

//        protected override void OnRegisterEntity(Entity entity)
//        {
//            UnpackSingleton(out FontSingleton fonts);
//            Unpack(entity, out Text text);

//            using (var stream = File.OpenRead($"{LuxGame.ContentDirectory}/Fonts/{text.FontName}"))
//            {
//                DynamicSpriteFont font = DynamicSpriteFont.FromTtf(stream, 14);
//                fonts.Fonts.Add(text.FontName, font);
//            }
//        }

//        public override void PreDraw()
//        {
//            UnpackSingleton(out FontSingleton fonts);

//            fonts.SpriteBatch.Begin(
//                SpriteSortMode.BackToFront,
//                BlendState.AlphaBlend,
//                SamplerState.PointClamp,
//                DepthStencilState.Default,
//                RasterizerState.CullNone,
//                null);
//        }

//        public override void Draw()
//        {
//            UnpackSingleton(out FontSingleton fonts);

//            foreach (var entity in RegisteredEntities)
//            {
//                Unpack(entity, out Text text);
//                Unpack(entity, out Transform transform);

//                int defaultFontSize = fonts.Fonts[text.FontName].Size;
//                fonts.Fonts[text.FontName].Size = text.FontSize; //* resolutionSettings.WindowScale; // todo scale

//                fonts.Fonts[text.FontName].DrawString(
//                    fonts.SpriteBatch,
//                    text.TextStr,
//                    new Vector2(transform.X, transform.Y),
//                    text.Color,
//                    Vector2.One,
//                    DrawUtils.CalculateSpriteDepth(text.Depth));

//                fonts.Fonts[text.FontName].Size = defaultFontSize;
//            }
//        }

//        public override void PostDraw()
//        {
//            UnpackSingleton(out FontSingleton fonts);
//            fonts.SpriteBatch.End();
//        }
//    }
//}
