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

    public class AuthServerSystem : ASystem<AuthServerSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.RequireSingleton<IsServerSingleton>();

            //signature.RequireSingleton<UsersSingleton>();
        }

        public override void PreUpdate()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Connection connection);

                // Handle in case the message was received
                Handshake(connection.Received[NetworkMessage.MessageOneofCase.Handshake], connection);
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

                Status status = Status.Success;
                connection.ConnectionState = ConnectionState.Handshaking;

                // If protocol versions aren't matching, set the status and state
                if (message.Handshake.ProtocolVersion != HardCodedConfig.PROTOCOL_VERSION)
                {
                    status = Status.NonMatchingProtocolVersions;
                    connection.ConnectionState = ConnectionState.Disconnecting;
                }

                // Send the response
                connection.MessagesToSend.Enqueue(new NetworkMessage()
                {
                    HandshakeResponse = new HandshakeResponse()
                    {
                        Status = status
                    }
                });
            }
        }

        /*
         * There are a few problems. The first is that to know the order
         * of the systems we need to check both their registration order
         * and their phase of execution. This sucks. I want some interface
         * that if I take one look at I know the order of all systems by catagory
         * (because I don't care about textures when I'm debugging networking).
         * Furthermore, I want to control that order from that centralized place
         * in order to prevent bugs related to the order.
         */

        //private void LoginStart(LoginStart loginStart)
        //{
        //    UnpackSingleton(out UsersSingleton users);
        //}
    }
}
