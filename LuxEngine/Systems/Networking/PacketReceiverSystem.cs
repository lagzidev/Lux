using System;
using System.Net;
using System.Net.Sockets;
using LuxProtobuf;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    /// <summary>
    /// Indicates that the world is a server
    /// </summary>
    //[Serializable]
    //public class Server : BaseComponent<Server>
    //{

    //}

    public class PacketReceiverSystem : ASystem<PacketReceiverSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            Unpack(entity, out Connection connection);

            // Initialize connection's socket and endpoint
            //connection.Socket = new Socket(connection.IPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //connection.Endpoint = new IPEndPoint(connection.IPAddress, connection.Port);
            //connection.Socket.Bind(connection.Endpoint);

            // Initialise the IPEndPoint for the clients
            //IPEndPoint clients = new IPEndPoint(IPAddress.Any, 1337);
            EndPoint epSender = connection.Endpoint;

            // Prepare the received data to receive the next packet's size
            connection.ReceivedData = BitConverter.GetBytes(0);
            connection.ReceivedSize = sizeof(int);

            connection.Socket.BeginReceiveFrom(
                connection.ReceivedData,
                0,
                connection.ReceivedSize,
                SocketFlags.None,
                ref epSender,
                new AsyncCallback(ReceiveData),
                connection);
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                Connection connection = (Connection)asyncResult.AsyncState;

                // Initialise the IPEndPoint for the clients
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = clients;

                // Receive all data
                connection.Socket.EndReceiveFrom(asyncResult, ref epSender);

                // In another system, parse the received data, if it's a login, add the ep
                // as a Connection in a new entity

                // If didn't receive the packet's size yet, it's received now
                if (0 == connection.ReceivedSize)
                {
                    connection.ReceivedSize = BitConverter.ToInt32(connection.ReceivedData, 0);
                    connection.ReceivedData = new byte[connection.ReceivedSize];
                }
                else
                {
                    // Parse the received packet
                    NetworkPacket packet = NetworkPacket.Parser.ParseFrom(connection.ReceivedData);

                    // Add messages to connection's list of received message
                    foreach (NetworkMessage message in packet.Messages)
                    {
                        connection.MessagesReceived.Add(message);
                    }

                    // Prepare the received data to receive the next packet's size
                    connection.ReceivedData = BitConverter.GetBytes(0);
                    connection.ReceivedSize = sizeof(int);
                }

                // Listen for more packets again
                connection.Socket.BeginReceiveFrom(
                    connection.ReceivedData,
                    0,
                    connection.ReceivedSize,
                    SocketFlags.None,
                    ref epSender,
                    new AsyncCallback(ReceiveData),
                    connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReceiveData Error: {ex.Message}");
            }
        }
    }
}
