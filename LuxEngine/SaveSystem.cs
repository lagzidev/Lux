using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class SaveSystem : BaseSystem<SaveSystem>
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private ContentManager _contentManager;

        public SaveSystem() : base()
        {
            _graphicsDeviceManager = null;
            _contentManager = null;
        }

        public override void Init(GraphicsDeviceManager graphicsDeviceManager)
        {
            base.Init(graphicsDeviceManager);
            _graphicsDeviceManager = graphicsDeviceManager;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            base.LoadContent(graphicsDevice, contentManager);
            _contentManager = contentManager;
        }

        public override void PostUpdate(GameTime gameTime)
        {
            base.Update(gameTime);

            var input = World.SingletonEntity.Unpack<InputSingleton>();
            if (input.F4)
            {
                FileStream stream = File.Open("exported_world.bin", FileMode.Create);

                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    World.Serialize(writer);
                }
            }
            else if (input.F5)
            {
                FileStream stream = File.Open("exported_world.bin", FileMode.Open);

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    World.InitWorld(reader, _graphicsDeviceManager, _contentManager);
                }
            }
        }
    }
}
