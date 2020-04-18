//using System;
//using System.Collections.Generic;
//using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using LuxEngine.ECS;

//namespace LuxEngine
//{
//    public class SaveSystem : BaseSystem<SaveSystem>
//    {
//        public override Type[] GetRequiredComponents()
//        {
//            return new Type[]
//            {
//            };
//        }

//        public override void PostUpdate(GameTime gameTime)
//        {
//            base.Update(gameTime);

//            var input = World.Unpack<InputSingleton>(World.SingletonEntity);
//            if (input.F4KeyPress)
//            {
//                FileStream stream = File.Open("exported_world.bin", FileMode.Create);

//                using (BinaryWriter writer = new BinaryWriter(stream))
//                {
//                    World.Serialize(writer);
//                }

//                Console.WriteLine("Saved!");
//            }
//            else if (input.F5KeyUp)
//            {
//                FileStream stream = File.Open("exported_world.bin", FileMode.Open);

//                using (BinaryReader reader = new BinaryReader(stream))
//                {
//                    World.InitWorld(reader);
//                }

//                Console.WriteLine("Loaded!");
//            }
//        }
//    }
//}
