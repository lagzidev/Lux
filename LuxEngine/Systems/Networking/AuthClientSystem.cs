using System;
using Microsoft.Xna.Framework;

namespace LuxEngine.Systems.Networking
{
    public class AuthClientSystem : ASystem<AuthClientSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.RequireSingleton<IsClientSingleton>();
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
    }
}
