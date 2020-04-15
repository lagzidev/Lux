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
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<DebugInfo>();
        }

        protected override void Update()
        {
            foreach (var entity in RegisteredEntities)
            {
                var debugInfo = _world.Unpack<DebugInfo>(entity);

                if (_world.TryUnpack(entity, out Transform transform))
                {
                    Console.WriteLine($"X: {transform.X} Y {transform.Y} - {debugInfo.Name}");
                }

            }
        }
    }

}
