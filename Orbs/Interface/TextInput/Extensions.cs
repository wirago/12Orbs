using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface.TextInput
{
    public static class Extensions
    {
        private static Texture2D pixel;

        public static Texture2D GetWhitePixel(this SpriteBatch spriteBatch)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false,
                    SurfaceFormat.Color);
                pixel.SetData(new[] { Color.White });
            }
            return pixel;
        }

        public static int Clamp(this int value, int min, int max)
        {
            if (value > max)
            {
                return max;
            }
            if (value < min)
            {
                return min;
            }
            return value;
        }
    }
}