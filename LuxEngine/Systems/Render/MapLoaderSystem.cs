using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace LuxEngine
{
    [Serializable]
    public class LoadedMapsSingleton : AComponent<LoadedMapsSingleton>
    {
        [NonSerialized]
        public Dictionary<string, TmxMap> Maps;
        public string CurrentMapName;

        public LoadedMapsSingleton()
        {
            Maps = new Dictionary<string, TmxMap>();
            CurrentMapName = null;
        }
    }

    [Serializable]
    public class Map : AComponent<Map>
    {
        public string MapName;

        public Map(string mapName)
        {
            MapName = mapName;
        }
    }

    public class MapLoaderSystem : ASystem<MapLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Map>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
            signature.RequireSingleton<LoadedMapsSingleton>();
        }

        protected override void InitSingleton()
        {
            _world.AddSingletonComponent(new LoadedMapsSingleton());
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var loadedTextures = _world.UnpackSingleton<LoadedTexturesSingleton>();
            var loadedMaps = _world.UnpackSingleton<LoadedMapsSingleton>();
            string mapName = _world.Unpack<Map>(entity).MapName;

            string mapFilePath = $"{LuxGame.ContentDirectory}/{HardCodedConfig.DEFAULT_MAPS_FOLDER_NAME}/{mapName}.tmx";
            TmxMap map = new TmxMap(mapFilePath);

            // For every used tileset
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                if (!loadedTextures.Textures.ContainsKey(map.Tilesets[i].Name))
                {
                    // Add texture
                    EntityHandle tileset = _world.CreateEntity();
                    tileset.AddComponent(new TextureComponent(map.Tilesets[i].Name));
                }
            }

            loadedMaps.Maps.Add(mapName, map);
            loadedMaps.CurrentMapName = mapName;
        }

        protected override void Draw()
        {
            SpriteBatch spriteBatch = _world.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            var loadedTextures = _world.UnpackSingleton<LoadedTexturesSingleton>();
            var loadedMaps = _world.UnpackSingleton<LoadedMapsSingleton>();

            foreach (Entity entity in RegisteredEntities)
            {
                string mapName = _world.Unpack<Map>(entity).MapName;
                TmxMap map = loadedMaps.Maps[mapName];

                foreach (TmxLayer layer in map.Layers)
                {
                    foreach (var tileset in map.Tilesets)
                    {
                        for (int i = 0; i < layer.Tiles.Count; i++)
                        {
                            int gid = layer.Tiles[i].Gid;
                            if (gid == 0 || gid < tileset.FirstGid)
                            {
                                continue;
                            }

                            int tileFrame = gid - 1;
                            int tilesetTilesWide = loadedTextures.Textures[tileset.Name].Width / tileset.TileWidth;
                            int column = tileFrame % tilesetTilesWide;
                            int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                            float x = (i % map.Width) * map.TileWidth;
                            float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                            Rectangle tilesetRec = new Rectangle((tileset.TileWidth * column) + column + 1, (tileset.TileHeight * row) + row + 1, tileset.TileWidth, tileset.TileHeight);

                            spriteBatch.Draw(
                                loadedTextures.Textures[tileset.Name],
                                new Vector2((int)x, (int)y),
                                tilesetRec,
                                Color.White,
                                0,
                                Vector2.Zero,
                                new Vector2(1, 1),
                                SpriteEffects.None,
                                DrawUtils.CalculateSpriteDepth(SpriteDepth.BehindCharacter));
                        }
                    }
                }
            }
        }
    }
}
