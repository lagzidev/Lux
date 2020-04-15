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

        protected override void Integrate()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Transform transform);
                UnpackPrevious(entity, out Transform prevTransform);

                transform.X = (float)(transform.X * Time.Alpha + prevTransform.X * (1.0 - Time.Alpha));
                transform.Y = (float)(transform.Y * Time.Alpha + prevTransform.Y * (1.0 - Time.Alpha));
            }
        }
    }
}
