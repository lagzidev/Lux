using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class Collidable : BaseComponent<Collidable>
    {

    }

    public class MapCollisionSystem : BaseSystem<MapCollisionSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.Require<Sprite>();
            signature.Require<Collidable>();
        }

        protected override void PostUpdate()
        {

        }
    }
}
