using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Orbs.LootSystem;
using System;
using Orbs.QuestSystem;
using System.Linq;

namespace Orbs.Interface
{
    public class UICharWindow : UIElement
    {
        public Dictionary<int, Rectangle> inventorySlotRectangles = new Dictionary<int, Rectangle>();
        public Dictionary<int, Rectangle> orbInventorySlotRectangles = new Dictionary<int, Rectangle>();

        private const int SLOTOFFSET = 4;
        private int inventoryColumns = 4;
        private const int ICONSIZE = 64;
        private Vector2 firstInventorySlotPos = new Vector2(57, 287);
        private Vector2 firstOrbInventorySlotPos = new Vector2(48, 22);
        private Vector2 selectedOrbPosition = new Vector2(58, 158);
        private Vector2 selectedWeaponPosition = new Vector2(58, 91);
        private Vector2 moneyPosition;

        public bool orbsInventoryOpen = false;
        private UIButton orbsInventoryButtonOpen;
        private UIButton orbsInventoryButtonClose;
        private Texture2D orbsInventoryTexture;
        private Vector2 orbsInventoryPosition = new Vector2(-158, 24); //offset from inventory
        private Vector2 mouseWindowPos = Vector2.Zero;

        private Texture2D playerPortrait;
        private Vector2 playerPortraitPosition;

        public static bool destroyWarning = false;

        public UICharWindow(string id, string source, Vector2 position) : base(id, source, position)
        {
            UITexturePosition.X = (Orbs.graphics.PreferredBackBufferWidth - UITexture.Width) / 2;
            UITexturePosition.Y = (Orbs.graphics.PreferredBackBufferHeight - UITexture.Height) / 2;

            UITextureBoundingbox.X = (int)UITexturePosition.X;
            UITextureBoundingbox.Y = (int)UITexturePosition.Y;

            orbsInventoryTexture = Orbs.content.Load<Texture2D>(@"UserInterface\orbs_grid");
            orbsInventoryButtonOpen = new UIButton("orbs", new Vector2(UITexturePosition.X + 34, UITexturePosition.Y + 195), 20, 75);
            orbsInventoryPosition = UITexturePosition + orbsInventoryPosition;

            orbsInventoryButtonClose = new UIButton("orbs", new Vector2(orbsInventoryPosition.X + 26, orbsInventoryPosition.Y + 170), 20, 75);
            selectedOrbPosition = UITexturePosition + selectedOrbPosition;
            selectedWeaponPosition = UITexturePosition + selectedWeaponPosition;

            playerPortrait = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\hero_male");
            playerPortraitPosition = UITexturePosition + new Vector2(131, 84); //portrait offset

            moneyPosition = UITexturePosition + new Vector2(56, 260);

            SetInventorySlotRectangles();
            SetOrbInventorySlotRectangles();
        }

        public void SetInventorySlotRectangles()
        {
            int curRow = 0;
            for (int i = 0; i < Player.maxInventorySize; i++)
            {
                if (i > 3)
                    curRow = 1;
                if (i > 7)
                    curRow = 2;

                inventorySlotRectangles[i] =
                    new Rectangle(
                        (int)UITexturePosition.X + SLOTOFFSET + ICONSIZE * (i % inventoryColumns) + 2 * (i % inventoryColumns) + (int)firstInventorySlotPos.X,
                        (int)UITexturePosition.Y + SLOTOFFSET + ICONSIZE * curRow + 2 * curRow + (int)firstInventorySlotPos.Y,
                        ICONSIZE,
                        ICONSIZE
                        );
            }
        }

        public void SetOrbInventorySlotRectangles()
        {
            int curRow = 0;
            for (int i = 0; i < 12; i++)
            {
                curRow = i / 2;

                orbInventorySlotRectangles[i] =
                    new Rectangle(
                        (int)orbsInventoryPosition.X + (int)firstOrbInventorySlotPos.X + ICONSIZE * (i % 2) + SLOTOFFSET * (i % 2),
                        (int)orbsInventoryPosition.Y + (int)firstOrbInventorySlotPos.Y + ICONSIZE * curRow + SLOTOFFSET * curRow,
                        ICONSIZE,
                        ICONSIZE
                        );
            }
        }

        private void ToggleOrbsInventory()
        {
            orbsInventoryOpen = !orbsInventoryOpen;
            Orbs.soundEffects.PlaySound("mouseclick", 1.0f);
        }

