using CocosSharp;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Graphics
{
    public class SpriteManager
    {

        public SpriteManager()
        {
        }

        public void Draw(CCTexture2D texture)
        {
            Draw(texture, 0, 0);
        }

        public void Draw(CCTexture2D texture, int x, int y)
        {
            Draw(texture, x, y, 0, 0);
        }

        public void Draw(CCTexture2D texture, int x, int y, int cx, int cy)
        {
            Draw(texture, x, y, cx, cy, 0, 0, new CCColor4B(0xFF, 0xFF, 0xFF), false);
        }

        private void Draw(CCTexture2D cCTexture2D, int x, int y, int cx, int cy, int rx, int ry, CCColor4B color, bool v)
        {
            
        }

        public CCTexture2D GenerateTexture2D(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return new CCTexture2D(stream.ToArray()); // It is sad that it doesn't work via normal methods....
            }
        }
    }
}

