using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Graphics
{
    /// <summary>
    /// Used to Load Textures into the SFML Texture Class
    /// </summary>
    public static class Texture2D
    {
        public static Stream LoadFromStream(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream;
            }
        }

        public static byte[] LoadFromArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Loads texture
        /// </summary>
        /// <param name="fromStream">Load using stream or byte[].</param>
        /// <param name="bitmap">The image</param>
        /// <returns></returns>
        public static Texture LoadTexture(bool fromStream = false, Bitmap bitmap = null)
        {
            Texture texture;

            if (fromStream)
                texture = new Texture(LoadFromStream(bitmap));
            else
                texture = new Texture(LoadFromArray(bitmap));

            return texture;
        }
    }
}
