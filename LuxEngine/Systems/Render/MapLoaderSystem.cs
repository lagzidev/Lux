using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace LuxEngine
{
    public class Map : BaseComponent<Map>
    {
        public string MapFileName;

        public Map(string mapFileName)
        {
            MapFileName = mapFileName;
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
        protected override void OnRegisterEntity(Entity entity)
        {
            SpriteBatch spriteBatch = World.UnpackSingleton<SpriteBatchSingleton>().SpriteBatch;
            var loadedTextures = World.UnpackSingleton<LoadedTexturesSingleton>();
            Map mapComponent = World.Unpack<Map>(entity);

            string mapFilePath = $"{World.ContentManager.RootDirectory}/{HardCodedConfig.DEFAULT_MAPS_FOLDER_NAME}/{mapComponent.MapFileName}.tmx";
            TmxMap map = new TmxMap(mapFilePath);

            // For every used tileset
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                // Add texture
                Texture2D texture = TextureLoader.Load(map.Tilesets[i].Name, World.ContentManager);
                loadedTextures.Textures.Add(map.Tilesets[i].Name, texture);

                //int tileWidth = map.Tilesets[i].TileWidth;
                //int tileHeight = map.Tilesets[i].TileHeight;
                //int tilesetTilesWide = tileWidth / map.Tilesets[i].
                //int tilesetTilesHigh = tileHeight / tileHeight;

            }


            foreach (TmxLayer layer in map.Layers)
            {
                for (int i = 0; i < layer.Tiles.Count; i++)
                {
                    int gid = layer.Tiles[i].Gid;
                    if (gid == 0)
                    {
                        continue;
                    }

                    //int tileFrame = gid - 1;
                    //int column = tileFrame % tilesetTilesWide;
                    //int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                    //float x = (i % map.Width) * map.TileWidth;
                    //float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                    //Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    //spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                }
            }
        }
    }
}
