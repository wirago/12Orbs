using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Orbs.LootSystem;
using Orbs.World;
using Orbs.Animations;
using System.Linq;
using Penumbra;
using System.Collections.ObjectModel;
using System.Globalization;
using Orbs.Effects;

namespace Orbs.Tiled
{
    public class TileMap
    {
        TmxMap curMap;
        Texture2D tileset;

        private const int TILEWIDTH_BASE = 64;
        private const int TILEHEIGHT_BASE = 64;
        private int tilesetTilesWide;
        private int tilesetTilesHigh;
        private int curFirstGid;

        private Dictionary<int, SpriteAnimation> animatedTiles = new Dictionary<int, SpriteAnimation>();

        public int worldWidth;
        public int worldHeight;

        public Vector2 playerSpawnPosition;

        public static List<Rectangle> collisionObjects = new List<Rectangle>();
        public static List<Rectangle> lootableObjects = new List<Rectangle>();
        //public static List<Rectangle> transits = new List<Rectangle>();
        public static bool[,] tileMapWalkable;
        public static Pathfinding.Grid pathfindGrid;

        public static Dictionary<int, Texture2D> tileSets = new Dictionary<int, Texture2D>();
        public Dictionary<string, Vector2> transitionTags = new Dictionary<string, Vector2>();

        public TileMap(string mapSource)
        {
            curMap = new TmxMap(mapSource);
            tileSets.Clear();

            foreach(TmxTileset tileSet in curMap.Tilesets)
            {
                string imgSrc = tileSet.Image.Source.Substring(tileSet.Image.Source.IndexOf("../") + 3);
                string imgName = imgSrc.Remove(imgSrc.Length - 4); //Removes .png .jpg etc

                tileSets.Add(tileSet.FirstGid, Orbs.content.Load<Texture2D>(imgName));

                if (tileSet.Properties.ContainsKey("isAnimation"))
                    GetTileAnimation(tileSet);
            }       

            worldWidth = curMap.Width * TILEWIDTH_BASE;
            worldHeight = curMap.Height * TILEHEIGHT_BASE;

            if(curMap.Properties.ContainsKey("playerSpawnPosX"))
            {
                playerSpawnPosition.X = Int32.Parse(curMap.Properties["playerSpawnPosX"]);
                playerSpawnPosition.Y = Int32.Parse(curMap.Properties["playerSpawnPosY"]);
            }

            if (curMap.Properties.ContainsKey("ambient"))
            {
                string hexColor = curMap.Properties["ambient"];
                byte r = byte.Parse(hexColor.Substring(3, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hexColor.Substring(5, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hexColor.Substring(7, 2), NumberStyles.HexNumber);
                byte a = byte.Parse(hexColor.Substring(1, 2), NumberStyles.HexNumber);
                
                Color _color = new Color(r, g, b, a);

                WorldObjects.ChangeAmbienteColor(_color);
            }

            foreach (var o in curMap.ObjectGroups["AnimatedObjects"].Objects)
            {
                if(o.Properties["isLootable"] == "true")
                    WorldObjects.animatedObjects.Add(new ObjectAnimation(o.Properties["source"],
                        o.Properties["source"], (int)o.X, (int)o.Y, 
                        Int32.Parse(o.Properties["width"]), Int32.Parse(o.Properties["height"]), o.Name, o.Type, o.Properties["icon"]));
                else
                    WorldObjects.animatedObjects.Add(new Animations.ObjectAnimation(
                        o.Properties["source"], (int)o.X, (int)o.Y,
                        Int32.Parse(o.Properties["width"]), Int32.Parse(o.Properties["height"]), o.Name, o.Type));
            }
                

            foreach (var o in curMap.ObjectGroups["Collision"].Objects)
                collisionObjects.Add(new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height));

            foreach (var o in curMap.ObjectGroups["Interact"].Objects)
                WorldObjects.interactionObjects.Add(o.Name, new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height));

            foreach (var o in curMap.ObjectGroups["Container"].Objects)
                WorldObjects.lootableContainers.Add(new ItemContainer(new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height), o.Id, o.Properties["opened"], o.Properties["closed"]));

            foreach (var o in curMap.ObjectGroups["MapChange"].Objects)
            {
                WorldObjects.transits.Add(
                new Transit(new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height),
                o.Properties["destination"], o.Properties["destinationName"],
                o.Properties["transitionTag"]));

            }

            foreach (var o in curMap.ObjectGroups["EnterPoints"].Objects)
            {
                if (o.Properties.ContainsKey("transitionTag"))
                {
                    transitionTags.Add(o.Properties["transitionTag"], new Vector2((float)o.X, (float)o.Y));
                }
                else
                {
                    //Error Message
                }
            }

            foreach (var o in curMap.ObjectGroups["Enemies"].Objects)
                WorldCharacters.GenerateWorldEnemy(o, EnemyTable.enemyTemplates.Where(x => x.uid == o.Type).First());

            foreach (var o in curMap.ObjectGroups["NPC"].Objects)
                WorldCharacters.GenerateWorldNPC(o);

            EffectManager.lightning.effectComponent.Lights.AddRange(GetLightsFromMap());

