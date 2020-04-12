using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class PendingConnection : BaseComponent<PendingConnection>
    {
        [NonSerialized]
        public Socket Socket;

        [NonSerialized]
        public ManualResetEvent ConnectDone;

        public PendingConnection(Socket socket)
        {
            Socket = socket;
            ConnectDone = new ManualResetEvent(false);
        }
    }

    [Serializable]
    public class Connection : BaseComponent<Connection>
    {
        [NonSerialized]
        public Socket Socket;

        [NonSerialized]
        public ManualResetEvent SendDone;

        [NonSerialized]
        public ManualResetEvent ReceiveDone;

        public Connection(Socket socket)
        {
            Socket = socket;
            SendDone = new ManualResetEvent(false);
            ReceiveDone = new ManualResetEvent(false);
        }
    }

    public class ClientSystem : BaseSystem<ClientSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<ServerInfo>();
            signature.Using<PendingConnection>();
            signature.Using<Connection>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            if (World.TryUnpack<Connection>(entity, out _))
            {
                // There's new server information, replace the current connection
                World.RemoveComponent<Connection>(entity);
            }

            if (World.TryUnpack<PendingConnection>(entity, out _))
            {
                // There's new server information, replace the current pending connection
                World.RemoveComponent<PendingConnection>(entity);
            }

            var serverInfo = World.Unpack<ServerInfo>(entity);

            // Create the server endpoint
            IPEndPoint remoteEP = new IPEndPoint(serverInfo.IPAddress, serverInfo.Port);

            // Create a UDP/IP socket.  
            Socket socket = new Socket(serverInfo.IPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // Add a pending connection component
            PendingConnection pendingConnection = new PendingConnection(socket);
            World.AddComponent(entity, pendingConnection);

            // Connect to the remote endpoint.
            socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), pendingConnection);

            //// Receive the response from the remote device.  
            //Receive(client);
            //connection.receiveDone.WaitOne();

            //// Write the response to the console.  
            //Console.WriteLine("Response received : {0}", response);
        }

        protected override void PreUpdate(GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {
                if (World.TryUnpack(entity, out PendingConnection pendingConnection))
                {
                    // Poll connection
                    if (pendingConnection.ConnectDone.WaitOne(1))
                    {
                        // If connected, switch the pending connection with a connection
                        World.AddComponent(entity, new Connection(pendingConnection.Socket));
                        World.RemoveComponent<PendingConnection>(entity);
                    }
                }

                if (World.TryUnpack(entity, out Connection connection))
                {
                    Send(connection, "This is a test<EOF>");
                    //connection.sendDone.WaitOne();
                }
            }
        }

        private static void Send(Connection connection, string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            connection.Socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), connection);
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

                // Signal that all bytes have been sent.  
                connection.SendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected override void OnUnregisterEntity(Entity entity)
        {
            if (World.TryUnpack(entity, out Connection connection))
            {
                // Shutdown the socket
                connection.Socket.Shutdown(SocketShutdown.Both);
                connection.Socket.Close();

                World.RemoveComponent<Connection>(entity);

            }

            if (World.TryUnpack(entity, out PendingConnection pendingConnection))
            {
                // Close the pending connection, expect an exception in the callback method
                pendingConnection.Socket.Close();
                World.RemoveComponent<PendingConnection>(entity);
            }
        }

        public static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var pendingConnection = (PendingConnection)ar.AsyncState;

                // Complete the connection.
                // This throws an exception if the socket was closed before connecting
                pendingConnection.Socket.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", pendingConnection.Socket.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                pendingConnection.ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
