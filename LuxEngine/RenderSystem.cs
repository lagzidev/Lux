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
            base(SpriteComponent.ComponentType, Transform.ComponentType) // TODO: Improve this syntax and only specify type
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

            //_graphicsDevice.Viewport = new Viewport(0, 0, 100, 100);

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
                var transform = World.Unpack<Transform>(entity);

                // Handle relationship logic
                Relationship  relationship;
                float parentX = 0;
                float parentY = 0;
                if (World.TryUnpack(entity, out relationship))
                {
                    var parentTransform = relationship.ParentEntity.Unpack<Transform>();
                    parentX = parentTransform.X;
                    parentY = parentTransform.Y;
                }

                _spriteBatch.Draw(
                    sprite.Texture,
                    new Vector2(transform.X + parentX, transform.Y + parentY),
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
