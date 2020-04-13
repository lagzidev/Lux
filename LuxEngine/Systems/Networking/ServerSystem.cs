using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class ConnectionToClient : BaseComponent<ConnectionToClient>
    {
    }

    public class ServerSystem : BaseSystem<ServerSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.Require<ConnectionToClient>();
        }

        protected override void LoadFrame(GameTime gameTime)
        {
        }
    }
}
