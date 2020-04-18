//using System;
//using System.Net;
//using LuxProtobuf;
//using Microsoft.Xna.Framework;
//using LuxEngine.ECS;

//namespace LuxEngine.Systems
//{
//    [Serializable]
//    public class IsClientSingleton : AComponent<IsClientSingleton>
//    {
//    }

//    /// <summary>
//    /// Temporary system TODO: remove this temp system
//    /// </summary>
//    public class ClientSystem : ASystem<ClientSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Connection>();
//            signature.RequireSingleton<IsClientSingleton>();
//            signature.RequireSingleton<InputSingleton>();
//        }

//        public override void InitSingleton()
//        {
//            // Set the world to be a client
//            AddSingletonComponent(new IsClientSingleton());
//        }

//        public override void PreUpdate()
//        {
//            UnpackSingleton(out InputSingleton input);

//            // TODO: We only support one connection now, support more?
//            if (input.F4KeyPress && RegisteredEntities.Count == 0)
//            {
//                // Connect to a server
//                Entity connection = CreateEntity();
//                AddComponent(connection, new Connection(IPAddress.Parse("127.0.0.1"), 1337));
//            }
//        }

//        //protected override void OnRegisterEntity(Entity entity)
//        //{
//        //    var connection = World.Unpack<Connection>(entity);

//        //    // Send a connect message
//        //    connection.MessagesToSend.Enqueue(new NetworkMessage()
//        //    {
//        //        Login = new Login()
//        //        {
//        //            Username = "Lagzi",
//        //            Password = "123456"
//        //        }
//        //    });
//        //}

//        //public override void LoadFrame(GameTime gameTime)
//        //{
//        //    var input = World.UnpackSingleton<InputSingleton>();
//        //    var messagesSingleton = World.UnpackSingleton<NetworkMessagesSingleton>();

//        //    // Send a message to the server indicating that the user logged into the server
//        //    NetworkMessage message = new NetworkMessage
//        //    {
//        //        Input = new Input
//        //        {
//        //            Right = true
//        //        }
//        //    };

//        //    messagesSingleton.MessagesToSend.Enqueue(message);
//        //}
//    }
//}