        public string CheckMouseClick(Vector2 mouseWindowPosition)
        {
            if (orbsInventoryButtonOpen.UITextureBoundingbox.Contains(mouseWindowPosition))
            {
                ToggleOrbsInventory();
                return "toggle";
            }

            if (orbsInventoryOpen)
            {
                if (orbsInventoryButtonClose.UITextureBoundingbox.Contains(mouseWindowPosition))
                {
                    ToggleOrbsInventory();
                    return "toggle";
                }

                foreach (KeyValuePair<int, Rectangle> r in orbInventorySlotRectangles)
                {
                    if (r.Value.Contains(mouseWindowPosition))
                    {
                        SelectOrb(r.Key);
                        return "orb";
                    }
                }
            }

            return "";
        }

        private void SelectOrb(int orbNumber)
        {
            Player.selectedOrb = Player.claimedOrbs[orbNumber];
            if(Player.selectedOrb != null)
                if(LootTable.glyphs.Where(s => s.UseEffect == Player.selectedOrb.Id).Any())
                    UIManager.actionBar["actionbar_2"] = LootTable.glyphs.Where(s => s.UseEffect == Player.selectedOrb.Id).First();
        }

        public bool HasItem(string id)
        {
            return Player.inventory.Any(item => item.Id == id);
        }

        public void DragAndDropItem(Vector2 mouseWindowPosition, bool buttonPressed)
        {
            //select item to drag
            if (buttonPressed && Orbs.selectedInventorySlot < 0)
            {
                foreach (KeyValuePair<int, Rectangle> slot in inventorySlotRectangles)
                {
                    if (slot.Value.Contains(mouseWindowPosition))
                        Orbs.selectedInventorySlot = slot.Key;
                }
            }

            if (!buttonPressed && Orbs.selectedInventorySlot >= 0)
            {
                foreach (var actionBarSlot in UIManager.actionbarButtons)
                {
                    if (actionBarSlot.UITextureBoundingbox.Contains(mouseWindowPosition))
                    {
                        if (actionBarSlot.Id.StartsWith("actionbar_"))
                        {
                            Item selectedItem = Player.inventoryGrid[Orbs.selectedInventorySlot];
                            UIManager.BindItemToActionbar(selectedItem, actionBarSlot.Id);

                            Orbs.selectedInventorySlot = -1;
                            return;
                        }
                        else
                        {
                            Orbs.selectedInventorySlot = -1;
                            return;
                        }
                    }
                }


                //Dropped outside Inventory
                if (!UITextureBoundingbox.Contains(mouseWindowPosition) || destroyWarning == true)
                {
                    Item selectedItem = Player.inventoryGrid[Orbs.selectedInventorySlot];

                    if(selectedItem != null)
                        if (selectedItem.isQuestItem && Player.claimedOrbs[0] != null)
                        {
                            UIManager.ShowNotification(Localization.strings.system_isQuestItem);

                            Orbs.selectedInventorySlot = -1;
                            destroyWarning = false;
                            return;
                        }

                    if (!destroyWarning)
                    {
                        UIManager.ShowYesNoNotification(Localization.strings.system_DestroyItem);
                        destroyWarning = true;
                    }

                    //Item will be destroyed
                    if (Player.notificationResponse == 1)
                    {
                        RemoveItemFromInventory(Orbs.selectedInventorySlot);
                        destroyWarning = false;
                        UIManager.ClearNotifications();
                        
                        return;
                    }

                    //Item will NOT be destroyed
                    if (Player.notificationResponse == 0)
                    {
                        Orbs.selectedInventorySlot = -1;
                        destroyWarning = false;
                        UIManager.ClearNotifications();
                        return;
                    }
                }

                if(!destroyWarning)
                    foreach (KeyValuePair<int, Rectangle> slot in inventorySlotRectangles)
                    {
                        //Drag Item to empty slot
                        if (slot.Value.Contains(mouseWindowPosition) && Player.inventoryGrid[slot.Key] == null)
                        {
                            Player.inventoryGrid[slot.Key] = Player.inventoryGrid[Orbs.selectedInventorySlot];
                            Player.inventoryGrid[Orbs.selectedInventorySlot] = null;
                            Orbs.selectedInventorySlot = -1;
                            Orbs.soundEffects.PlaySound("cloth-inventory", 1.0f);
                            return;
                        }

                        //Exchange Itemslots
                        if (slot.Value.Contains(mouseWindowPosition) && Player.inventoryGrid[slot.Key] != null)
                        {
                            Item temp = Player.inventoryGrid[slot.Key];
                            Player.inventoryGrid[slot.Key] = Player.inventoryGrid[Orbs.selectedInventorySlot];
                            Player.inventoryGrid[Orbs.selectedInventorySlot] = temp;
                            Orbs.soundEffects.PlaySound("cloth-inventory", 1.0f);
                            Orbs.selectedInventorySlot = -1;
                            return;
                        }
                    }
            }

        }

