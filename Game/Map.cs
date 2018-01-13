using Ecalia.Graphics;
using Ecalia.Tools;
using reWZ;
using reWZ.WZProperties;
using SFML.Graphics;
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
        //private CCSpriteBatchNode spriteBatchNode; // Even if it's depreceted it's still pretty handy IMO

        // Wz 
        protected WZFile mapFile = new WZFile(Path.Combine(Config.wzFolder, "Map.wz"), WZVariant.GMS, true); // I know I can do Path.Combine, but I don't really feel like it.
        private WZImage mapImg;
        private WZPointProperty origin;

        // Lists
        Multimap<int, Sprite> backgrounds = new Multimap<int, Sprite>();
        List<string> MapObjLocation = new List<string>();
        

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
                mapId = 10000;
            else
                mapId = id;


            mapImg = mapFile.MainDirectory["Map"]["Map" + GetMapName(mapId)[0]][GetMapName(mapId) + ".img"] as WZImage; // Get's the Map Image

            mapBackground = new Background(); // Loads the Background
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
            //AddChild(spriteBatchNode);
            // When Finished
            //DispatchEvent("PlayAnimation");
        }

        #region Background

        /// <summary>
        /// Loads the background for the map
        /// </summary>
        /// <param name="wzImage"></param>
        private void LoadBackground(WZImage wzImage)
        {
            var background = wzImage["back"] as WZSubProperty;

            foreach (var back in background)
            {
                //Console.WriteLine(back.Name);
                var map = mapBackground.LoadFromNode((WZSubProperty)background[back.Name]);
                WZCanvasProperty canvas = mapFile.MainDirectory["Back"][map.bS + ".img"]["back"][map.no.ToString()] as WZCanvasProperty;

                /*if (canvas.HasChild("z"))
                {
                    if (DataTool.GetInt(canvas["z"]) == 0)
                        map.z = map.zM;
                    else
                        map.z = DataTool.GetInt(canvas["z"]);
                }*/

                /*AddChild(new CCSprite(spriteManager.GenerateTexture2D((canvas.Value)))
                {
                    Position = new CCPoint(map.x, InvertY(map.y)),
                    //ZOrder = map.z,
                });*/

                if (canvas.HasChild("origin"))
                    origin = canvas["origin"] as WZPointProperty;

                Application.Window.Draw(new Sprite(Texture2D.LoadTexture(false, canvas.Value))
                {
                    Origin = new SFML.System.Vector2f(origin.Value.X, origin.Value.Y),
                    Position = new SFML.System.Vector2f(map.x, map.y),
                    Color = new Color(0xFF, 0xFF, 0xFF, (byte)map.a),
                });
                //Sprite spr = new Sprite(Texture2D.LoadTexture(false, canvas.Value));
                //backgrounds.Add(int.Parse(back.Name), spr);
                
            }
        }

        #endregion

        #region Obj

        /// <summary>
        /// Loads Map Objects
        /// </summary>
        /// <param name="wzImage"></param>
        private void LoadObjects(WZImage wzImage)
        {
            for (int i = 0; i < 8; i++)
            {
                var map = wzImage[i.ToString()]["obj"];
                foreach (var obj in map)
                {
                    //Console.WriteLine(obj.Name);
                    var mapobj = mapObjects.LoadFromNode((WZSubProperty)map[obj.Name]);
                    //Console.WriteLine("{0}->{1}->{2}->{3}", mapobj.oS, mapobj.l0, mapobj.l1, mapobj.l2);
                    var location = mapFile.MainDirectory["Obj"][mapobj.oS + ".img"][mapobj.l0][mapobj.l1][mapobj.l2];
                    if (location.ChildCount > 1)
                    {
                        foreach (var canvas in location)
                        {

                            if (canvas.HasChild("z"))
                            {
                                if (DataTool.GetInt(canvas["z"]) != 0)
                                    mapobj.z = DataTool.GetInt(location["z"]);
                                else
                                    mapobj.z = mapobj.zM;
                            }

                            if (canvas.HasChild("origin"))
                                origin = canvas["origin"] as WZPointProperty;

                            //Console.WriteLine("{0}->{1}->{2}->{3}->{4}", mapobj.oS, mapobj.l0, mapobj.l1, mapobj.l2, canvas.Name);
                            if (canvas is WZCanvasProperty) // TODO: Improve.
                            {
                                MapObjLocation.Add(mapobj.oS + ":" + mapobj.l0 + ":" + mapobj.l1 + ":" + mapobj.l2);
                                //Console.WriteLine(location.Name);
                                //var tex = spriteManager.GenerateTexture2D(((WZCanvasProperty)canvas).Value);
                                //sprFrame.Add(new CCSpriteFrame(tex, new CCRect(0, 0, tex.PixelsWide, tex.PixelsHigh)));
                            }
                        }
                    }
                    else
                    {
                        foreach (var canvas in location)
                        {
                            if (canvas is WZCanvasProperty)
                            {
                                if (canvas.HasChild("z"))
                                {
                                    if (DataTool.GetInt(canvas["z"]) != 0)
                                        mapobj.z = DataTool.GetInt(location["z"]);
                                    else
                                        mapobj.z = mapobj.zM;
                                }

                                if (canvas.HasChild("origin"))
                                    origin = canvas["origin"] as WZPointProperty;
                                Application.Window.Draw(new Sprite(Texture2D.LoadTexture(false, ((WZCanvasProperty)canvas).Value))
                                {
                                    Origin = new SFML.System.Vector2f(origin.Value.X, origin.Value.Y),
                                    Position = new SFML.System.Vector2f(mapobj.x, mapobj.y),
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Tiles

        private void LoadTiles(WZImage map)
        {
            for (int i = 0; i < 8; i++) // Supposed to be 7..but I don't feel like changing it.
            {
                //Console.WriteLine(i);

                var tiles = map[i.ToString()]["tile"];
                //Console.WriteLine("Node: {0}", tiles.Name);

                foreach (var tile in tiles)
                {
                    //Console.WriteLine("Tile: {0}", tile.Name);
                    var mapTile = mapTiles.LoadFromNode((WZSubProperty)tiles[tile.Name]);
                    mapTile.tS = DataTool.GetString(map["0"]["info"]["tS"]);
                    //Console.WriteLine("{0}:{1}:{2}", mapTile.tS, mapTile.u, mapTile.no.ToString());
                    var location = mapFile.MainDirectory["Tile"][mapTile.tS + ".img"][mapTile.u][mapTile.no.ToString()];

                    if (location is WZCanvasProperty)
                    {
                        //Console.WriteLine("{0}:{1}", mapTile.x, mapTile.y);

                        if (location.HasChild("z"))
                        {
                            if (DataTool.GetInt(location["z"]) == 0)
                                mapTile.z = mapTile.zM;
                            else
                                mapTile.z = DataTool.GetInt(location["z"]);
                        }

                        if (location.HasChild("origin"))
                            origin = location["origin"] as WZPointProperty;

                        Application.Window.Draw(new Sprite(Texture2D.LoadTexture(false, ((WZCanvasProperty)location).Value))
                        {
                            Origin = new SFML.System.Vector2f(origin.Value.X, origin.Value.Y),
                            Position = new SFML.System.Vector2f(mapTile.x, mapTile.y),
                            
                        });
                    }
                }
            }
        }

        #endregion

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

        private float InvertY(float ypos)
        {
            return 0 - ypos;
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
        public int type { get; set; } // What type of whatever it is (tile/moving/etc)
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
        internal Background LoadFromNode(WZSubProperty node)
        {
            var back = new Background()
            {
                a = node["a"].ValueOrDie<int>(),
                ani = node["ani"].ValueOrDie<int>(),
                bS = node["bS"].ValueOrDie<string>(),
                cx = node["cx"].ValueOrDie<int>(),
                cy = node["cy"].ValueOrDie<int>(),
                flipped = node["f"].ValueOrDie<int>(),
                front = node["front"].ValueOrDie<int>(),
                no = node["no"].ValueOrDie<int>(),
                rx = node["rx"].ValueOrDie<int>(),
                ry = node["ry"].ValueOrDie<int>(),
                type = node["type"].ValueOrDie<int>(),
                x = node["x"].ValueOrDie<int>(),
                y = node["y"].ValueOrDie<int>(),
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

    public class MapObjects
    {
        public string oS { get; set; } // background set
        public string l0 { get; set; } // The WZ Image
        public string l1 { get; set; } // Sub Folder
        public string l2 { get; set; } // The Node
        public int flipped { get; set; } // Flipped
        public int x { get; set; } // x-pos of the image.
        public int y { get; set; } // y-pos of the image.
        public int z { get; set; } // z-pos of the image.
        public int zM { get; set; } // I have no idea...

        public MapObjects()
        {

        }

        /// <summary>
        /// Loads the node information
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal MapObjects LoadFromNode(WZSubProperty node)
        {
            var obj = new MapObjects()
            {
                oS = DataTool.GetString(node["oS"]),
                l0 = DataTool.GetString(node["l0"]),
                l1 = DataTool.GetString(node["l1"]),
                l2 = DataTool.GetString(node["l2"]),
                x = DataTool.GetInt(node["x"]),
                y = DataTool.GetInt(node["y"]),
                z = DataTool.GetInt(node["z"]),
                flipped = DataTool.GetInt(node["f"]),
                zM = DataTool.GetInt(node["zM"]),
            };

            return obj;
        }
    }

    #endregion

    #region MapTiles Class

    public class MapTiles
    {

        public int x { get; set; } // X-Pos
        public int y { get; set; } // Y-Pos
        public string u { get; set; } // the sub folder
        public int no { get; set; } // The node that contains the image
        public int z { get; set; }
        public int zM { get; set; } // still don't know what this....
        public string tS { get; set; } // TileSet

        public MapTiles()
        {

        }

        internal MapTiles LoadFromNode(WZSubProperty node)
        {
            var tile = new MapTiles()
            {
                //tS = DataTool.GetString(node["info"]["tS"]),
                x = DataTool.GetInt(node["x"]),
                y = DataTool.GetInt(node["y"]),
                u = DataTool.GetString(node["u"]),
                no = DataTool.GetInt(node["no"]),
                zM = DataTool.GetInt(node["zM"]),
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
