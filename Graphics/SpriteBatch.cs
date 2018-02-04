using Ecalia.Tools;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Graphics
{

    /// <summary>
    /// PROTOTYPE: Draws multiple objects at the same time. Using only ONE-TWO OPENGL Draw Call(s)
    /// </summary>
    public class SpriteBatch : IDisposable
    {
        MultiMap<int, Drawable> sorted = new MultiMap<int, Drawable>();
        List<Drawable> list = new List<Drawable>();
        private DrawOrder drawOrder;
        private DrawType dt;

        /// <summary>
        /// Initializes SpriteBatch class
        /// </summary>
        public SpriteBatch(DrawOrder order, DrawType drawType = DrawType.BACKGROUND)
        {
            drawOrder = order;
            dt = drawType;
        }

        /// <summary>
        /// Adds a drawable to sequence
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="zOrder"></param>
        public void AddChild(Drawable sprite, int zOrder = 0)
        {
            if (drawOrder == DrawOrder.UNSORTED)
                list.Add(sprite);
            else
                sorted.Add(zOrder, sprite);
        }

        /// <summary>
        /// Draw all the Drawable objects
        /// </summary>
        public void Draw()
        {
            if (drawOrder == DrawOrder.UNSORTED)
            {
                foreach (Drawable d in list)
                    Application.Window.Draw(d, RenderStates.Default);
            }

            if (drawOrder == DrawOrder.SORTED)
            {
                
                var keys = sorted.Keys.ToList();
                foreach (var key in keys)//sorted.Keys)
                {
                    //Console.WriteLine("Key: {0}", key);
                    foreach (Drawable d in sorted[key])
                        Application.Window.Draw(d, RenderStates.Default);
                } 

            }
        }

        public void Dispose()
        {
            sorted.Clear();
            list.Clear();
        }

        public enum DrawOrder
        {
            UNSORTED,
            SORTED
        }

        public enum DrawType
        {
            BACKGROUND,
            TILES,
            OBJECTS,
            PLAYER
        }
    }
}