        public void AddItemToInventory(string itemId)
        {
            Item itemToAdd = null;

            if (LootTable.questItems.Any(i => i.Id == itemId))
                itemToAdd = LootTable.questItems.Where(item => item.Id == itemId).First();

            if (itemToAdd == null && LootTable.armor.Any(i => i.Id == itemId))
                itemToAdd = LootTable.armor.Where(item => item.Id == itemId).First();

            if (itemToAdd == null && LootTable.consumables.Any(i => i.Id == itemId))
                itemToAdd = LootTable.consumables.Where(item => item.Id == itemId).First();

            if (itemToAdd == null && LootTable.weapons.Any(i => i.Id == itemId))
            {
                if (Player.HasItem(itemId))
                {
                    UIManager.ShowAlert("weapon already exists", "Weapon already in inventory!");
                    return;
                }
                itemToAdd = LootTable.weapons.Where(item => item.Id == itemId).First();
            }

            if (itemToAdd == null && LootTable.materials.Any(i => i.Id == itemId))
                itemToAdd = LootTable.materials.Where(item => item.Id == itemId).First();

            if (itemToAdd == null && LootTable.miscItems.Any(i => i.Id == itemId))
                itemToAdd = LootTable.miscItems.Where(item => item.Id == itemId).First();
            
            if(itemToAdd != null)
                AddItemToInventory(itemToAdd);
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            if (itemToAdd == null)
                return;

            if(itemToAdd.Id.StartsWith("orb"))
            {
                AddOrbToInventory(itemToAdd);
                return;
            }

            if (Player.inventory.Count >= Player.maxInventorySize)
            {
                UIManager.ShowNotification("Inventory Full!");
                return;
            }

            Player.inventory.Add(itemToAdd);

            //search empty inventory slot an add item
            for (int i = 0; i < Player.maxInventorySize; i++)
            {
                if (Player.inventoryGrid[i] == null)
                {
                    Player.inventoryGrid[i] = itemToAdd;
                    QuestManager.HandleItemPickup(itemToAdd.Name);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Adds given orb to the orb inventory. Parses the position (0-11) from the Id of the orb 
        /// </summary>
        /// <param name="orbToAdd"></param>
        private void AddOrbToInventory(Item orbToAdd)
        {
            int orbNumber;

            //Check if number of the orb is 1 or 2 digits long
            if(orbToAdd.Id[orbToAdd.Id.IndexOf("_") + 2] != '_')
                orbNumber = Int32.Parse(orbToAdd.Id.Substring(orbToAdd.Id.IndexOf("_") + 1, 2));
            else
                orbNumber = Int32.Parse(orbToAdd.Id.Substring(orbToAdd.Id.IndexOf("_") + 1, 1));

            Player.claimedOrbs[orbNumber - 1] = orbToAdd;
        }

        public void RemoveItemFromInventory(int inventorySlot)
        {
            string itemId = Player.inventoryGrid[inventorySlot].Id;
            Player.inventory.Remove(Player.inventoryGrid[inventorySlot]);
            Player.inventoryGrid[inventorySlot] = null;

            for(int i = UIManager.actionBar.Count - 1; i >= 0; i--)
            {
                if (UIManager.actionBar.ElementAt(i).Value != null)
                    if (UIManager.actionBar.ElementAt(i).Value.Id == itemId)
                        UIManager.UnbindItemFromActionbar(UIManager.actionBar.ElementAt(i).Key);
            }

            Orbs.selectedInventorySlot = -1;
        }

        public void RemoveItemFromInventory(string itemId)
        {
            int inventorySlot = Player.inventoryGrid.Where(i => i.Value != null && i.Value.Id == itemId).First().Key;
            Player.inventory.Remove(Player.inventoryGrid[inventorySlot]);
            Player.inventoryGrid[inventorySlot] = null;

            for (int i = UIManager.actionBar.Count - 1; i >= 0; i--)
            {
                if (UIManager.actionBar.ElementAt(i).Value != null)
                    if (UIManager.actionBar.ElementAt(i).Value.Id == itemId)
                        UIManager.UnbindItemFromActionbar(UIManager.actionBar.ElementAt(i).Key);
            }

            Orbs.selectedInventorySlot = -1;
        }

        public void UseItem(Vector2 mouseWindowPosition)
        {
            foreach (KeyValuePair<int, Rectangle> slot in inventorySlotRectangles)
            {
                if (slot.Value.Contains(mouseWindowPosition))
                    Orbs.selectedInventorySlot = slot.Key;
            }
            if (Orbs.selectedInventorySlot < 0)
                return;

            Item item = Player.inventoryGrid[Orbs.selectedInventorySlot];

            if (item != null)
            {
                if(item.UseEffect == "equip")
                {
                    //TODO: check for item type
                    if (item.Type == Item.type.Weapon)
                        Player.EquipWeapon(item.Id);

                    UIManager.ShowAlert("equip", item.Name + " Equipped");
                }

                if (item.UseType == "textbox")
                    UIManager.ShowNotification(Localization.strings.ResourceManager.GetString(item.UseEffect));

                if (item.Type == Item.type.Consumable)
                    Player.itemToBeApplied = item.Id;
            }
                
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Drawing Items icons
            spriteBatch.Draw(playerPortrait, new Rectangle((int)playerPortraitPosition.X, (int)playerPortraitPosition.Y, 120, 185), Color.White);
            spriteBatch.Draw(UITexture, UITexturePosition, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, Localization.strings.ui_purse + " " + Player.money + " Glaani", moneyPosition, Color.Black);
            mouseWindowPos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();

            if (Player.inventory.Count > 0)
            {
                foreach (KeyValuePair<int, Item> item in Player.inventoryGrid)
                {
                    if (item.Value != null)
                        spriteBatch.Draw(item.Value.IconTexture, inventorySlotRectangles[item.Key], Color.White);
                }

                foreach (KeyValuePair<int, Item> item in Player.inventoryGrid)
                {
                    if (item.Value != null)
                    {
                        if (inventorySlotRectangles[item.Key].Contains(mouseWindowPos))
                            DrawStatus(spriteBatch, item.Value.Name);
                    }
                }
            }

            if(Player.selectedWeapon != null)
                spriteBatch.Draw(Player.selectedWeapon.IconTexture, selectedWeaponPosition, Color.White);

            if (orbsInventoryOpen)
            {
                spriteBatch.Draw(orbsInventoryTexture, orbsInventoryPosition, Color.White);

                foreach (KeyValuePair<int, Item> o in Player.claimedOrbs)
                {
                    if (o.Value != null)
                    {
                        spriteBatch.Draw(o.Value.IconTexture, orbInventorySlotRectangles[o.Key], Color.White);

                        if (orbInventorySlotRectangles[o.Key].Contains(mouseWindowPos))
                            DrawStatus(spriteBatch, o.Value.Name);
                    }
                }

                foreach (KeyValuePair<int, Item> o in Player.claimedOrbs)
                {
                    if (o.Value != null)
                    {
                        if (orbInventorySlotRectangles[o.Key].Contains(mouseWindowPos))
                            DrawStatus(spriteBatch, o.Value.Name);
                    }
                }
            }
                
            if(Player.selectedOrb != null)
                spriteBatch.Draw(Player.selectedOrb.IconTexture, selectedOrbPosition, Color.White);
        }

        private void DrawStatus(SpriteBatch spriteBatch, string name)
        {
            Vector2 displayTextSize = Orbs.spriteFontDefault.MeasureString(name);
            int x = (int)mouseWindowPos.X + 10;
            int y = (int)mouseWindowPos.Y - 20;

            spriteBatch.Draw(UIManager.charLabelBackGround, new Rectangle(x, y, (int)displayTextSize.X, (int)displayTextSize.Y), Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, name, new Vector2(x, y), Color.Black);

        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
