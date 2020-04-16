using System;
using System.Collections.Generic;
using LuxProtobuf;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class User
    {
        public string Username;
        public string Password;
    }

    [Serializable]
    public class UsersSingleton : AComponent<UsersSingleton>
    {
        /// <summary>
        /// string = Username
        /// </summary>
        public Dictionary<string, User> PendingLoginUsers;
        public Dictionary<string, User> LoggedInUsers;
        public Dictionary<string, User> RegisteredUsers;
    }

    public class UserServerSystem : ASystem<UserServerSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.RequireSingleton<IsServerSingleton>();

            //signature.RequireSingleton<UsersSingleton>();
        }

        protected override void PreUpdate()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Connection connection);

                // Handle in case the message was received
                Handshake(connection.MessagesReceived[NetworkMessage.MessageOneofCase.Handshake], connection);
            }
        }

        private void Handshake(Queue<NetworkMessage> receivedMessages, Connection connection)
        {
            while (receivedMessages.Count > 0)
            {
                NetworkMessage message = receivedMessages.Dequeue();

                connection.ProtocolVersion = message.Handshake.ProtocolVersion;

                // TODO: Support non-matching protocol versions (client must be at least server's version)
                // TODO: Handle loss of the Handshake or HandshakeResponse packet, and similar losses
                // that may cause a deadlock/timeout.

                // If protocol versions aren't matching, send an error message
                if (message.Handshake.ProtocolVersion != HardCodedConfig.PROTOCOL_VERSION)
                {
                    connection.MessagesToSend.Enqueue(new NetworkMessage()
                    {
                        HandshakeResponse = new HandshakeResponse()
                        {
                            Status = Status.NonMatchingProtocolVersions
                        }
                    });

                    connection.ConnectionState = ConnectionState.Disconnecting;
                    continue;
                }

                connection.ConnectionState = ConnectionState.Handshaking;
            }
        }

        //private void LoginStart(LoginStart loginStart)
        //{
        //    UnpackSingleton(out UsersSingleton users);
        //}
    }
}
