using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    /// <summary>
    /// Rendering depth of the sprite.
    /// The actual enum int value will be converted to float and divided by 10
    /// when passed to the sprite batch.
    /// </summary>
    public enum SpriteDepth : int
    {
        Min = 0, // Front-most

        Character = 5,
        Wall = 6,

        Max = 10 // Deepest
    }

    //public class TextureAsset : BaseComponent<TextureAsset>
    //{
    //    public string TextureFilePath;
    //}

    [Serializable]
    public class SpriteComponent : BaseComponent<SpriteComponent>
    {
        public string TextureFilePath;
        public Rectangle PositionInTexture;
        public Color Color;
        public float Rotation;
        public SpriteDepth SpriteDepth;

        [NonSerialized]
        public Texture2D Texture; // Set by the RenderSystem

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

    public class RenderSystem : BaseSystem<RenderSystem>
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        public RenderSystem() :
            base(SpriteComponent.ComponentType, Transform.ComponentType)
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

           if (World.SingletonEntity.TryUnpack(out ResolutionSingleton resolution))
            {
                // Resize the viewport to the whole window
                _graphicsDevice.Viewport = new Viewport(0, 0, resolution.Width, resolution.Height);

                // Clear to Black
                _graphicsDevice.Clear(Color.Black);

                // Calculate Proper Viewport according to Aspect Ratio
                _graphicsDevice.Viewport = ResolutionUtils.GetVirtualViewport(resolution);
                // and clear that so we can have black bars if aspect ratio requires it and
                // the clear color on the rest
            }

            _graphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                ResolutionUtils.GetScaleMatrix(resolution, _graphicsDevice.Viewport.Width)); // , Camera.GetTransformMatrix() // Black bars?


            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<SpriteComponent>(entity);
                var transform = World.Unpack<Transform>(entity);

                // Handle parent logic
                Parent parent;
                float parentX = 0;
                float parentY = 0;
                if (World.TryUnpack(entity, out parent))
                {
                    var parentTransform = World.Unpack<Transform>(parent.ParentEntity);
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
                    (float)sprite.SpriteDepth / 10f);
            }

            _spriteBatch.End();
        }
    }
}
