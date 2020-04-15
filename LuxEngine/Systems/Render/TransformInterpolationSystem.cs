using System;
namespace LuxEngine
{
    public class TransformInterpolationSystem : ASystem<TransformInterpolationSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Transform>();
            signature.KeepPreviousState<Transform>();
        }

        // TODO: Decouple engine from XNA

        protected override void Update()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Transform transform);
                UnpackPrevious(entity, out Transform prevTransform);
                //transform.X;
            }
        }
    }
}
