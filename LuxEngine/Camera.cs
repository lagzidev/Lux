using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class Camera : BaseComponent<Camera>
    {
        public Camera()
        {
        }
    }

    [Serializable]
    public class PlatformerLens : BaseComponent<PlatformerLens>
    {
        public PlatformerLens()
        {
        }
    }

    public class CameraSystem : BaseSystem<CameraSystem>
    {
        public CameraSystem() : base(Camera.ComponentType, Transform.ComponentType)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {
                var transform = World.Unpack<Transform>(entity);

                Entity parent = World.Unpack<Parent>(entity).ParentEntity;
                var parentTransform = World.Unpack<Transform>(parent);

                PlatformerLens platformerLens;
                if (World.TryUnpack(entity, out platformerLens))
                {
                    // Se
                }
            }
        }
    }
}
