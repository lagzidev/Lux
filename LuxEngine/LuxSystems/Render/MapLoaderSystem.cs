//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using TiledSharp;
//using LuxEngine.ECS;

//namespace LuxEngine.ECS
//{
//    [Serializable]
//    public class LoadedMapsSingleton : AComponent<LoadedMapsSingleton>
//    {
//        [NonSerialized]
//        public Dictionary<string, TmxMap> Maps;
//        public string CurrentMapName;

//        public LoadedMapsSingleton()
//        {
//            Maps = new Dictionary<string, TmxMap>();
//            CurrentMapName = null;
//        }
//    }

//    [Serializable]
//    public class Map : AComponent<Map>
//    {
//        public string MapName;

//        public Map(string mapName)
//        {
//            MapName = mapName;
//        }
//    }

//    public class MapLoaderSystem : ASystem<MapLoaderSystem>
//    {
//        public override void SetSignature(SystemSignature signature)
//        {
//            signature.Require<Map>();
//            signature.RequireSingleton<SpriteBatchSingleton>();
//            signature.RequireSingleton<LoadedTexturesSingleton>();
//            signature.RequireSingleton<LoadedMapsSingleton>();
//        }

//        public override void InitSingleton()
//        {
//            AddSingletonComponent(new LoadedMapsSingleton());
//        }

//        protected override void OnRegisterEntity(Entity entity)
//        {
//            UnpackSingleton(out LoadedTexturesSingleton loadedTextures);
//            UnpackSingleton(out LoadedMapsSingleton loadedMaps);
//            Unpack(entity, out Map map);

//            string mapFilePath = $"{LuxGame.ContentDirectory}/{HardCodedConfig.DEFAULT_MAPS_FOLDER_NAME}/{map.MapName}.tmx";
//            TmxMap tmxMap = new TmxMap(mapFilePath);

//            // For every used tileset
//            for (int i = 0; i < tmxMap.Tilesets.Count; i++)
//            {
//                if (!loadedTextures.Textures.ContainsKey(tmxMap.Tilesets[i].Name))
//                {
//                    // Add texture
//                    Entity tileset = CreateEntity();
//                    AddComponent(tileset, new TextureComponent(tmxMap.Tilesets[i].Name));
//                }
//            }

//            loadedMaps.Maps.Add(map.MapName, tmxMap);
//            loadedMaps.CurrentMapName = map.MapName;
//        }

//        public override void Draw()
//        {
//            UnpackSingleton(out SpriteBatchSingleton spriteBatchSingleton);
//            SpriteBatch spriteBatch = spriteBatchSingleton.Batch;

//            UnpackSingleton(out LoadedTexturesSingleton loadedTextures);
//            UnpackSingleton(out LoadedMapsSingleton loadedMaps);

//            foreach (Entity entity in RegisteredEntities)
//            {
//                Unpack(entity, out Map map);
//                TmxMap tmxMap = loadedMaps.Maps[map.MapName];

//                foreach (TmxLayer layer in tmxMap.Layers)
//                {
//                    foreach (var tileset in tmxMap.Tilesets)
//                    {
//                        for (int i = 0; i < layer.Tiles.Count; i++)
//                        {
//                            int gid = layer.Tiles[i].Gid;
//                            if (gid == 0 || gid < tileset.FirstGid)
//                            {
//                                continue;
//                            }

//                            int tileFrame = gid - 1;
//                            int tilesetTilesWide = loadedTextures.Textures[tileset.Name].Width / tileset.TileWidth;
//                            int column = tileFrame % tilesetTilesWide;
//                            int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

//                            float x = (i % tmxMap.Width) * tmxMap.TileWidth;
//                            float y = (float)Math.Floor(i / (double)tmxMap.Width) * tmxMap.TileHeight;

//                            Rectangle tilesetRec = new Rectangle((tileset.TileWidth * column) + column + 1, (tileset.TileHeight * row) + row + 1, tileset.TileWidth, tileset.TileHeight);

//                            spriteBatch.Draw(
//                                loadedTextures.Textures[tileset.Name],
//                                new Vector2((int)x, (int)y),
//                                tilesetRec,
//                                Color.White,
//                                0,
//                                Vector2.Zero,
//                                new Vector2(1, 1),
//                                SpriteEffects.None,
//                                DrawUtils.CalculateSpriteDepth(SpriteDepth.BehindCharacter));
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
