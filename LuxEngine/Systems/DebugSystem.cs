using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class DebugInfo : BaseComponent<DebugInfo>
    {
        public string Name;

        public DebugInfo(string name)
        {
            Name = name;
        }
    }

    public class DebugSystem : BaseSystem<DebugSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<DebugInfo>();
        }

        protected override void Update()
        {
            foreach (var entity in RegisteredEntities)
            {
                var debugInfo = World.Unpack<DebugInfo>(entity);

                if (World.TryUnpack(entity, out Transform transform))
                {
                    Console.WriteLine($"X: {transform.X} Y {transform.Y} - {debugInfo.Name}");
                }

            }
        }
    }

}
