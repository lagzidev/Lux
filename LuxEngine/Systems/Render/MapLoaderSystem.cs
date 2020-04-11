using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace LuxEngine
{
    [Serializable]
    public class Map : BaseComponent<Map>
    {
        public string MapFileName;

        [NonSerialized]
        public TmxMap TmxMap;

        public Map(string mapFileName)
        {
            MapFileName = mapFileName;
            TmxMap = null;
        }
    }

    public class MapLoaderSystem : BaseSystem<MapLoaderSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Map>();
            signature.RequireSingleton<SpriteBatchSingleton>();
            signature.RequireSingleton<LoadedTexturesSingleton>();
        }

        protected override void InitSingleton()
        {
            //World.AddSingletonComponent(new TilesetSingleton());
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();
            Map mapComponent = World.Unpack<Map>(entity);

            string mapFilePath = $"{LuxGame.ContentDirectory}/{HardCodedConfig.DEFAULT_MAPS_FOLDER_NAME}/{mapComponent.MapFileName}.tmx";
            TmxMap map = new TmxMap(mapFilePath);

            // For every used tileset
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                if (!loadedTextures.Textures.ContainsKey(map.Tilesets[i].Name))
                {
                    // Add texture
                    EntityHandle tileset = World.CreateEntity();
                    tileset.AddComponent(new TextureComponent(map.Tilesets[i].Name));
                }

                //int tileWidth = map.Tilesets[i].TileWidth;
                //int tileHeight = map.Tilesets[i].TileHeight;
                //int tilesetTilesWide = tileWidth / map.Tilesets[i].
                //int tilesetTilesHigh = tileHeight / tileHeight;
            }

            mapComponent.TmxMap = map;
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();

            foreach (Entity entity in RegisteredEntities)
            {
                Map map = World.Unpack<Map>(entity);

                foreach (TmxLayer layer in map.TmxMap.Layers)
                {
                    foreach (var tileset in map.TmxMap.Tilesets)
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

                            float x = (i % map.TmxMap.Width) * map.TmxMap.TileWidth;
                            float y = (float)Math.Floor(i / (double)map.TmxMap.Width) * map.TmxMap.TileHeight;

                            Rectangle tilesetRec = new Rectangle((tileset.TileWidth * column) + column + 1, (tileset.TileHeight * row) + row + 1, tileset.TileWidth, tileset.TileHeight);

                            spriteBatch.Draw(
                                loadedTextures.Textures[tileset.Name],
                                new Vector2((int)x, (int)y),
                                tilesetRec,
                                Color.White,
                                0,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                DrawUtils.CalculateSpriteDepth(SpriteDepth.BehindCharacter));
                        }
                    }
                }
            }
        }
    }
}
