using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Orbs
{
    public class PlayerPet
    {
        private Vector2 position;
        private Texture2D texture; //TODO replace with spine

        #region Waypoint System
        private Vector2 targetPosition; //current waypointPosition
        private Vector2 petMovement = Vector2.Zero;

        //Pathfinding
        private Point followPath_from = Point.Zero;
        private Point followPath_to = Point.Zero;
        private List<Point> followPath = null;
        private int currentPathStep = 0;
        #endregion

        private const int MAXDISTANCE = 200;
        private const int MOVEMENTSPEED = 4;
        private Vector2 PLAYEROFFSET = new Vector2(-48, 48);

        public PlayerPet(Vector2 playerPos)
        {
            position = playerPos + PLAYEROFFSET;
            targetPosition = position;

            texture = Orbs.content.Load<Texture2D>(@"Animations\Characters\petTexture");
        }

        public void Update(GameTime gameTime)
        {
            if (Vector2.Distance(position, Player.PlayerPosition) > MAXDISTANCE && followPath == null)
                CreateFollowPath();

            if (followPath != null)
            {
                if (Vector2.Distance(position, targetPosition) > 2)
                {
                    petMovement = Vector2.Normalize(targetPosition - position);
                    position += petMovement * MOVEMENTSPEED;
                }
                else
                {
                    Point targetPoint = GetNextPathPoint();
                    targetPosition = new Vector2(targetPoint.X * 64, targetPoint.Y * 64);
                }
            }
        }
        
        private void CreateFollowPath()
        {
            followPath_from = new Point((int)position.X / 64, (int)position.Y / 64);
            followPath_to = GetTargetPoint();

            currentPathStep = 0;
            followPath = Pathfinding.Pathfinding.FindPath(Tiled.TileMap.pathfindGrid, followPath_from, followPath_to);

            if (followPath.Count == 0)
                return;

            targetPosition = new Vector2(followPath[0].X * 64, followPath[0].Y * 64);
        }

        private Point GetTargetPoint()
        {
            return new Point(
                (int)((Player.PlayerPosition.X)) / 64,
                (int)((Player.PlayerPosition.Y)) / 64
                );
        }

        /// <summary>
        /// Gets the next path point from current follow path
        /// </summary>
        /// <returns>Point of next targetposition</returns>
        private Point GetNextPathPoint()
        {
            if(currentPathStep == followPath.Count - 1)
            {
                Point p = followPath[currentPathStep];
                followPath = null;
                return p;
            }

            return followPath[currentPathStep++];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
