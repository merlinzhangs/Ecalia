using CocosSharp;
using Ecalia.Tools;
using reWZ;
using reWZ.WZProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecalia.Graphics
{
    public partial class Map : CCLayer
    {
        #region Classes

        private SpriteManager spriteManager;
        private Background mapBackground;
        private MapObjects mapObjects;
        private MapTiles mapTiles;
        private Animation animation;
        private CCSpriteBatchNode spriteBatchNode;

        // Wz 
        protected WZFile mapFile = new WZFile(@"E:\v83\Map.wz", WZVariant.GMS, true);
        private WZImage mapImg;
        private WZImage backgroundImg;
        private WZImage objectImg;

        // Lists
        AnimationList objects = new AnimationList();
        AnimationList backgrounds = new AnimationList();

        // Events
        protected CCEventListenerKeyboard keyboard;

        #endregion

        // Testing
        private bool testingInProgress = true;
        public int mapId { protected get; set; }
        protected Camera camera = new Camera();

        /// <summary>
        /// Starts the process of loading a map
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="id"></param>
        public Map(int id = 0)
        {
            spriteManager = new SpriteManager(); // Handles Drawing Functions
            keyboard = new CCEventListenerKeyboard();
            animation = new Animation();
            spriteBatchNode = new CCSpriteBatchNode();
            

            // If there's testing going on, use the specified ID, if not use the correct ID
            if (testingInProgress)
                mapId = 10000;
            else
                mapId = id;


            mapImg = mapFile.MainDirectory["Map"]["Map" + GetMapName(mapId)[0]][GetMapName(mapId) + ".img"] as WZImage; // Get's the Map Image

            mapBackground = new Background(); // Loads the Background
            mapObjects = new MapObjects();
            mapTiles = new MapTiles();
            
            // Event Listeners
            InitEventListeners();

            // Camera
            AddChild(camera);
            RunAction(new CCFollow(camera, CCRect.Zero));

            // Keyboard
            keyboard.OnKeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(CCEventKeyboard obj)
        {
            switch (obj.KeyboardEventType)
            {
                case CCKeyboardEventType.KEYBOARD_PRESS:
                    HandlInput(obj);
                    break;
            }
        }

        private void HandlInput(CCEventKeyboard obj)
        {
            switch(obj.Keys)
            {
                case CCKeys.Left:
                    camera.PositionX -= 100;
                    break;
                case CCKeys.Right:
                    camera.PositionX += 100;
                    break;
                case CCKeys.Up:
                    camera.PositionY += 100;
                    break;
                case CCKeys.Down:
                    camera.PositionY -= 100;
                    break;

            }
        }

        private void PlayAnimations(CCEventCustom eventCustom)
        {
            //CCAnimate Animate = new CCAnimate(animation);
            //Animate.Duration = 100;
            //animation.Loops = 10;
            //AddChild(objects[0]);
            //objects[0].RunAction(Animate);
        }

        private void InitEventListeners()
        {
            AddEventListener(keyboard, this);
            AddCustomEventListener("OnLoad", OnLoad); // Registers the EventListener with the Scene/Layer.
            AddCustomEventListener("PlayAnimation", PlayAnimations);
        }

        /// <summary>
        /// Starts the process of loading the map
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoad(CCEventCustom obj)
        {
            LoadBackground(mapImg);
            LoadObjects(mapImg);
            LoadTiles(mapImg);
            AddChild(spriteBatchNode);
            // When Finished
            DispatchEvent("PlayAnimation");
        }

        public override void OnEnter()
        {
            base.OnEnter();
            DispatchEvent("OnLoad");
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

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
                spriteBatchNode.AddChild(new CCSprite(
                    spriteManager.GenerateTexture2D(((
                    (WZCanvasProperty)mapFile.MainDirectory["Back"][map.bS + ".img"]["back"][map.no.ToString()]).Value)))
                {
                    Position = new CCPoint(map.x, -map.y)
                });
            }
            //AddChild(spriteBatchNode);
        }

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
                            if (canvas is WZCanvasProperty) // TODO: Improve.
                            {
                                objects.Add(new CCSprite(spriteManager.GenerateTexture2D(((WZCanvasProperty)canvas).Value))
                                {
                                    Position = new CCPoint(mapobj.x, -mapobj.y),
                                    VertexZ = mapobj.z,
                                });
                                animation.AddFrame(new CCSprite(spriteManager.GenerateTexture2D(((WZCanvasProperty)canvas).Value)), animation.GetDelay(canvas));
                            }
                        }
                    }
                    else
                    {
                        foreach (var canvas in location)
                        {
                            if (canvas is WZCanvasProperty)
                            {
                                spriteBatchNode.AddChild(new CCSprite(spriteManager.GenerateTexture2D(((WZCanvasProperty)canvas).Value))
                                {
                                    Position = new CCPoint(mapobj.x, -mapobj.y),
                                    ZOrder = mapobj.z,
                                });
                            }
                        }
                    }
                }
            }
        }

        private void LoadTiles(WZImage map)
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(i);

                var tiles = map[i.ToString()]["tile"];
                Console.WriteLine("Node: {0}", tiles.Name);

                foreach (var tile in tiles)
                {
                    Console.WriteLine("Tile: {0}", tile.Name);
                    var mapTile = mapTiles.LoadFromNode((WZSubProperty)tiles[tile.Name]);
                    mapTile.tS = DataTool.GetString(map["0"]["info"]["tS"]);
                    Console.WriteLine("{0}:{1}:{2}", mapTile.tS, mapTile.u, mapTile.no.ToString());
                    var location = mapFile.MainDirectory["Tile"][mapTile.tS + ".img"][mapTile.u][mapTile.no.ToString()];

                    if (location is WZCanvasProperty)
                    {
                        Console.WriteLine("{0}:{1}", mapTile.x, -mapTile.y);
                        spriteBatchNode.AddChild(new CCSprite(spriteManager.GenerateTexture2D(((WZCanvasProperty)location).Value))
                        {
                            Position = new CCPoint(mapTile.x, -mapTile.y),
                            ZOrder = mapTile.zM,
                        });
                    }
                }
            }
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
        public int zM { get; set; }

        public CCColor4B color;

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
                color = new CCColor4B(0xFF, 0xFF, 0xFF, a),
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

    public class MapTiles
    {

        public int x { get; set; } // X-Pos
        public int y { get; set; } // Y-Pos
        public string u { get; set; } // the sub folder
        public int no { get; set; } // The node that contains the image
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
}
