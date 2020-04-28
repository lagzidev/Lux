using System;
using LuxEngine.ECS;
using LuxEngine.ECS;

namespace LuxEngine.Features
{
    public class RenderFeature : IFeature, IUpdateFeature, IDrawFeature, IInitFeature, IOnAddComponent
    {
        public void Init(Systems systems)
        {
            systems
                .Add<Context>
                    (TextureLoaderSystem.CreateLoadedTexturesSingleton)
                .Add<Context>
                    (SpriteBatchSystem.CreateSpriteBatchSingleton)
            ;
        }

        public void OnAddComponent(Systems systems)
        {
            systems
                .Add<TextureComponent, LoadedTexturesSingleton>
                    (TextureLoaderSystem.LoadTextureIntoSingleton)

                .Add<Sprite, Context>
                    (SpriteLoaderSystem.LoadSpriteTextureInfo)
            ;
        }

        public void Update(Systems systems)
        {
            //world.RegisterSystem<CameraSystem>();
            //world.RegisterSystem<TextureLoaderSystem>();
            //world.RegisterSystem<SpriteLoaderSystem>();
            //world.RegisterSystem<SpriteDrawSystem>();
            //world.RegisterSystem<AnimationSystem>();
        }

        public void Draw(Systems systems)
        {
            systems
                .Add<Sprite>
                    (AnimationSystem.UpdateAnimation)

                .Add<SpriteBatchSingleton>
                    (SpriteBatchSystem.BeginDrawToRenderTarget)

                .Add<SpriteBatchSingleton, LoadedTexturesSingleton, Sprite, Transform, TextureComponent>
                    (SpriteBatchSystem.DrawToRenderTarget)

                .Add<SpriteBatchSingleton>
                    (SpriteBatchSystem.DrawRenderTargetToScreen)
            ;
        }
    }
}
