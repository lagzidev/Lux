using System;
namespace LuxEngine
{
    public class Collidable : BaseComponent<Collidable>
    {

    }

    public class CollisionSystem : BaseSystem<CollisionSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Collidable>();
        }


    }
}
