//using System;
//using System.Collections.Generic;

//namespace LuxEngine.Systems
//{
//    [Serializable()]
//    public class PlayerControlledTD : BaseComponent<PlayerControlledTD>
//    {
//        public PlayerControlledTD()
//        {
//        }
//    }

//    public class PlayerControllerTDSystem : BaseSystem<PlayerControllerTDSystem>
//    {
//        //TODO: Components to implement:
//        // PhysicsWorld, Grounded, GameInput (DynamicPhysics?) https://youtu.be/W3aieHjyNvw?t=156

//        public override Type[] GetRequiredComponents()
//        {
//            return new Type[]
//            {
//                typeof(PlayerControlledTD),
//                typeof(Transform)
//            };
//        }

//        public override void Update(GameTime gameTime)
//        {
//            foreach (var entity in RegisteredEntities)
//            {
//                var transform = World.Unpack<Transform>(entity);
//                var input = World.Unpack<InputSingleton>(World.SingletonEntity);

//                if (input.Up)
//                {
//                    transform.Y -= 1;
//                }
//                else if (input.Down)
//                {
//                    transform.Y += 1;
//                }

//                if (input.Right)
//                {
//                    transform.X += 1;
//                }
//                else if (input.Left)
//                {
//                    transform.X -= 1;
//                }
//            }
//        }
//    }
//}
