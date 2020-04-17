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

            // Prepare the received data to receive the next packet's size
            connection.ReceivedData = BitConverter.GetBytes(0);
            connection.ReceivedSize = sizeof(int);

            EndPoint epSender = connection.Endpoint;

            Console.WriteLine("Listening for packets..");
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
                EndPoint epSender = connection.Endpoint;

                // Receive all data
                connection.Socket.EndReceiveFrom(asyncResult, ref epSender);

                // In another system, parse the received data, if it's a login, add the ep
                // as a Connection in a new entity

                // If didn't receive the packet's size yet, it's received now
                if (!connection.DidReceiveSize)
                {
                    connection.ReceivedSize = BitConverter.ToInt32(connection.ReceivedData, 0);
                    connection.ReceivedData = new byte[connection.ReceivedSize];
                    connection.DidReceiveSize = true;
                    Console.WriteLine($"Received size of the next packet: {connection.ReceivedSize}");
                }
                else
                {
                    // Parse the received packet
                    NetworkPacket packet = NetworkPacket.Parser.ParseFrom(connection.ReceivedData);

                    // Add messages to connection's list of received message
                    foreach (NetworkMessage message in packet.Messages)
                    {
                        Console.WriteLine($"Received a '{message.MessageCase.ToString()}'...");
                        connection.Received.Add(message);
                    }

                    // Prepare the received data to receive the next packet's size
                    connection.ReceivedData = BitConverter.GetBytes(0);
                    connection.ReceivedSize = sizeof(int);
                    connection.DidReceiveSize = false;
                }

                // Listen for more packets again
                Console.WriteLine("Listening for packets again..");
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
                // TODO: Handle socket disconnect, unparsable packet, etc.
                Console.WriteLine($"ReceiveData Error: {ex.Message}");
            }
        }
    }
}
