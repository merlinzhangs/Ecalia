using Ecalia.Graphics;
using Ecalia.Tools;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure;

using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ecalia.Game
{
    public partial class Map
    {
        #region Classes

        private Background mapBackground;
        private MapObjects mapObjects;
        private MapTiles mapTiles;
        List<Layer> images = new List<Layer>();
        MultiMap<float, Sprite> objs = new MultiMap<float, Sprite>();
        MultiMap<float, Sprite> tiles = new MultiMap<float, Sprite>();
        MultiMap<float, Sprite> backgrounds = new MultiMap<float, Sprite>();
        //SpriteBatch ob = new SpriteBatch(SpriteBatch.DrawOrder.SORTED, SpriteBatch.DrawType.OBJECTS);
        SpriteBatch spr = new SpriteBatch(SpriteBatch.DrawOrder.SORTED, SpriteBatch.DrawType.TILES);

        // Wz 
        protected WzFile mapFile = new WzFile(Path.Combine(Config.wzFolder, "Map.wz"), WzMapleVersion.GMS);
        protected WzFile uiFile = new WzFile(Path.Combine(Config.wzFolder, "UI.wz"), WzMapleVersion.GMS);
        //protected WZFile mapFile = new WZFile(Path.Combine(Config.wzFolder, "Map.wz"), WZVariant.GMS, true); // I know I can do Path.Combine, but I don't really feel like it.
        //private WZImage mapImg;
        private WzImage mapImg;
        private WzVectorProperty origin;
        //private WZPointProperty origin;

        // Events

        #endregion

        public int mapId { protected get; set; }

        /// <summary>
        /// Starts the process of loading a map
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="id"></param>
        public Map(int id = 0)
        {
            // If there's testing going on, use the specified ID, if not use the correct ID
            if (Config.developerMode)
                mapId = 60000;
            else
                mapId = id;
            mapFile.ParseWzFile();
            uiFile.ParseWzFile();
            mapImg = uiFile["MapLogin.img"] as WzImage;
            //mapImg = mapFile["Map"]["Map" + GetMapName(mapId)[0]][GetMapName(mapId) + ".img"] as WzImage;//mapFile.MainDirectory["Map"]["Map" + GetMapName(mapId)[0]][GetMapName(mapId) + ".img"] as WZImage; // Get's the Map Image

            mapBackground = new Background();
            mapObjects = new MapObjects();
            mapTiles = new MapTiles();
        }

        private void PlayAnimations()
        {
            /*
            CCAnimation[] animation = new CCAnimation[ani.Count()];
            CCAnimate[] animate = new CCAnimate[ani.Count()];
            CCSprite[] sprite = new CCSprite[ani.Count()];

            for (int i = 0; i < ani.Count(); i++)
            {
                sprite[i] = new CCSprite();
                animation[i] = new CCAnimation();
                
                foreach (CCSprite spr in ani[i.ToString()])
                {
                    animation[i].AddSpriteFrame(spr);
                    animation[i].DelayPerUnit = 0.2f;
                }

                animate[i] = new CCAnimate(animation[i]);
                AddChild(ani[i.ToString()].First());
                ani[i.ToString()].First().RunAction(new CCRepeatForever(animate[i]));
            }*/

            /*CCAnimation[] animation = new CCAnimation[ani.Count()];
            CCAnimate[] animate = new CCAnimate[ani.Count()];
            CCSprite[] sprite = new CCSprite[ani.Count()];

            for (int i = 0; i < ani.Count(); i++)
            {
                sprite[i] = new CCSprite();
                animation[i] = new CCAnimation();
                var s = MapObjLocation.Distinct().ToList()[i];

                foreach (CCSprite spr in ani[s])
                {
                    animation[i].AddSpriteFrame(spr);
                    animation[i].DelayPerUnit = 0.2f;
                }

                animate[i] = new CCAnimate(animation[i]);
                AddChild(ani[s].First());
                ani[s].First().RunAction(new CCRepeatForever(animate[i]));
            }*/

        }

        /// <summary>
        /// Starts the process of loading the map
        /// </summary>
        /// <param name="obj"></param>
        public void OnLoad()
        {
            LoadBackground(mapImg);
            LoadTiles(mapImg);
            LoadObjects(mapImg);

            backgrounds.Keys.ToList().ForEach(k =>
            {
                backgrounds[k].ToList().ForEach(layer =>
                {
                    spr.AddChild(layer, (int)k);
                });

            });


            tiles.Keys.ToList().ForEach(k =>
            {
                tiles[k].ToList().ForEach(layer =>
                {
                    spr.AddChild(layer,(int)k);
                });

            });

            objs.Keys.ToList().ForEach(k =>
            {
                objs[k].ToList().ForEach(layer =>
                {
                    spr.AddChild(layer, (int)k);
                });
            });
        }

        #region Background

        /// <summary>
        /// Loads the background for the map
        /// </summary>
        /// <param name="wzImage"></param>
        private void LoadBackground(WzImage wzImage)
        {
            var background = wzImage["back"] as WzSubProperty;

            foreach (var back in background.WzProperties)
            {
                var map = mapBackground.LoadFromNode((WzSubProperty)background[back.Name]);
                WzCanvasProperty canvas = mapFile["Back"][map.bS + ".img"]["back"][map.no.ToString()] as WzCanvasProperty;//mapFile.MainDirectory["Back"][map.bS + ".img"]["back"][map.no.ToString()] as WZCanvasProperty;
                var z = 0;
                //if (canvas.HasChild("origin"))
                if (canvas["origin"] != null)
                    origin = canvas["origin"] as WzVectorProperty;
                if (canvas["z"] != null)
                    z = InfoTool.GetInt(canvas["z"]);
                backgrounds.Add(z, new Sprite(Texture2D.LoadTexture(false, canvas.GetBitmap()))
                {
                    Position = new Vector2f(map.x, map.y),
                    Origin = new Vector2f(InfoTool.GetFloat(origin.X), InfoTool.GetFloat(origin.Y)),
                });

            }
        }

        #endregion

        #region Obj

        /// <summary>
        /// Loads Map Objects
        /// </summary>
        /// <param name="wzImage"></param>
        private void LoadObjects(WzImage wzImage)
        {
            for (int i = 0; i < 8; i++)
            {
                var map = wzImage[i.ToString()]["obj"];
                foreach (var obj in map.WzProperties)
                {
                    //Console.WriteLine(obj.Name);
                    var mapobj = mapObjects.LoadFromNode((WzSubProperty)map[obj.Name]);
                    //Console.WriteLine("{0}->{1}->{2}->{3}", mapobj.oS, mapobj.l0, mapobj.l1, mapobj.l2);
                    var location = mapFile["Obj"][mapobj.oS + ".img"][mapobj.l0][mapobj.l1][mapobj.l2] as WzSubProperty;//mapFile.MainDirectory["Obj"][mapobj.oS + ".img"][mapobj.l0][mapobj.l1][mapobj.l2];
                    if (location.WzProperties.Count > 1)
                    {
                        foreach (var canvas in location.WzProperties)
                        {

                            /*if (canvas.HasChild("z"))
                            {
                                if (DataTool.GetInt(canvas["z"]) != 0)
                                    mapobj.z = DataTool.GetInt(location["z"]);
                                else
                                    mapobj.z = mapobj.zM;
                            }*/

                            if (canvas["origin"] != null)
                                origin = canvas["origin"] as WzVectorProperty;

                            //Console.WriteLine("{0}->{1}->{2}->{3}->{4}", mapobj.oS, mapobj.l0, mapobj.l1, mapobj.l2, canvas.Name);
                            if (canvas is WzCanvasProperty) // TODO: Improve.
                            {
                            }
                        }
                    }
                    else
                    {
                        foreach (var canvas in location.WzProperties)
                        {
                            if (canvas is WzCanvasProperty)
                            {
                                if (canvas["origin"] != null)
                                    origin = canvas["origin"] as WzVectorProperty;

                                var z1 = mapobj.z;
                                var z2 = mapobj.zM;
                                var rz = Math.Min(z1, z2);
                                Console.WriteLine($"Z1 {z1} Z2 {z2}");

                                objs.Add(z1, new Sprite(Texture2D.LoadTexture(false, canvas.GetBitmap()))
                                {
                                    Position = new Vector2f(mapobj.x, mapobj.y),
                                    Origin = new Vector2f(InfoTool.GetFloat(origin.X), InfoTool.GetFloat(origin.Y)),
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Tiles

        private void LoadTiles(WzImage map)
        {
            for (int i = 0; i < 8; ++i) // Supposed to be 7..but I don't feel like changing it.
            {
                var tileobj = map[i.ToString()]["tile"];
                if (map[i.ToString()]["info"]["tS"] != null)
                {
                    foreach (var tile in tileobj.WzProperties)
                    {
                        var mapTile = mapTiles.LoadFromNode((WzSubProperty)tileobj[tile.Name]);
                        mapTile.tS = InfoTool.GetString(map[i.ToString()]["info"]["tS"]);
                        var location = mapFile["Tile"][mapTile.tS + ".img"][mapTile.u][mapTile.no.ToString()];//mapFile.MainDirectory["Tile"][mapTile.tS + ".img"][mapTile.u][mapTile.no.ToString()];

                        if (location is WzCanvasProperty)
                        {
                            if (location["z"] != null)
                                mapTile.z = InfoTool.GetInt((WzIntProperty)location["z"]);

                            var z1 = mapTile.z;
                            var z2 = mapTile.zM;
                            var rz = z1 == z2 ? 0 : (z1 > z2 ? -1 : mapTile.zM);

                            if (location["origin"] != null)
                                origin = location["origin"] as WzVectorProperty;

                            // This works...somehow..I'm surprised it doesn't complain about dividing by zero
                            tiles.Add(z1 / z2, new Sprite(Texture2D.LoadTexture(false, location.GetBitmap()))
                            {
                                Position = new Vector2f(mapTile.x, mapTile.y),
                                Origin = new Vector2f(InfoTool.GetFloat(origin.X), InfoTool.GetFloat(origin.Y)),
                            });
                            
                        }
                    }
                }
            }
        }

        #endregion

        public void Draw()
        {
            spr.Draw();
        }

        public void Dispose()
        {
            mapFile.Dispose();
        }

        /// <summary>
        /// Get's the string version of the map id.
        /// </summary>
        /// <param name="mapid">the id of the map being loaded</param>
        /// <returns></returns>
        private string GetMapName(int mapid)
        {
            if (mapid.ToString().Length == 5)
                return "0000" + mapid;
            if (mapid.ToString().Length == 7)
                return "00" + mapid;
            return mapid.ToString();
        }
    }

    #region Background Class

    /// <summary>
    /// Loads all information for the backgrounds
    /// </summary>
    public class Background
    {
        #region Variables

        public int a { get; set; } // alpha
        public int ani { get; set; } // animation (bool)
        public string bS { get; set; } // background set
        public int cx { get; set; } // center x-pos
        public int cy { get; set; } // center y-pos
        public int flipped { get; set; } // Flipped
        public int front { get; set; } // foreground (bool)
        public int no { get; set; } // Node which the image is set in
        public int rx { get; set; } // rotation?
        public int ry { get; set; }
        public BackgroundType type { get; set; } // What type of whatever it is (tile/moving/etc)
        public int x { get; set; } // x-pos of the image.
        public int y { get; set; } // y-pos of the image.
        public int z { get; set; }
        public int zM { get; set; }

        #endregion

        public Background()
        {
        }

        /// <summary>
        /// Loads all the node information for the background(s) that need to be loaded.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal Background LoadFromNode(WzSubProperty node)
        {
            var back = new Background()
            {
                a = InfoTool.GetInt(node["a"]),//node["a"].ValueOrDie<int>(),
                ani = InfoTool.GetInt(node["ani"]),
                bS = InfoTool.GetString(node["bS"]),
                cx = InfoTool.GetInt(node["cx"]),
                cy = InfoTool.GetInt(node["cy"]),
                flipped = InfoTool.GetInt(node["f"]),
                front = InfoTool.GetInt(node["front"]),
                no = InfoTool.GetInt(node["no"]),
                rx = InfoTool.GetInt(node["rx"]),
                ry = InfoTool.GetInt(node["ry"]),
                type = (BackgroundType)InfoTool.GetInt(node["type"]),
                x = InfoTool.GetInt(node["x"]),
                y = InfoTool.GetInt(node["y"]),
                //zM = DataTool.GetInt(node["zM"]),
            };
            return back;
        }

        public enum BackgroundType
        {
            Regular = 0,
            HorizontalTiling = 1,
            VerticalTiling = 2,
            HVTiling = 3,
            HorizontalMoving = 4,
            VerticalMoving = 5,
            HorizontalMovingHVTiling = 6,
            VerticalMovingHVTiling = 7
        }
    }

    #endregion

    #region MapObjects Class

    /// <summary>
    /// Loads Map Object(s) information
    /// </summary>
    public class MapObjects
    {
        public string oS { get; set; } // background set
        public string l0 { get; set; } // The WZ Image
        public string l1 { get; set; } // Sub Folder
        public string l2 { get; set; } // The Node
        public int flipped { get; set; } // Flipped
        public int x { get; set; } // x-pos of the image.
        public int y { get; set; } // y-pos of the image.
        public float z { get; set; } // z-pos of the image.
        public int zM { get; set; } // I have no idea...

        public MapObjects()
        {

        }

        /// <summary>
        /// Loads the node information
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal MapObjects LoadFromNode(WzSubProperty node)
        {
            var obj = new MapObjects()
            {
                oS = InfoTool.GetString(node["oS"]),
                l0 = InfoTool.GetString(node["l0"]),
                l1 = InfoTool.GetString(node["l1"]),
                l2 = InfoTool.GetString(node["l2"]),
                x = InfoTool.GetInt(node["x"]),
                y = InfoTool.GetInt(node["y"]),
                z = InfoTool.GetInt(node["z"]),
                flipped = InfoTool.GetInt(node["f"]),
                zM = InfoTool.GetInt(node["zM"]),
            };

            return obj;
        }
    }

    #endregion

    #region MapTiles Class

    /// <summary>
    /// Loads map tile information
    /// </summary>
    public class MapTiles
    {

        public int x { get; set; } // X-Pos
        public int y { get; set; } // Y-Pos
        public string u { get; set; } // the sub folder
        public int no { get; set; } // The node that contains the image
        public float z { get; set; }
        public int zM { get; set; } // still don't know what this....
        public string tS { get; set; } // TileSet

        public MapTiles()
        {

        }

        internal MapTiles LoadFromNode(WzSubProperty node)
        {
            var tile = new MapTiles()
            {
                //tS = DataTool.GetString(node["info"]["tS"]),
                x = InfoTool.GetInt(node["x"]),
                y = InfoTool.GetInt(node["y"]),
                u = InfoTool.GetString(node["u"]),
                no = InfoTool.GetInt(node["no"]),
                zM = InfoTool.GetInt(node["zM"]),
            };

            return tile;
        }

        /// <summary>
        /// Get's the type of tile that needs to be implmented in order to be loaded.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TileType GetTileType(int type)
        {
            switch (type)
            {
                case 0:
                    return TileType.None;
                case 1:
                    return TileType.Horizontal;
                case 2:
                    return TileType.Vertical;
                case 3:
                    return TileType.Both;
                case 4:
                    return TileType.Horizontal | TileType.HorizontalScroll;
                case 5:
                    return TileType.Vertical | TileType.VerticalScroll;
                case 6:
                    return TileType.Both | TileType.HorizontalScroll;
                case 7:
                    return TileType.Both | TileType.VerticalScroll;
                default:
                    return TileType.None;
            }
        }

        [Flags]
        public enum TileType
        {
            None,
            Horizontal,
            Vertical,
            Both = Horizontal | Vertical,
            HorizontalScroll = 4,
            VerticalScroll = 8
        };
    }

    #endregion
}