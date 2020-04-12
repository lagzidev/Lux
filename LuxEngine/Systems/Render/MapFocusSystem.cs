﻿using Microsoft.Xna.Framework;
using TiledSharp;

namespace LuxEngine
{
    public class MapFocusSystem : BaseSystem<MapFocusSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Camera>();
            signature.RequireSingleton<LoadedMapsSingleton>();
        }

        protected override void PreDraw(GameTime gameTime)
        {
            var loadedMaps = World.UnpackSingleton<LoadedMapsSingleton>();
            TmxMap currentMap = loadedMaps.Maps[loadedMaps.CurrentMapName];

            foreach (var entity in RegisteredEntities)
            {
                var camera = World.Unpack<Camera>(entity);
                camera.Limits = new Rectangle(0, 0, currentMap.Width, currentMap.Height);
            }
        }
    }
}
