using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using LuxProtobuf;
using System.IO;
using Google.Protobuf;
using System.Net;

namespace LuxEngine
{
    public class PacketSenderSystem : ASystem<PacketSenderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
        }

        protected override void LoadFrame()
        {
            // For each connection
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Connection connection);

                if (connection.MessagesToSend.Count == 0)
                {
                    continue;
                }

                // Empty MessagesToSend into the packet
                NetworkPacket networkPacket = new NetworkPacket();
                while (connection.MessagesToSend.Count > 0)
                {
                    networkPacket.Messages.Add(connection.MessagesToSend.Dequeue());
                }

                // Serialize packet
                byte[] dataToSend = SerializePacket(networkPacket);

                // Send packet
                connection.Socket.BeginSendTo(
                    dataToSend,
                    0,
                    dataToSend.Length,
                    SocketFlags.None,
                    connection.Endpoint,
                    new AsyncCallback(SendCallback),
                    connection);
            }
        }

        /// <summary>
        /// Serializes a network packet into a byte array.
        /// </summary>
        /// <param name="packet">The packet to serialize</param>
        /// <returns>The packet in bytes</returns>
        private byte[] SerializePacket(NetworkPacket packet)
        {
            // Serialize packet
            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                // Write the size of the packet
                int size = packet.CalculateSize();
                memoryStream.Write(BitConverter.GetBytes(size), 0, sizeof(int));

                // Write the packet
                packet.WriteTo(memoryStream);

                // Return the data written to the memory stream
                data = memoryStream.ToArray();
            }

            return data;
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the connection from the state object.  
                Connection connection = (Connection)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = connection.Socket.EndSendTo(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to endpoint.");

                // If connection is disconnecting, change it to disconnected
                // as we just sent the last packet
                if (connection.ConnectionState == ConnectionState.Disconnecting)
                {
                    connection.ConnectionState = ConnectionState.Disconnected;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
