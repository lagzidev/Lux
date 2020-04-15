using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class DebugInfo : AComponent<DebugInfo>
    {
        public string Name;

        public DebugInfo(string name)
        {
            Name = name;
        }
    }

    public class DebugSystem : ASystem<DebugSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<DebugInfo>();
        }

        protected override void Update()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out DebugInfo debugInfo);

                if (Unpack(entity, out Transform transform))
                {
                    Console.WriteLine($"X: {transform.X} Y {transform.Y} - {debugInfo.Name}");
                }

            }
        }
    }

}
