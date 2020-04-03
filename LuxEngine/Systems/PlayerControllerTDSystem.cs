using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable()]
    public class PlayerControlledTD : BaseComponent<PlayerControlledTD>
    {
        public PlayerControlledTD()
        {
        }
    }

    public class PlayerControllerTDSystem : BaseSystem<PlayerControllerTDSystem>
    {
        //TODO: Components to implement:
        // PhysicsWorld, Grounded, GameInput (DynamicPhysics?) https://youtu.be/W3aieHjyNvw?t=156

        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<PlayerControlledTD>();
            signature.Require<Transform>();
            signature.Require<Sprite>();
        }

        protected override void Update(GameTime gameTime)
        {
            var input = World.UnpackSingleton<InputSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var transform = World.Unpack<Transform>(entity);
                var sprite = World.Unpack<Sprite>(entity);

                if (input.Up)
                {
                    transform.Y -= 1;
                    sprite.CurrentAnimationName = "WalkUp";
                }
                else if (input.Down)
                {
                    transform.Y += 1;
                    sprite.CurrentAnimationName = "WalkDown";
                }

                if (input.Right)
                {
                    transform.X += 1;
                    sprite.CurrentAnimationName = "WalkRight";
                }
                else if (input.Left)
                {
                    transform.X -= 1;
                    sprite.CurrentAnimationName = "WalkLeft";
                }
            }
        }
    }
}
