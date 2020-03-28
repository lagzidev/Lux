using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class Camera : BaseComponent<Camera>
    {
        public Camera()
        {
        }
    }

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

                EntityHandle parent = World.Unpack<Parent>(entity).ParentEntity;
                var parentTransform = parent.Unpack<Transform>();

                PlatformerLens platformerLens;
                if (World.TryUnpack(entity, out platformerLens))
                {
                    // Se
                }
            }
        }
    }
}
