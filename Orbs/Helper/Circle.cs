using Microsoft.Xna.Framework;

namespace Orbs.Helper
{
    public class Circle
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Contains(Vector2 enemyPos)
        {
            return ((enemyPos - Center).Length() <= Radius);
        }

        public bool Intersects(Circle other)
        {
            return ((other.Center - Center).Length() < (other.Radius - Radius));
        }

        public bool Intersects(Rectangle enemyRect)
        {
            Vector2 enemyCenter = new Vector2(enemyRect.X + enemyRect.Width / 2, enemyRect.Y + enemyRect.Height / 2);
            float enemyRadius = enemyRect.Height / 2;

            return ((enemyCenter - Center).Length() < (enemyRadius - Radius));
        }
    }
}
