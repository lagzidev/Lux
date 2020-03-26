using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class SaveSystem : BaseSystem<SaveSystem>
    {
        public SaveSystem() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var input = World.SingletonEntity.Unpack<InputSingleton>();
            if (input.F4)
            {
                TextWriter writer = new StreamWriter("exported_world.xml");
                World.Serialize(writer);
                writer.Close();
            }
            else if (input.F5)
            {
                TextReader reader = new StreamReader("exported_world.xml");
                World.Deserialize(reader);
                reader.Close();
            }
        }
    }
}
