using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public enum SpriteDepth : int
    {
        Normal = 1, // TODO: Change the name
    }

    public class SpriteComponent : BaseComponent<SpriteComponent>
    {
        public string TextureFilePath;
        public Rectangle PositionInTexture;
        public Color Color;
        public float Rotation;
        public SpriteDepth SpriteDepth;

        // This is set by the RenderSystem
        public Texture2D Texture;

        // TODO: Add: bool mipMap, SurfaceFormat format (for Texture2d)
        public SpriteComponent(string textureFilePath, Rectangle positionInTexture, SpriteDepth spriteDepth, Color color, float rotation = 0)
        {
            TextureFilePath = textureFilePath;
            PositionInTexture = positionInTexture;
            Color = color;
            Rotation = rotation;
            SpriteDepth = spriteDepth;

            Texture = null;
        }
    }

    public class RenderSystem : IdentifiableSystem<RenderSystem>
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        public RenderSystem() :
            base(SpriteComponent.ComponentType, TransformComponent.ComponentType) // TODO: Improve this syntax and only specify type
        {
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            base.LoadContent(graphicsDevice, contentManager);

            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            // Instantiate texture for every sprite
            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<SpriteComponent>(entity);
                sprite.Texture = TextureLoader.Load(sprite.TextureFilePath, contentManager);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Clear what's on the screen each frame
            _graphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null); // , Camera.GetTransformMatrix()

            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<SpriteComponent>(entity);
                var transform = World.Unpack<TransformComponent>(entity);

                _spriteBatch.Draw(
                    sprite.Texture,
                    new Vector2(transform.X, transform.Y),
                    sprite.PositionInTexture,
                    sprite.Color,
                    sprite.Rotation,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    (float)sprite.SpriteDepth);
            }

            _spriteBatch.End();
        }
    }
}
