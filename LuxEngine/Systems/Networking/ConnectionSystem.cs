using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using LuxProtobuf;
using System.Collections.Generic;

namespace LuxEngine
{
    [Serializable]
    public class Connection : AComponent<Connection>
    {
        public IPAddress IPAddress;
        public int Port;

        [NonSerialized]
        public Socket Socket;

        [NonSerialized]
        public IPEndPoint Endpoint;

        [NonSerialized]
        public byte[] ReceivedData;

        [NonSerialized]
        public int ReceivedSize;

        [NonSerialized]
        public Queue<NetworkMessage> MessagesToSend;

        [NonSerialized]
        public NetworkMessages MessagesReceived;

        [NonSerialized]
        public ConnectionState ConnectionState;

        [NonSerialized]
        public int ProtocolVersion;

        public Connection(IPAddress ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
            Socket = null;
            Endpoint = null;
            ReceivedSize = 0;
            ReceivedData = null;
            MessagesToSend = new Queue<NetworkMessage>();
            MessagesReceived = new NetworkMessages();
            ConnectionState = ConnectionState.Status;
            ProtocolVersion = 0;
        }
    }

    public enum ConnectionState
    {
        Disconnecting, // Indicates that the server wants to disconnect right after sending the final message
        Disconnected, // Indicates that the connection is disconnected and can safely be removed
        Status,
        Handshaking,
        LoggingIn,
        Play
    }

    public class NetworkMessages
    {
        private Dictionary<NetworkMessage.MessageOneofCase, Queue<NetworkMessage>> _messages;

        public NetworkMessages()
        {
            _messages = new Dictionary<NetworkMessage.MessageOneofCase, Queue<NetworkMessage>>();
        }

        public Queue<NetworkMessage> this[NetworkMessage.MessageOneofCase i]
        {
            get
            {
                if (!_messages.ContainsKey(i))
                {
                    _messages.Add(i, new Queue<NetworkMessage>());
                }

                return _messages[i];
            }
        }

        public void Add(NetworkMessage message)
        {
            this[message.MessageCase].Enqueue(message);
        }
    }

    /// <summary>
    /// Responsible for creating and closing connections.
    /// </summary>
    public class ConnectionSystem : ASystem<ConnectionSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
        }

        protected override void LoadFrame()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Connection connection);
                if (connection.ConnectionState == ConnectionState.Disconnected)
                {
                    // TODO: Make sure this is OKAY to do here
                    _world.DestroyEntity(entity);
                }
            }
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            Unpack(entity, out Connection connection);

            // Initialize connection's socket and endpoint
            connection.Socket = new Socket(connection.IPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            connection.Endpoint = new IPEndPoint(connection.IPAddress, connection.Port);
        }

        protected override void OnUnregisterEntity(Entity entity)
        {
            Unpack(entity, out Connection connection);
            connection.Socket.Close();
        }
    }
}
