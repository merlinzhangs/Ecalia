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
    /// PROTOTYPE: Draws multiple objects at the same time. Using only ONE OPENGL Draw Call
    /// </summary>
    public class SpriteBatch : IDisposable
    {
        Multimap<int, Drawable> sorted = new Multimap<int, Drawable>();
        List<Drawable> list = new List<Drawable>();

        public SpriteBatch()
        {

        }

        public void AddChild(Drawable sprite)
        {
            list.Add(sprite);
        }

        public void AddChild(Drawable sprite, int zOrder)
        {
            sorted.Add(zOrder, sprite);
        }

        public void Draw(DrawOrder drawOrder)
        {
            if (drawOrder == DrawOrder.UNSORTED)
            {
                foreach (Drawable d in list)
                {
                    Application.Window.Draw(d);
                }
            }

            if (drawOrder == DrawOrder.SORTED)
            {
                for (int i = 0; i < sorted.Keys.Count; i++)
                {
                    foreach (int key in sorted.Keys)
                    {
                        foreach (Drawable d in sorted[key])
                            Application.Window.Draw(d);
                    }
                }
            }
        }

        public void Dispose()
        {

        }

        public enum DrawOrder
        {
            UNSORTED,
            SORTED
        }
    }
}
