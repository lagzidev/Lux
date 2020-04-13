using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class Connection : BaseComponent<Connection>
    {
        public IPAddress IPAddress;
        public int Port;

        [NonSerialized]
        public Socket Socket;

        [NonSerialized]
        public IPEndPoint Endpoint;

        public Connection(IPAddress ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
            Socket = null;
            Endpoint = null;
        }
    }

    /// <summary>
    /// Responsible for creating and closing connections.
    /// </summary>
    public class ConnectionSystem : BaseSystem<ConnectionSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            Connection connection = World.Unpack<Connection>(entity);
            connection.Socket = new Socket(connection.IPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            connection.Endpoint = new IPEndPoint(connection.IPAddress, connection.Port);
        }

        protected override void OnUnregisterEntity(Entity entity)
        {
            Connection connection = World.Unpack<Connection>(entity);
            connection.Socket.Close();
        }
    }
}
