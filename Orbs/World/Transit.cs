using Orbs.Interface;
using Microsoft.Xna.Framework;
using Orbs.Tiled;
using Orbs.Effects;

namespace Orbs.World
{
    public class Transit
    {
        private string destination;
        private string destinationName;
        public Rectangle positionRectangle;
        private string targetPosition = "";

        public Transit(Rectangle position, string destination, string destinationName)
        {
            positionRectangle = position;
            this.destination = destination;
            this.destinationName = destinationName;
        }

        public Transit(Rectangle position, string destination, string destinationName, string transitionTag)
        {
            positionRectangle = position;
            this.destination = destination;
            this.destinationName = destinationName;
            this.targetPosition = transitionTag;
        }

        public void enterTarget()
        {
            changeMap(destination);
        }

        public void changeMap(string destination)
        {
            if (Player.notificationUp)
                return;

            ClearLevelData();
            MapLoader.changeMap(destination.ToLower(), targetPosition);
        }

        /// <summary> 
        /// Clears map specific data 
        /// </summary>
        private void ClearLevelData()
        {
            WorldCharacters.enemyWorldCharacters.Clear();
            WorldCharacters.standardWorldCharacters.Clear();
            WorldObjects.interactionObjects.Clear();
            WorldObjects.lootableContainers.Clear();
            WorldObjects.animatedObjects.Clear();
            WorldObjects.transits.Clear();

            EffectManager.ClearLights();

            TileMap.collisionObjects.Clear();
            TileMap.lootableObjects.Clear();
        }
    }
}
