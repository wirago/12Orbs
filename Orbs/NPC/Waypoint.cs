using Microsoft.Xna.Framework;

namespace Orbs.NPC
{
    public class Waypoint
    {
        //waypoint coordinates
        private Vector2 coordinate;

        //special atributes
        private float waitingTime = 0;

        //get the coords
        public Vector2 GetCoords()
        {
            return coordinate;
        }
        public float GetCoordX()
        {
            return coordinate.X;
        }
        public float GetCoordY()
        {
            return coordinate.Y;
        }

        #region special Atribute
        
        public void setWaitingTime(float waitingTime)
        {
            this.waitingTime = waitingTime;
        }

        #endregion

        //check the waitingtime
        public bool HasToWait()
        {
            if(waitingTime == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Get the Time an Object has to wait on the final Waypoint position
        /// </summary>
        public float GetWaitingTime()
        {
            return waitingTime;
        }

        //overloaded functions to set coords and special atributes

        #region Constructor
        //overloaded constructor to set only coords or add some special atributes
        /// <summary>
        /// Create a Waypoint with default coords(0|0)
        /// </summary>
        public Waypoint()
        {
            coordinate = new Vector2(0, 0);
        }
        /// <summary>
        /// Create a Waypoint
        /// </summary>
        /// <param name="xCoord">X Coordinate for the Waypoint</param>
        /// <param name="yCoord">Y Coordinate for the Waypoint</param>
        public Waypoint(float xCoord, float yCoord)
        {
            coordinate = new Vector2(xCoord, yCoord);
        }
        #endregion
    }
}
