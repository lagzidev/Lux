//using Microsoft.Xna.Framework;
//using TiledSharp;
//using LuxEngine.ECS;

//namespace LuxEngine.Systems
//{
//    public class MapFocusSystem : ASystem<MapFocusSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Camera>();
//            signature.RequireSingleton<LoadedMapsSingleton>();
//        }

//        //public override void PreDraw(GameTime gameTime)
//        //{
//        //    var loadedMaps = World.UnpackSingleton<LoadedMapsSingleton>();
//        //    TmxMap currentMap = loadedMaps.Maps[loadedMaps.CurrentMapName];

//        //    foreach (var entity in RegisteredEntities)
//        //    {
//        //        var camera = World.Unpack<Camera>(entity);
//        //        camera.Limits = new Rectangle(0, 0, currentMap.Width, currentMap.Height);
//        //    }
//        //}
//    }
//}
