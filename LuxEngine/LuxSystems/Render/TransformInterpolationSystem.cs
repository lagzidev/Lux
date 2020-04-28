//using LuxEngine.ECS;

//namespace LuxEngine.ECS
//{
//    public class TransformInterpolationSystem : ASystem<TransformInterpolationSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Transform>();
//            signature.KeepPreviousState<Transform>();
//        }



//        [ECSFilter(typeof(Transform))]
//        public static void Init2()
//        {

//        }

//        public static void Update2(Transform transform)
//        {

//        }

//        public static void Draw2(World world, Transform transform, Previous<Transform> previousTransform) // TODO: Not<Parent> _
//        {

//        }

//        // TODO: Decouple engine from XNA

//        //public override void Integrate()
//        //{
//        //    //foreach (var entity in RegisteredEntities)
//        //    //{
//        //    //    Unpack(entity, out Transform transform);
//        //    //    UnpackPrevious(entity, out Transform prevTransform);

//        //    //    transform.X = (float)(transform.X * Time.Alpha + prevTransform.X * (1.0 - Time.Alpha));
//        //    //    transform.Y = (float)(transform.Y * Time.Alpha + prevTransform.Y * (1.0 - Time.Alpha));
//        //    //}
//        //}
//    }
//}
