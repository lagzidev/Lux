using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class ConnectionToServer : BaseComponent<ConnectionToServer>
    {
    }

    /// <summary>
    /// Responsible for managing a client connected to a server
    /// </summary>
    public class ClientSystem : BaseSystem<ClientSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.Require<ConnectionToServer>();
            signature.RequireSingleton<InputSingleton>();
        }

        protected override void LoadFrame(GameTime gameTime)
        {
            var input = World.UnpackSingleton<InputSingleton>();

            //LuxMessage luxMessage = new LuxMessage();

            //foreach (var entity in RegisteredEntities)
            //{
            //    var connection = World.Unpack<Connection>(entity);

            //    using (MemoryStream memStream = new MemoryStream())
            //    {
            //        Serializer.Serialize(memStream, input);
            //        byte[] data = memStream.ToArray();
            //        connection.Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, connection.Endpoint, new AsyncCallback(SendCallback), connection);
            //    }
            //}
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Connection connection = (Connection)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = connection.Socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
