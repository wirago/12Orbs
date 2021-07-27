using Microsoft.Xna.Framework;

namespace Orbs.Helper
{
    public static class Triangle
    {
        private static Vector2 topLeft = new Vector2(0, 0);
        private static Vector2 topRight = new Vector2(Orbs.graphics.PreferredBackBufferWidth, 0);
        private static Vector2 bottomRight = new Vector2(Orbs.graphics.PreferredBackBufferWidth, Orbs.graphics.PreferredBackBufferHeight);
        private static Vector2 bottomLeft = new Vector2(0, Orbs.graphics.PreferredBackBufferHeight);

        //Return true if p1 and p2 are on the same side of BA
        private static bool SameSide(Vector2 p1, Vector2 p2, Vector2 A, Vector2 B)
        {
            // Convert points to Vector3 for use the Cross product, which is Vector3-only
            Vector3 cp1 = Vector3.Cross(new Vector3(B - A, 0), new Vector3(p1 - A, 0));
            Vector3 cp2 = Vector3.Cross(new Vector3(B - A, 0), new Vector3(p2 - A, 0));
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        //Return true if the point p is in the triangle ABC
        private static bool PointInTriangle(Vector2 p, Vector2 A, Vector2 B, Vector2 C)
        {
            return SameSide(p, A, B, C) && SameSide(p, B, A, C) && SameSide(p, C, A, B);
        }

        public static string GetAndSetPlayerOrientationByClickedSector()
        {
            Vector2 playerCenter = new Vector2(Player.PlayerPosition.X - Orbs.camera.Position.X + 48, Player.PlayerPosition.Y - Orbs.camera.Position.Y + 48);
            Vector2 mouseWindowPos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();

            if (PointInTriangle(mouseWindowPos, topLeft, playerCenter, topRight))
            {
                Player.curDirection = "North";
                return "North";
            }

            if (PointInTriangle(mouseWindowPos, topRight, playerCenter, bottomRight))
            {
                Player.curDirection = "East";
                return "East";
            }

            if (PointInTriangle(mouseWindowPos, bottomRight, playerCenter, bottomLeft))
            {
                Player.curDirection = "South";
                return "South";
            }

            if (PointInTriangle(mouseWindowPos, topLeft, playerCenter, bottomLeft))
            {
                Player.curDirection = "West";
                return "West";
            }

            return "";
        }
    }
}
