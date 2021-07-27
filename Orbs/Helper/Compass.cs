using Microsoft.Xna.Framework;
using System;

namespace Orbs.Helper
{
    public static class Compass
    {
        private static readonly string[] headings = new string[8] { "E", "NE", "N", "NW", "W", "SW", "S", "SE" };

        public static string GetAndSetPlayerOrientationByClickedSector()
        {
            Vector2 playerCenter = new Vector2(Player.PlayerPosition.X - Orbs.camera.Position.X, Player.PlayerPosition.Y - Orbs.camera.Position.Y);
            Vector2 mouseWindowPos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();
            Vector2 vector = mouseWindowPos - playerCenter;

            double angle = Math.Atan2(-vector.Y, vector.X);
            int octant = (int)Math.Round(8 * angle / (2 * Math.PI) + 8) % 8;

            return headings[octant];
        }
    }
}
