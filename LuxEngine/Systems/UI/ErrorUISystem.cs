using System;
using LuxProtobuf;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class ErrorUIElement : AComponent<ErrorUIElement>
    {
    }

    public class ErrorUISystem : ASystem<ErrorUISystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.RequireSingleton<FontSingleton>();
            signature.Require<ErrorUIElement>();
            signature.Require<Text>();
        }

        public override void Init()
        {
            Entity entity = CreateEntity();
            AddComponent(entity, new ErrorUIElement());
            AddComponent(entity, new Transform(50, 10));
            AddComponent(entity, new Text("", "arial", 20, Color.Black, SpriteDepth.OverCharacter));
        }

        public override void PreDraw()
        {
            // For each error UI element
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Text text);
                text.TextStr = LuxGame.Error.Message;
            }
        }
    }
}
