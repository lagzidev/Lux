using System;

namespace LuxEngine
{
    public class RenderSuper : ASuperSystem
    {
        public override void Init(World world)
        {
            //Action x = (Action);
            world.Register(TransformInterpolationSystem.Init2);

            world.RegisterSystem<CameraSystem>();
            world.RegisterSystem<TextureLoaderSystem>();
            world.RegisterSystem<SpriteLoaderSystem>();
            world.RegisterSystem<SpriteBatchSystem>();
            world.RegisterSystem<SpriteDrawSystem>();
            world.RegisterSystem<AnimationSystem>();
        }

        public override void LoadContent(World world)
        {
        }

        public override void Update(World world)
        {
            world.Register<Transform>(TransformInterpolationSystem.Update2);

        }

        public override void UpdateFixed(World world)
        {
        }

        public override void Draw(World world)
        {
            Action x = TransformInterpolationSystem.Draw2;
            world.Register(TransformInterpolationSystem.Draw2);
        }
    }
}
