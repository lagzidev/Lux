using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class IsServerSingleton : AComponent<IsServerSingleton>
    {
    }

    /// <summary>
    /// Temporary system TODO
    /// </summary>
    public class ServerSystem : ASystem<ServerSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Using<Connection>();
            signature.RequireSingleton<IsServerSingleton>();
        }

        protected override void InitSingleton()
        {
            // Set the world to be a server
           AddSingletonComponent(new IsServerSingleton());
        }

        protected override void Init()
        {
            // Set a connection to any IP addresses
            Entity connection = CreateEntity();
            
            AddComponent(connection, new Connection(IPAddress.Any, 1337));
            // TODO: Figure out how to randomize the client port (to avoid collisions in the same network)
        }
    }
}
