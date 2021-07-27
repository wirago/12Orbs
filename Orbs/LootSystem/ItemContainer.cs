using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Orbs.Interface;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.LootSystem
{
    public class ItemContainer
    {
        private bool isLooted;
        private List<Item> loot = new List<Item>();
        private string id;
        private int lootRange = 20;
        public Rectangle positionRectangle;
        public Rectangle lootRectangle;

        private Texture2D closedState;
        private Texture2D openedState;

        [Obsolete]
        public ItemContainer(Rectangle position, string id)
        {
            positionRectangle = position;
            lootRectangle = new Rectangle(position.X - lootRange, position.Y - lootRange, position.Width + lootRange * 2, position.Height + lootRange * 2);
            isLooted = false;
            this.id = id;
            FillContainer();
        }

        public ItemContainer(Rectangle position, string id, string openedStateSource, string closedStateSource)
        {
            positionRectangle = position;
            isLooted = false;
            this.id = id;
            closedState = Orbs.content.Load<Texture2D>(@"Textures\misc\" + closedStateSource);
            openedState = Orbs.content.Load<Texture2D>(@"Textures\misc\" + openedStateSource);

            positionRectangle.Width = openedState.Width;
            positionRectangle.Height = openedState.Height;
            Tiled.TileMap.collisionObjects.Add(positionRectangle);

            lootRectangle = new Rectangle(position.X - lootRange, position.Y - lootRange, positionRectangle.Width + lootRange * 2, positionRectangle.Height + lootRange * 2);

            FillContainer();
        }

        private void FillContainer()
        {
            Random rand = new Random();

            for (int i = rand.Next(1, 3); i > 0; i--)
                loot.Add(LootTable.consumables[rand.Next(0, LootTable.consumables.Count)]);
    
            foreach(Item i in loot)
                Console.WriteLine("Container " + id + " filled with: " + i.Name);
        }

        public void LootContainer()
        {
            if (lootRectangle.Intersects(Player.PlayerPositionRectangle))
            {
                if (!isLooted)
                {
                    StringBuilder sb = new StringBuilder();
                    Orbs.soundEffects.PlaySound("chestOpen", 0.5f);
                    
                    if(loot.Count == 0)
                    {
                        UIManager.ShowNotification("Nothing to loot.", new Vector2(10, 300));
                        loot.Clear();
                        isLooted = true;
                        return;
                    }

                    if(Player.inventory.Count + loot.Count > Player.maxInventorySize)
                    {
                        UIManager.ShowNotification("Inventory Full!");
                        return;
                    }

                    sb.Append("You have looted:\n");
                    foreach (Item i in loot)
                    {
                        sb.Append(i.Name + "\n");
                        UIManager.charWindow.AddItemToInventory(i);
                    }
                    UIManager.ShowNotification(sb.ToString());
                    loot.Clear();
                    isLooted = true;
                }
                else
                {
                    UIManager.ShowNotification("Already Looted!");
                    return;
                }
            }
            else
            {
                UIManager.ShowAlert("containerNotInRange", "Not in range!");
                return;
            }
        }

        public void DrawStatus()
        {
            Orbs.spriteBatch.DrawString(Orbs.spriteFontDefault, "Id: " + id + " Looted: " + isLooted.ToString(), 
                new Vector2(positionRectangle.X, positionRectangle.Y), Color.Black);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isLooted)
                spriteBatch.Draw(openedState, positionRectangle, Color.White);
            else
                spriteBatch.Draw(closedState, positionRectangle, Color.White);
        }
    }
}
