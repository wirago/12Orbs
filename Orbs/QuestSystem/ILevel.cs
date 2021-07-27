using Microsoft.Xna.Framework;

namespace Orbs.QuestSystem
{
    public interface ILevel
    {
        void InteractWithNPC(string name);

        void InteractWithObject(string iObject);

        void HandleItemPickup(string itemName);

        void HandleTextInput(string text);

        string[] HandleEnemyKill(string enemyType);

        void SpawnObject(string itemId, string textureName, int width, int height, string objectName, Vector2 spawnPosition, string objectType);

        void Update(GameTime gameTime);

        void HandleNPCProximity(string npcId);
    }
}
