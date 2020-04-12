using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class ConnectionToServer : BaseComponent<ConnectionToServer>
    {
        [NonSerialized]
        public UdpClient Client;

        public ConnectionToServer(IPEndPoint serverEP)
        {
            Client = new UdpClient();
            Client.Connect(serverEP);
        }
    }

    public class ClientSystem : BaseSystem<ClientSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<ServerInfo>();
            signature.Using<ConnectionToServer>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            if (World.TryUnpack<ConnectionToServer>(entity, out _))
            {
                // There's new server information, replace the current connection
                World.RemoveComponent<ConnectionToServer>(entity);
            }

            var serverInfo = World.Unpack<ServerInfo>(entity);
            World.AddComponent(entity, new ConnectionToServer(serverInfo.Endpoint));

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
                var serverInfo = World.Unpack<ServerInfo>(entity);

                if (World.TryUnpack(entity, out ConnectionToServer connection))
                {
                    byte[] data = Encoding.ASCII.GetBytes("hey");
                    connection.Client.SendAsync(data, data.Length);
                }
            }
        }

        protected override void OnUnregisterEntity(Entity entity)
        {
            if (World.TryUnpack(entity, out ConnectionToServer connection))
            {
                connection.Client.Close();
                World.RemoveComponent<ConnectionToServer>(entity);
            }
        }
    }
}