            TileMap.tileMapWalkable = new bool[curMap.Width, curMap.Height];
            InitTilesMap();
            TileMap.pathfindGrid = new Pathfinding.Grid(curMap.Width, curMap.Height, tileMapWalkable);
        }

        private void InitTilesMap()
        {
            Rectangle curTileRect = new Rectangle(0, 0, curMap.TileWidth, curMap.TileHeight); //first tile top left corner

            for(int y = 0; y < curMap.Height; y++)
            {
                curTileRect.Y = y * curMap.TileHeight;

                for(int x = 0; x < curMap.Width; x++)
                {
                    curTileRect.X = x * curMap.TileWidth;

                    foreach (Rectangle co in TileMap.collisionObjects)
                    {
                        if(curTileRect.Intersects(co))
                        {
                            tileMapWalkable[x, y] = false;
                            break;
                        }
                        else
                        {
                            tileMapWalkable[x, y] = true;
                        }

                    }
                }
            }
        }

        public Point GetTilePosition(Point mousePosition, Vector2 cameraPosition)
        {
            int mouseWorldXPos = mousePosition.X + (int)cameraPosition.X;
            int mouseWorldYPos = mousePosition.Y + (int)cameraPosition.Y;

            int tileXPos = mouseWorldXPos / TILEWIDTH_BASE;
            int tileYPos = mouseWorldYPos / TILEHEIGHT_BASE;

            return new Point(tileXPos, tileYPos);
        }

        public string GetBackgroundMusicFromMap()
        {
            return curMap.Properties["music"];
        }

        private ObservableCollection<Light> GetLightsFromMap()
        {
            ObservableCollection<Light> lamps = new ObservableCollection<Light>();

            if(curMap.ObjectGroups.Contains("LightSources"))
            {
                foreach(var o in curMap.ObjectGroups["LightSources"].Objects)
                {
                    Light tempLamp = new PointLight
                    {
                        Scale = new Vector2(400), //Range of the light
                        ShadowType = ShadowType.Solid,
                        Position = new Vector2((float)o.X, (float)o.Y),
                        Color = Color.Yellow
                    };

                    lamps.Add(tempLamp);

                    if (Lightning.lightSourceIds.ContainsKey(o.Id))
                        continue;

                    Lightning.lightSourceIds.Add(o.Id, tempLamp.GetHashCode());
                    Lightning.lightSourceExtensions.Add(new LightSourceExtension(o.Id, new Vector2((float)o.X, (float)o.Y)));
                }
            }
            return lamps;
        } 

        private void GetTileAnimation(TmxTileset tileSet)
        {
            SpriteAnimation temp = new SpriteAnimation(tileSets.Where(ts => ts.Key == tileSet.FirstGid).First().Value);
            temp.AddAnimation(tileSet.Name, 0, 0, tileSet.TileWidth, tileSet.TileHeight, tileSet.Tiles[0].AnimationFrames.Count, 0.1f);
            temp.CurrentAnimation = tileSet.Name;
            animatedTiles.Add(tileSet.FirstGid, temp);
        }

        private Texture2D GetTileSetImage(int gid)
        {
            Texture2D texture = null;

            foreach(KeyValuePair<int, Texture2D> tex in tileSets)
            {
                if (gid >= tex.Key)
                {
                    texture = tex.Value;
                    curFirstGid = tex.Key;
                }
            }
            return texture;
        }

        private int GetTileWidth(int firstGid)
        {
            foreach(TmxTileset tileset in curMap.Tilesets)
            {
                if (tileset.FirstGid == firstGid)
                    return tileset.TileWidth;
            }
            return 64; //fallback
        }

        private int GetTileHeight(int firstGid)
        {
            foreach (TmxTileset tileset in curMap.Tilesets)
            {
                if (tileset.FirstGid == firstGid)
                    return tileset.TileHeight;
            }
            return 64; //fallback
        }

        public void UpdateAnimatedTiles(GameTime gameTime)
        {
            foreach (KeyValuePair<int, SpriteAnimation> at in animatedTiles)
                at.Value.Update(gameTime);
        }

        public void DrawLayer(int layer)
        {

            for (var j = 0; j < curMap.Layers[layer].Tiles.Count; j++)
            {
                int gid = curMap.Layers[layer].Tiles[j].Gid;

                if (gid == 0)
                { 
                    //empty tile
                }

                else
                {
                    tileset = GetTileSetImage(gid);
                    int tilewidth = GetTileWidth(curFirstGid);
                    int tileheight = GetTileHeight(curFirstGid);

                    tilesetTilesWide = tileset.Width / tilewidth;
                    tilesetTilesHigh = tileset.Height / tileheight;
                    
                    int tileFrame = gid - curFirstGid;
                    int column = tileFrame % tilesetTilesWide;
                    int row = (int)Math.Floor((double)tileFrame / tilesetTilesWide);

                    float x = (j % curMap.Width) * TILEWIDTH_BASE;
                    // * TILEHEIGHT - TILEHEIGHT * ((tileheight / TILEHEIGHT) - 1) because Tiles >64px high have their 0|0 position bottom left instead top left
                    float y = (float)Math.Floor(j / (double)curMap.Width) * TILEHEIGHT_BASE - TILEHEIGHT_BASE * ((tileheight / TILEHEIGHT_BASE) - 1);

                    if(animatedTiles.ContainsKey(gid))
                    {
                        animatedTiles[gid].X = (int)x;
                        animatedTiles[gid].Y = (int)y;
                        animatedTiles[gid].Draw(Orbs.spriteBatch, null);
                    }
                    else
                    {
                        Rectangle tilesetRec = new Rectangle(TILEWIDTH_BASE * column, TILEHEIGHT_BASE * row, tilewidth, tileheight);
                        Orbs.spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tilewidth, tileheight), tilesetRec, Color.White);
                    }
                }
            }

            //For Debug. Writes True/False of tile is walkable
            //for (int x = 0; x < curMap.Width; x++)
            //    for (int y = 0; y < curMap.Height; y++)
            //        Game1.spriteBatch.DrawString(Game1.spriteFontDefault, tileMapWalkable[x, y].ToString(), new Vector2(x * 64, y * 64), Color.Red);
        }
    }
}
