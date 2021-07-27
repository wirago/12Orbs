using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.LootSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbs.Interface
{
    public class UITradeWindow
    {
        private Texture2D tradeWindow = Orbs.content.Load<Texture2D>(@"Userinterface\tradewindow");
        private Vector2 tradeWindowPosition;
        private Vector2 merchantNamePosition;
        private Vector2 firstMerchantItemSlotOffset = new Vector2(65, 97);
        private Vector2 firstPlayerItemSlotOffset = new Vector2(472, 100);
        private Vector2 playerMoneyOffset = new Vector2(475, 305);
        private List<Item> offeredItems;
        private string merchant;

        private Texture2D mouseCursorBag = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\cursor_bag");

        public Dictionary<Rectangle, Item> merchantItems = new Dictionary<Rectangle, Item>();
        public Dictionary<Rectangle, Item> playerItems = new Dictionary<Rectangle, Item>();
        public Vector2 initiatedFromWorldPosition;
        
        public UITradeWindow(string merchant, string items, Vector2 initiatedFromWorldPosition)
        {
            tradeWindowPosition = new Vector2((Orbs.graphics.PreferredBackBufferWidth - tradeWindow.Width) / 2, (Orbs.graphics.PreferredBackBufferHeight - tradeWindow.Height) / 2);
            merchantNamePosition = tradeWindowPosition + new Vector2(165, 30);

            this.merchant = merchant;
            this.initiatedFromWorldPosition = initiatedFromWorldPosition;
            offeredItems = GetItemsByIds(items);

            SetMerchantItemSlots();
            SetPlayerItemSlots();
        }

        private List<Item> GetItemsByIds(string ids)
        {
            string[] itemIds = ids.Split(',').ToArray();
            List<Item> returnValue = new List<Item>();

            //TODO: this is going to crash so hard at some point
            foreach (string s in itemIds)
                returnValue.Add(LootTable.consumables.Where(x => x.Id == s).First());

            return returnValue;
        }

        private void SetPlayerItemSlots()
        {
            playerItems.Clear();

            int curRow;
            Vector2 curPosition;
            for (int i = 0; i <= Player.inventory.Count - 1; i++)
            {
                curRow = i / 4;

                curPosition = new Vector2(
                    Player.inventory[i].IconTexture.Width * (i % 4) + 4 * (i % 4),
                    Player.inventory[i].IconTexture.Height * curRow + 2 * curRow)
                    + tradeWindowPosition + firstPlayerItemSlotOffset;

                playerItems.Add(
                    new Rectangle((int)curPosition.X, (int)curPosition.Y, Player.inventory[i].IconTexture.Width,
                    Player.inventory[i].IconTexture.Height), Player.inventory[i]);
            }
        }

        private void SetMerchantItemSlots()
        {
            int curRow;
            Vector2 curPosition;
            for (int i = 0; i <= offeredItems.Count - 1; i++)
            {
                curRow = i / 4;

                curPosition = new Vector2(
                    offeredItems[i].IconTexture.Width * i + 4 * (i % 4),
                    offeredItems[i].IconTexture.Height * curRow + 2 * curRow)
                    + tradeWindowPosition + firstMerchantItemSlotOffset;

                merchantItems.Add(
                    new Rectangle((int)curPosition.X, (int)curPosition.Y, offeredItems[i].IconTexture.Width, 
                    offeredItems[i].IconTexture.Height), offeredItems[i]);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 mouseWindowPos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();

            spriteBatch.Draw(tradeWindow, tradeWindowPosition, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, merchant, merchantNamePosition, Color.Black);
            spriteBatch.DrawString(Orbs.spriteFontDefault, Localization.strings.ui_purse + " " + Player.money + " Glaani", playerMoneyOffset + tradeWindowPosition, Color.Black);

            foreach (KeyValuePair<Rectangle, Item> horst in merchantItems)
                spriteBatch.Draw(horst.Value.IconTexture, horst.Key, Color.White);

            foreach (KeyValuePair<Rectangle, Item> item in playerItems)
                spriteBatch.Draw(item.Value.IconTexture, item.Key, Color.White);

            bool hovered = false;

            foreach (KeyValuePair<Rectangle, Item> merchItem in merchantItems)
            {
                if (merchItem.Key.Contains(mouseWindowPos))
                {
                    DrawStatus(spriteBatch, merchItem.Value.Name + ", " + merchItem.Value.Value + " GL", mouseWindowPos);
                    Microsoft.Xna.Framework.Input.Mouse.SetCursor(Microsoft.Xna.Framework.Input.MouseCursor.FromTexture2D(mouseCursorBag,0,0));
                    hovered = true;
                    break;
                }                   
            }

            foreach (KeyValuePair<Rectangle, Item> playerItem in playerItems)
            {
                if (playerItem.Key.Contains(mouseWindowPos))
                {
                    DrawStatus(spriteBatch, playerItem.Value.Name + ", " + playerItem.Value.Value + " GL", mouseWindowPos);
                    Microsoft.Xna.Framework.Input.Mouse.SetCursor(Microsoft.Xna.Framework.Input.MouseCursor.FromTexture2D(mouseCursorBag, 0, 0));
                    hovered = true;
                    break;
                }
            }

            if(!hovered)
                Microsoft.Xna.Framework.Input.Mouse.SetCursor(Microsoft.Xna.Framework.Input.MouseCursor.FromTexture2D(Orbs.mouseCursorDefault, 0, 0));
        }

        private void DrawStatus(SpriteBatch spriteBatch, string name, Vector2 mouseWindowPos)
        {
            Vector2 displayTextSize = Orbs.spriteFontDefault.MeasureString(name);
            int x = (int)mouseWindowPos.X + 10;
            int y = (int)mouseWindowPos.Y - 20;

            spriteBatch.Draw(UIManager.charLabelBackGround, new Rectangle(x, y, (int)displayTextSize.X, (int)displayTextSize.Y), Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, name, new Vector2(x, y), Color.Black);
        }

        public void HandleRightClick(Vector2 mouseWindowPos)
        {
            //Buy Item
            foreach (KeyValuePair<Rectangle, Item> merchItem in merchantItems)
            {
                if (merchItem.Key.Contains(mouseWindowPos))
                {
                    if(Player.money - merchItem.Value.Value < 0)
                    {
                        UIManager.ShowAlert("notEnoughMoney", Localization.strings.system_NotEnoughMoney);
                        return;
                    }

                    if(Player.inventory.Count >= Player.maxInventorySize)
                    {
                        UIManager.ShowAlert("notEnoughMoney", Localization.strings.system_InventoryFull);
                        return;
                    }

                    Player.AddItemToInventory(merchItem.Value.Id);
                    Player.money -= merchItem.Value.Value;
                    Orbs.soundEffects.PlaySound(@"Effects\coinsBuySell", 1.0f);

                    SetPlayerItemSlots();
                    return;
                }
            }

            //Sell Item
            foreach (KeyValuePair<Rectangle, Item> playerItem in playerItems)
            {
                if (playerItem.Key.Contains(mouseWindowPos))
                {
                    Player.RemoveItemFromInventory(playerItem.Value.Id);
                    Player.money += playerItem.Value.Value;
                    Orbs.soundEffects.PlaySound(@"Effects\coinsBuySell", 1.0f);

                    SetPlayerItemSlots();
                    return;
                }
            }
        }
    }
}
