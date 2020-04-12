using System;
using System.Net;
using System.Net.Sockets;

namespace LuxEngine
{
    [Serializable]
    public class ServerInfo : BaseComponent<ServerInfo>
    {
        public IPAddress IPAddress;
        public int Port;

        public ServerInfo(IPAddress ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
        }
    }

    public class ServerSystem : BaseSystem<ServerSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
        }
    }
}
