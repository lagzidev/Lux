using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public enum SpriteDepth : int
    {
        Normal = 1, // TODO: Change the name
    }

    public class SpriteComponent : BaseComponent<SpriteComponent>
    {
        public Color Color;
        public float Rotation;
        public SpriteDepth SpriteDepth;

        // This is set by the RenderSystem
        public Texture2D Texture;

        // TODO: Add: bool mipMap, SurfaceFormat format (for Texture2d)
        public SpriteComponent(Color color, SpriteDepth spriteDepth, float rotation = 0) 
        {
            Color = color;
            Rotation = rotation;
            SpriteDepth = spriteDepth;

            Texture = null;
        }
    }

    public class RenderSystem : BaseSystem
    {
        private SpriteBatch _spriteBatch;

        public RenderSystem() :
            base(SpriteComponent.ComponentType, TransformComponent.ComponentType) // TODO: Improve this syntax and only specify type
        {
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            base.LoadContent(graphicsDevice);

            _spriteBatch = new SpriteBatch(graphicsDevice);

            // Instantiate texture for every sprite
            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<SpriteComponent>(entity);
                var transform = World.Unpack<TransformComponent>(entity);

                sprite.Texture = new Texture2D(graphicsDevice, transform.Size.Width, transform.Size.Height);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

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
                    transform.Position,
                    transform.Size,
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
