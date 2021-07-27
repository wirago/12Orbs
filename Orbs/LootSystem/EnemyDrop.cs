using Orbs.Interface;
using Orbs.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.LootSystem
{
    public class EnemyDrop
    {
        public Vector2 DropPosition { get; set; }
        public Rectangle BoundingBox { get; set; }
        public Rectangle InteractRectangle { get; private set; }
        private int interactRange = 10;
        private Item droppedItem;

        public EnemyDrop(Item droppedItem, Vector2 dropPosition)
        {
            this.droppedItem = droppedItem;
            DropPosition = dropPosition;
            BoundingBox = new Rectangle((int)DropPosition.X, (int)DropPosition.Y, droppedItem.IconTexture.Width / 2,  droppedItem.IconTexture.Height / 2);
            InteractRectangle = new Rectangle(BoundingBox.X - interactRange, BoundingBox.Y - interactRange, BoundingBox.Width + interactRange * 2, BoundingBox.Height + interactRange * 2);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(droppedItem.IconTexture, DropPosition, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        public void CollectDrop(EnemyDrop drop)
        {
            if (!Player.PlayerPositionRectangle.Intersects(InteractRectangle))
                return;

            UIManager.charWindow.AddItemToInventory(drop.droppedItem);
            QuestSystem.QuestManager.HandleItemPickup(drop.droppedItem.Id);
            WorldObjects.enemyDrops.Remove(drop);
        }
    }
}
