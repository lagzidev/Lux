using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    //[Serializable]
    //public class SpriteComponent : BaseComponent<SpriteComponent>
    //{
    //    public string TextureFilePath;
    //    public Rectangle PositionInTexture;
    //    public Color Color;
    //    public float Rotation;
    //    public SpriteDepth SpriteDepth;

    //    [NonSerialized]
    //    public Texture2D Texture; // Set by the RenderSystem

    //    // TODO: Add: bool mipMap, SurfaceFormat format (for Texture2d)
    //    public SpriteComponent(string textureFilePath, Rectangle positionInTexture, SpriteDepth spriteDepth, Color color, float rotation = 0)
    //    {
    //        TextureFilePath = textureFilePath;
    //        PositionInTexture = positionInTexture;
    //        Color = color;
    //        Rotation = rotation;
    //        SpriteDepth = spriteDepth;

    //        Texture = null;
    //    }
    //}

    public class RenderSystem : BaseSystem<RenderSystem>
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        public override Type[] GetRequiredComponents()
        {
            return new Type[]
            {
                typeof(Sprite),
                typeof(Transform)
            };
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

           if (World.TryUnpack(World.SingletonEntity, out ResolutionSingleton resolution))
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

            // Get loaded textures
            var loadedTextures = World.Unpack<LoadedTexturesSingleton>(World.SingletonEntity);

            foreach (var entity in RegisteredEntities)
            {
                var sprite = World.Unpack<Sprite>(entity);
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
                    loadedTextures.Textures[sprite.SpriteData.TextureName],
                    new Vector2(transform.X + parentX, transform.Y + parentY),
                    sprite.SpriteData.PositionInTexture,
                    sprite.SpriteData.Color,
                    sprite.SpriteData.Rotation,
                    Vector2.Zero,
                    sprite.SpriteData.Scale,
                    sprite.SpriteData.SpriteEffects,
                    (float)sprite.SpriteData.SpriteDepth / 10f);
            }

            _spriteBatch.End();
        }
    }
}
