using System;
using Microsoft.Xna.Framework;

namespace LuxEngine.Systems.Networking
{
    public class AuthClientSystem : BaseSystem<AuthClientSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Connection>();
            signature.RequireSingleton<IsClientSingleton>();
        }

        //protected override void PreUpdate(GameTime gameTime)
        //{
        //    foreach (var entity in RegisteredEntities)
        //    {
        //        var connection = World.Unpack<Connection>(entity);

        //        Handshake(connection.MessagesReceived[NetworkMessage.MessageOneofCase.Handshake], connection);
        //    }
        //}
    }
}
