//using System;
//using System.Collections.Generic;
//using LuxProtobuf;
//using Microsoft.Xna.Framework;
//using LuxEngine.ECS;

//namespace LuxEngine.ECS
//{
//    public class AuthClientSystem : ASystem<AuthClientSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Connection>();
//            signature.RequireSingleton<IsClientSingleton>();
//        }

//        protected override void OnRegisterEntity(Entity entity)
//        {
//            Unpack(entity, out Connection connection);
//            connection.MessagesToSend.Enqueue(new NetworkMessage()
//            {
//                Handshake = new Handshake()
//                {
//                    ProtocolVersion = HardCodedConfig.PROTOCOL_VERSION
//                }
//            });
//        }

//        public override void PreUpdate()
//        {
//            foreach (var entity in RegisteredEntities)
//            {
//                Unpack(entity, out Connection connection);

//                // Handle in case the message was received
//                HandshakeResponse(connection.Received[NetworkMessage.MessageOneofCase.HandshakeResponse], connection);
//            }
//        }

//        private void HandshakeResponse(Queue<NetworkMessage> receivedMessages, Connection connection)
//        {
//            while (receivedMessages.Count > 0)
//            {
//                NetworkMessage message = receivedMessages.Dequeue();

//                switch (message.HandshakeResponse.Status)
//                {
//                    case Status.Success: // TODO: CHANGE THIS TO THE RIGHT STATUS
//                        LuxGame.Error = new GameError(
//                            message.HandshakeResponse.Status,
//                            "Server is using a different game protocol version.");
//                        break;

//                    default:
//                        LuxCommon.Assert(false); // Status isn't handled
//                        break;
//                }

//            }
//        }
//    }
//}
