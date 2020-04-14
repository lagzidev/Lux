using System;
using System.Net;
using LuxProtobuf;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    [Serializable]
    public class IsClientSingleton : BaseComponent<IsClientSingleton>
    {
    }

    /// <summary>
    /// Temporary system TODO
    /// </summary>
    public class ClientSystem : BaseSystem<ClientSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.RequireSingleton<IsClientSingleton>();
            //signature.RequireSingleton<InputSingleton>();
        }

        protected override void InitSingleton()
        {
            // Set the world to be a client
            World.AddSingletonComponent(new IsClientSingleton());
        }

        protected override void Init()
        {
            // Connect to a server
            EntityHandle connection = World.CreateEntity();
            connection.AddComponent(new Connection(IPAddress.Parse("127.0.0.1"), 1337));
        }

        //protected override void OnRegisterEntity(Entity entity)
        //{
        //    var connection = World.Unpack<Connection>(entity);

        //    // Send a connect message
        //    connection.MessagesToSend.Enqueue(new NetworkMessage()
        //    {
        //        Login = new Login()
        //        {
        //            Username = "Lagzi",
        //            Password = "123456"
        //        }
        //    });
        //}

        //protected override void LoadFrame(GameTime gameTime)
        //{
        //    var input = World.UnpackSingleton<InputSingleton>();
        //    var messagesSingleton = World.UnpackSingleton<NetworkMessagesSingleton>();

        //    // Send a message to the server indicating that the user logged into the server
        //    NetworkMessage message = new NetworkMessage
        //    {
        //        Input = new Input
        //        {
        //            Right = true
        //        }
        //    };

        //    messagesSingleton.MessagesToSend.Enqueue(message);
        //}
    }
}
