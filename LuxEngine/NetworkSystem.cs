using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class ConnectionToClient : BaseComponent<ConnectionToClient>
    {
        public Socket Socket;
        public IPAddress IPAddress;
        public int Port;

        public ConnectionToClient(IPAddress ipAddress, int port)
        {
            Socket = null;
            IPAddress = ipAddress;
            Port = port;
        }
    }

    public class ConnectionToServer : BaseComponent<ConnectionToServer>
    {
        public Socket Socket;
        public IPAddress ServerIPAddress;
        public int Port;

        // ManualResetEvent instances signal completion.  
        public ManualResetEvent connectDone = new ManualResetEvent(false);
        public ManualResetEvent sendDone = new ManualResetEvent(false);
        public ManualResetEvent receiveDone = new ManualResetEvent(false);

        public ConnectionToServer(IPAddress serverIPAddress, int port)
        {
            Socket = null;
            ServerIPAddress = serverIPAddress;
            Port = port;
        }
    }

    public class NetworkClientSystem : BaseSystem<NetworkClientSystem>
    {
        public NetworkClientSystem() : base(ConnectionToServer.ComponentType)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var entity in RegisteredEntities)
            {
                var connection = World.Unpack<ConnectionToServer>(entity);

                // Initialize connection to server if doesn't exist
                if (null == connection.Socket)
                {
                    IPEndPoint remoteEP = new IPEndPoint(connection.ServerIPAddress, connection.Port);

                    // Create a UDP/IP socket.  
                    connection.Socket = new Socket(
                        connection.ServerIPAddress.AddressFamily,
                        SocketType.Dgram, ProtocolType.Tcp);

                    // Connect to the remote endpoint.  
                    connection.Socket.BeginConnect(
                        remoteEP,
                        new AsyncCallback(ConnectCallback), connection);
                }

                // Poll connection
                if (connection.connectDone.WaitOne(1))
                {
                    Console.WriteLine("Connected");
                }

                //// Send test data to the remote device.  
                //Send(client, "This is a test<EOF>");
                //connection.sendDone.WaitOne();

                //// Receive the response from the remote device.  
                //Receive(client);
                //connection.receiveDone.WaitOne();

                //// Write the response to the console.  
                //Console.WriteLine("Response received : {0}", response);

                // Release the socket.  
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
        }

        public static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var connection = (ConnectionToServer)ar.AsyncState;

                // Complete the connection.  
                connection.Socket.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    connection.Socket.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connection.connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class NetworkServerSystem : BaseSystem<NetworkServerSystem>
    {
        public NetworkServerSystem() : base(ConnectionToClient.ComponentType)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //foreach (var entity in RegisteredEntities)
            //{
            //    var connection = World.Unpack<ConnectionToClient>(entity);

            //}
        }
    }
}
