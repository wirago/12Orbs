using Orbs.LootSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Orbs.Interface.TextInput;
using System.Threading;
using System.Globalization;
using Microsoft.Xna.Framework.Input;

namespace Orbs.Interface
{
    public static class UIManager
    {
        public static List<UIButton> actionbarButtons = new List<UIButton>();
        public static List<UIButton> startMenuButtons = new List<UIButton>();
        public static List<UIButton> settingsMenuButtons = new List<UIButton>();
        public static List<UIButton> pauseMenuButtons = new List<UIButton>();

        public static UIButton startNewGameMaleButton;
        public static UIButton startNewGameFemaleButton;

        public static List<UITextBox> textboxes = new List<UITextBox>();
        public static List<UIElement> uiTextElements = new List<UIElement>();
        public static List<UICombatText> combatTexts = new List<UICombatText>();
        public static Dictionary<string, Item> actionBar = new Dictionary<string, Item>();
        public static UIDialogBox currentDialogBox = null;

        public static UICharWindow charWindow;
        public static UIElement userInterface;
        public static Texture2D charLabelBackGround = Orbs.content.Load<Texture2D>(@"Textures\CharLabelBackground");
        public static Texture2D selectionBarBackground = Orbs.content.Load<Texture2D>(@"Textures\SelectionBarBackground");

        public static UITradeWindow tradeWindow;

        public static bool charWindowOpen = false;
        public static bool questWindowOpen = false;
        public static bool mapWindowOpen = false;
        public static bool textInputOpen = false;
        public static bool weaponSelectionOpen = false;
        public static bool spellSelectionOpen = false;
        public static string selectedHero = "";

        private static Texture2D heroMaleSmall;
        private static Texture2D heroFemaleSmall;
        private static Vector2 heroSelectPosition;
        private static Vector2 NewHeroTextOffset = new Vector2(110, 240);

        private static Texture2D activationOverlay;
        private static bool actionBarClicked = false;
        private static string clickedActionBarSlot;
        private static TimeSpan clickedAt;

        private static Texture2D actionbarTexture;
        private static Rectangle actionbarSourceRectangle;
        private static Vector2 actionbarPosition;
        private static Vector2 charWindowPosition;

        private static Texture2D playerFrame;
        private static Vector2 playerFramePosition;
        private static Vector2 healthbarPosition;
        private static Rectangle healthbar_SourceRectangle;
        private static Rectangle manabar_SourceRectangle;
        private static Vector2 healthtextPostion;
        private static Texture2D statusbar_yellow;
        private static Vector2 manabarToHealthbarOffset = new Vector2(0, 42);

        private static Rectangle balanceBarYellowSourceRect; //Same Texturefile as actionbar
        private static Rectangle balanceBarPurpleSourceRect; //Same Texturefile as actionbar
        private static Texture2D balanceBar_bg;
        private static Vector2 balanceBarCenterPosition; //Screenpos of the Middle of the scale symbol
        private static Rectangle balanceBar_bgRect; //Relative to actionbar texture

        private const int ICONSIZE = 64;

        public static Texture2D gamePauseWindow;
        public static Texture2D gameMenuWindow;
        public static Vector2 gamePauseWindowPosition;
        public static Vector2 gameMenuWindowPosition;

        public static Texture2D pilarLeft;
        public static Texture2D pilarRight;
        public static Texture2D menuBackground;
        public static Texture2D defaultMenuButton;
        public static Texture2D orbsLogo;
        private static Vector2 orbsLogoPosition;

        private static TextBox textInput;
        private static Rectangle textInputPosition = new Rectangle(400, 400, 500, 100);

        public static List<UIEnemyNameplate> enemyNameplates = new List<UIEnemyNameplate>();
        private static Vector2 firstNameplatePosition = new Vector2(0, 300);

        private static Dictionary<Item, Rectangle> weaponSelectionSlots = new Dictionary<Item, Rectangle>();
        private static Dictionary<Spell, Rectangle> spellSelectionSlots = new Dictionary<Spell, Rectangle>();

        #region Buttonpostions
        private static Vector2 buttonPosition_Character;
        private static Vector2 buttonPosition_Quests;
        private static Vector2 buttonPosition_Map;
        private static Vector2 actionBarPosition_0;
        private static Vector2 actionBarPosition_1;
        private static Vector2 actionBarPosition_2;
        private static Vector2 actionBarPosition_3;
        #endregion

        public static void GenerateUI()
        {
            actionbarTexture = Orbs.content.Load<Texture2D>(@"UserInterface\interface");
            actionbarSourceRectangle = new Rectangle(0, 0, actionbarTexture.Width, 157);
            actionbarPosition = new Vector2((Orbs.graphics.PreferredBackBufferWidth - actionbarSourceRectangle.Width) / 2, Orbs.graphics.PreferredBackBufferHeight - actionbarSourceRectangle.Height);

            playerFrame = Orbs.content.Load<Texture2D>(@"UserInterface\player_healthbar");
            playerFramePosition = new Vector2(0, Orbs.graphics.PreferredBackBufferHeight - 99);
            healthbar_SourceRectangle = new Rectangle(86, 99, 275, 22);
            manabar_SourceRectangle = new Rectangle(86, 122, 275, 22);
            healthbarPosition = playerFramePosition + new Vector2(85, 25);
            healthtextPostion = new Vector2(
                healthbarPosition.X + healthbar_SourceRectangle.Width / 2 - Orbs.spriteFontDefault.MeasureString("100 / 100").X / 2,
                healthbarPosition.Y);

            statusbar_yellow = Orbs.content.Load<Texture2D>(@"UserInterface\Manabar_yellow");

            balanceBarYellowSourceRect = new Rectangle(34, 160, 226, 28);
            balanceBarPurpleSourceRect = new Rectangle(336, 160, 226, 28);
            balanceBar_bg = Orbs.content.Load<Texture2D>(@"UserInterface\healthbar_grey_px");
            balanceBarCenterPosition = new Vector2(280, 34) + actionbarPosition;
            balanceBar_bgRect = new Rectangle(22 + (int)actionbarPosition.X, 35 + (int)actionbarPosition.Y, 520, 28);


            actionBarPosition_0 = actionbarPosition + new Vector2(27, 75);
            actionBarPosition_1 = actionbarPosition + new Vector2(120, 75);
            actionBarPosition_2 = actionbarPosition + new Vector2(206, 75);
            actionBarPosition_3 = actionbarPosition + new Vector2(296, 75);

            buttonPosition_Character = actionbarPosition + new Vector2(383, 75);
            buttonPosition_Quests = actionbarPosition + new Vector2(470, 75);
            //buttonPosition_Map = actionbarPosition + new Vector2(829, 98);

            actionbarButtons.Add(new UIButton("button_Character", @"UserInterface\Icons\character", buttonPosition_Character));
            actionbarButtons.Add(new UIButton("button_Questlog", @"UserInterface\Icons\quests", buttonPosition_Quests));
            //actionbarButtons.Add(new UIButton("button_Map", @"UserInterface\Icons\map", buttonPosition_Map));

            //Dummy actionbar filling
            actionBar.Add("actionbar_0", LootTable.weapons.Where(w => w.Id == "w01").First());
            actionBar.Add("actionbar_1", LootTable.spells.Where(s => s.Id == "sf01").First());
            actionBar.Add("actionbar_2", LootTable.glyphs.Where(s => s.Id == "rn01").First());
            actionBar.Add("actionbar_3", LootTable.consumables.Where(c => c.Id == "c06").First());

            //Fill dummy actionbarslots 1-5 with textures from their item/spell
            actionbarButtons.Add(new UIButton("actionbar_0", actionBar["actionbar_0"].IconTexture, actionBarPosition_0, null, "actionbar_0", LabelOrientation.TopLeft));
            actionbarButtons.Add(new UIButton("actionbar_1", actionBar["actionbar_1"].IconTexture, actionBarPosition_1, null, "actionbar_1", LabelOrientation.TopLeft));
            actionbarButtons.Add(new UIButton("actionbar_2", actionBar["actionbar_2"].IconTexture, actionBarPosition_2, null, "actionbar_2", LabelOrientation.TopLeft));
            actionbarButtons.Add(new UIButton("actionbar_3", actionBar["actionbar_3"].IconTexture, actionBarPosition_3, null, "actionbar_3", LabelOrientation.TopLeft));

            charWindow = new UICharWindow("window_inventory", @"UserInterface\character", Vector2.Zero);
            charWindowPosition = new Vector2((Orbs.graphics.PreferredBackBufferWidth - charWindow.UITexture.Width) / 2, (Orbs.graphics.PreferredBackBufferHeight - charWindow.UITexture.Height) / 2);
                        gameMenuWindow = Orbs.content.Load<Texture2D>(@"UserInterface/menu_book");
            gameMenuWindowPosition = new Vector2((Orbs.graphics.PreferredBackBufferWidth - gameMenuWindow.Width) / 2, (Orbs.graphics.PreferredBackBufferHeight - gameMenuWindow.Height) / 2);
           
            activationOverlay = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Misc\activate");

            pilarLeft = Orbs.content.Load<Texture2D>(@"UserInterface\pilar_left");
            pilarRight = Orbs.content.Load<Texture2D>(@"UserInterface\pilar_right");
            menuBackground = Orbs.content.Load<Texture2D>(@"UserInterface\menu_background");
            defaultMenuButton = Orbs.content.Load<Texture2D>(@"UserInterface\button_0");
            orbsLogo = Orbs.content.Load<Texture2D>(@"UserInterface\orbs_logo");
            orbsLogoPosition = new Vector2(gameMenuWindowPosition.X + (gameMenuWindow.Width - orbsLogo.Width) / 2, gameMenuWindowPosition.Y);

            heroMaleSmall = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\hero_male_small");
            heroFemaleSmall = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\hero_female_small");
            heroSelectPosition = gameMenuWindowPosition + new Vector2(75, 300);

            GenerateStartMenuButtons();
            GenerateSettingsMenuButtons();
            GeneratePauseMenuButtons();
        }

        public static void GenerateStartMenuButtons()
        {
            startMenuButtons.Clear();

            startMenuButtons.Add(
                new UIButton("newGameButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 250, 200, 50), 
                Localization.strings.label_newGame, "newGame"));
            startMenuButtons.Add(
                new UIButton("loadButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 310, 200, 50), 
                Localization.strings.label_load, "load"));
            startMenuButtons.Add(
                new UIButton("settingsButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 370, 200, 50),
                Localization.strings.label_settings, "settings"));
            startMenuButtons.Add(
                new UIButton("creditsButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 430, 200, 50),
                Localization.strings.label_credits, "credits"));
            startMenuButtons.Add(
                new UIButton("quitButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 490, 200, 50),
                Localization.strings.label_quit, "quit"));
            startMenuButtons.Add(
                new UIButton("selectMale", heroMaleSmall, heroSelectPosition, "", "selectMale", null));
            startMenuButtons.Add(
                new UIButton("selectFemale", heroFemaleSmall, heroSelectPosition + new Vector2(200, 0), "", "selectFemale", null));

            startNewGameMaleButton = new UIButton(
                "newGameMale", new Rectangle((int)heroSelectPosition.X + 100, (int)heroSelectPosition.Y + 210, 200, 50), 
                Localization.strings.menu_startGame, "newGameMale");
            startNewGameFemaleButton = new UIButton(
                "newGameFemale", new Rectangle((int)heroSelectPosition.X + 100, (int)heroSelectPosition.Y + 210, 200, 50),
                Localization.strings.menu_startGame, "newGameFemale");

        }

        public static void GenerateSettingsMenuButtons()
        {
            settingsMenuButtons.Clear();

            settingsMenuButtons.Add(new UIButton("toggleLanguage", new Rectangle((int)gameMenuWindowPosition.X + 100, (int)gameMenuWindowPosition.Y + 250, 200, 50),
                Localization.strings.label_language, "toggleLanguage"));
            settingsMenuButtons.Add(new UIButton("toggleMusic", new Rectangle((int)gameMenuWindowPosition.X + 100, (int)gameMenuWindowPosition.Y + 310, 200, 50),
                Localization.strings.label_music, "toggleMusic"));
            settingsMenuButtons.Add(new UIButton("toggleSound", new Rectangle((int)gameMenuWindowPosition.X + 100, (int)gameMenuWindowPosition.Y + 370, 200, 50),
                Localization.strings.label_sound, "toggleSound"));
            settingsMenuButtons.Add(new UIButton("back", new Rectangle((int)gameMenuWindowPosition.X + 100, (int)gameMenuWindowPosition.Y + 430, 200, 50),
                Localization.strings.label_apply, "back"));
        }

        public static void GeneratePauseMenuButtons()
        {
            pauseMenuButtons.Clear();

            pauseMenuButtons.Add(
                new UIButton("resume", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 230, 200, 50),
                Localization.strings.label_resume, "resume"));
            pauseMenuButtons.Add(
                new UIButton("newGameButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 290, 200, 50),
                Localization.strings.label_newGame, "newGame"));
            pauseMenuButtons.Add(
                new UIButton("save", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 350, 200, 50),
                Localization.strings.label_save, "save"));
            pauseMenuButtons.Add(
                new UIButton("load", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 410, 200, 50),
                Localization.strings.label_load, "load"));
            pauseMenuButtons.Add(
                new UIButton("settings", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 470, 200, 50),
                Localization.strings.label_settings, "settings"));
            pauseMenuButtons.Add(
                new UIButton("quitButton", new Rectangle((int)gameMenuWindowPosition.X + 570, (int)gameMenuWindowPosition.Y + 530, 200, 50),
                Localization.strings.label_quit, "quit"));
        }

        public static string GetKeyLabel(Keys button)
        {
            if (button.ToString().Length > 1)
                return (button.ToString().Substring(1));

            return button.ToString();
        }

        public static string actionToBeTriggered(string actionBarSlot)
        {
            actionBarClicked = true;
            clickedActionBarSlot = actionBarSlot;
            clickedAt = Orbs.currentTimeSpan;
            return actionBar[actionBarSlot] == null ? "" : actionBar[actionBarSlot].Id;
        }

        public static void BindItemToActionbar(Item item, string targetSlot)
        {
            if (targetSlot == "actionbar_0" && !item.Id.StartsWith("w")) //only weapons allowed in weaponslot
                return;

            int targetIndex = actionbarButtons.IndexOf(actionbarButtons.Where(slot => slot.Id == targetSlot).First());
            actionbarButtons[targetIndex] = new UIButton(targetSlot, item, actionbarButtons[targetIndex].UITexturePosition);
            actionBar[targetSlot] = item;
        }

        public static void UnbindItemFromActionbar(string targetSlot)
        {
            int targetIndex = actionbarButtons.IndexOf(actionbarButtons.Where(slot => slot.Id == targetSlot).First());
            actionbarButtons[targetIndex] = new UIButton(targetSlot, actionbarButtons[targetIndex].UITexturePosition); //emtpy button
            actionBar[targetSlot] = null;
        }

        public static string CheckMouseClick(Vector2 mouseWorldPos, Vector2 mouseWindowPos)
        {
            if (Orbs.curGameState == GameState.Menu || Orbs.curGameState == GameState.NewGame)
            {
                foreach (UIButton button in startMenuButtons)
                {
                    if (button.IsClicked(mouseWindowPos))
                    {
                        Orbs.soundEffects.PlaySound("mouseclick", 0.5f);
                        return button.onClickAction;
                    }
                }

                if(selectedHero == "female" && startNewGameFemaleButton.IsClicked(mouseWindowPos))
                {
                    Orbs.soundEffects.PlaySound("mouseclick", 0.5f);
                    return startNewGameFemaleButton.onClickAction;
                }

                if (selectedHero == "male" && startNewGameMaleButton.IsClicked(mouseWindowPos))
                {
                    Orbs.soundEffects.PlaySound("mouseclick", 0.5f);
                    return startNewGameMaleButton.onClickAction;
                }
            }

            if(Orbs.curGameState == GameState.Settings)
            {
                foreach (UIButton button in settingsMenuButtons)
                {
                    if (button.IsClicked(mouseWindowPos))
                    {
                        Orbs.soundEffects.PlaySound("mouseclick", 0.5f);
                        return button.onClickAction;
                    }
                }
            }

            if (Orbs.curGameState == GameState.Paused)
            {
                foreach (UIButton button in pauseMenuButtons)
                {
                    if (button.IsClicked(mouseWindowPos))
                    {
                        Orbs.soundEffects.PlaySound("mouseclick", 0.5f);
                        return button.onClickAction;
                    }
                }
            }

            foreach (var b in actionbarButtons)
            {
                if(b != null)
                    if (b.UITextureBoundingbox.Contains(mouseWindowPos))
                    {
                        Orbs.soundEffects.PlaySound("mouseclick", 1.0f);

                        if (b.Id == "actionbar_0")
                            return "weaponSelection";

                        if (b.Id == "actionbar_1")
                            return "spellSelection";

                        return b.Id;
                    }
            }

            if (currentDialogBox != null)
            {
                if (currentDialogBox.okButton.UITextureBoundingbox.Contains(mouseWindowPos))
                {
                    if (UIDialogBox.isLastText)
                    {
                        currentDialogBox.toBeRemoved = true;
                        UIDialogBox.isLastText = false;
                    }
                    else
                        UIDialogBox.nextText = true;
                }

                return "dialogbox";
            }

            if (weaponSelectionOpen)
            {
                string selectedWeapon = CheckWeaponSelectionClick(mouseWindowPos);
                if (selectedWeapon != null)
                {
                    Player.EquipWeapon(selectedWeapon);
                    return null;
                }
            }

            if (spellSelectionOpen)
            {
                string selectedSpell = CheckSpellSelectionClick(mouseWindowPos);
                if (selectedSpell != null)
                {
                    Player.EquipSpell(selectedSpell);
                    return null;
                }
            }

            return null;
        }

        public static int CheckNotificationClick()
        {
            int returnValue = 0;

            foreach (UITextBox t in textboxes)
            {
                if (t.HandleMousClick() == 1)
                    returnValue = 1;
            }

            if(textboxes.Count > 0)
               if (!textboxes[0].isActive)
                    ClearNotifications();

            return returnValue;
        }

        public static void ClearNotifications()
        {
            textboxes.Clear();
            Player.notificationUp = false;

            if(!UICharWindow.destroyWarning)
                Player.notificationResponse = -1;
        }

        public static void ToggleWindowById(string id)
        {
            if (id == "button_Character")
            {
                if (tradeWindow != null)
                    tradeWindow = null;

                charWindowOpen = !charWindowOpen;
            }

            if (id == "button_Questlog")
            {
                if (questWindowOpen)
                {
                    questWindowOpen = false;
                    return;
                }

                if(!questWindowOpen)
                {
                    if (tradeWindow != null)
                        tradeWindow = null;

                    QuestSystem.QuestManager.GetQuestTitlePositions();
                    questWindowOpen = true;
                }

                Orbs.soundEffects.PlaySound("turn_page", 1.0f);
            }

            if (id == "button_Map")
            {
                if (tradeWindow != null)
                    tradeWindow = null;

                mapWindowOpen = !mapWindowOpen;
            }
        }

        public static void ShowDialogWindow(string charLeft, string targetName, string charRight, List<string> content)
        {
            currentDialogBox = new UIDialogBox(charLeft, targetName, charRight, content);
        }

        /// <summary>
        /// Shows Notificationbox with OK-Button
        /// </summary>
        /// <param name="content"></param>
        public static void ShowNotification(string content)
        {
            textboxes.Add(new UITextBox(content));
            Player.notificationUp = true;
        }

        /// <summary>
        /// Shows Notificationbox with OK-Button at specified position
        /// </summary>
        /// <param name="content"></param>
        /// <param name="position"></param>
        public static void ShowNotification(string content, Vector2 position)
        {
            textboxes.Add(new UITextBox(content, position));
            Player.notificationUp = true;
        }

        /// <summary>
        /// Shows Notificationbox with question on top, no buttons
        /// </summary>
        /// <param name="content"></param>
        /// <param name="position"></param>
        public static void ShowQuestion(string content, Vector2 position)
        {
            textboxes.Add(new UITextBox(content, content, position));
            Player.notificationUp = true;
        }

        /// <summary>
        /// Shows Story Questbox with Ok-Button
        /// </summary>
        /// <param name="content">Story Text</param>
        /// <param name="contentPosition">Contentposition relative to Textwindow position</param>
        /// <param name="image">Image of the Storybox</param>
        public static void ShowStoryTextbox(string content, Vector2 contentPosition, string image)
        {
            textboxes.Add(new UITextBox(content, contentPosition, image));
            Player.notificationUp = true;
        }

        /// <summary>
        ///  Shows Notificationbox with Yes/No-Buttons
        /// </summary>
        /// <param name="content"></param>
        public static void ShowYesNoNotification(string content)
        {
            textboxes.Add(new UITextBox(content, true));
            Player.notificationUp = true;
        }

        public static void GetPlayerResponse(string content)
        {
            textInput = new TextBox(textInputPosition, 200, "", Orbs.graphics.GraphicsDevice, Orbs.spriteFontDefault, Color.LightGray, Color.LightBlue, 30, content);

            float margin = 5;
            textInput.Area = new Rectangle((int)(textInputPosition.X + margin), textInputPosition.Y, (int)(textInputPosition.Width - margin),
                textInputPosition.Height);
            textInput.Renderer.Color = Color.White;
            textInput.IsActive = true;
            textInputOpen = true;
        }

        /// <summary>
        /// Floating red text with fade out
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public static void ShowAlert(string id, string content)
        {
            uiTextElements.Add(new UITextAlert(id, content));
        }

        public static void ShowCombatText(string id, string content, Vector2 position)
        {
            combatTexts.Add(new UICombatText(id, content, position));
        }

        private static void UpdateEnemyNameplatePosition()
        {
            enemyNameplates = enemyNameplates.OrderByDescending(x => x.curHealth).ToList();

            if (enemyNameplates.Any(x => x.type == EnemyType.Boss))
            {
                int index = enemyNameplates.IndexOf(enemyNameplates.Where(i => i.type == EnemyType.Boss).First());
                enemyNameplates.Insert(0, enemyNameplates.ElementAt(index));
                enemyNameplates.RemoveAt(index + 1);
            }
            
            for (int i = 0; i < enemyNameplates.Count; i++ )
            {
                enemyNameplates.ElementAt(i).position.Y = 100 + i * 100;
            }
        }

        public static void Update(GameTime gameTime)
        {
            uiTextElements.RemoveAll(item => item.toBeRemoved);
            combatTexts.RemoveAll(item => item.toBeRemoved);

            if (currentDialogBox != null)
            {
                if(currentDialogBox.toBeRemoved)
                    currentDialogBox = null;
                else
                    currentDialogBox.Update(gameTime);
            }

            if(textInput != null)
                if (textInput.IsActive)
                    textInput.Update();

            if (actionBarClicked && gameTime.TotalGameTime.Seconds - clickedAt.Seconds > 0)
                actionBarClicked = false;

            foreach (UIElement uit in uiTextElements)
                uit.Update(gameTime);

            foreach (UICombatText ct in combatTexts)
                ct.Update(gameTime);

            foreach (UITextBox tb in textboxes)
                tb.Update(gameTime);

            UpdateEnemyNameplatePosition();

            if(tradeWindow != null)
                CheckMerchantProximity();
        }

        public static void DrawUI(SpriteBatch spriteBatch, Vector2 mouseWindowPos)
        {
            if (currentDialogBox != null)
                currentDialogBox.Draw(spriteBatch);

            spriteBatch.Draw(balanceBar_bg, balanceBar_bgRect, Color.White);
            if (Player.combatBalance > 0)
            {
                spriteBatch.Draw(actionbarTexture,
                    new Rectangle((int)balanceBarCenterPosition.X, (int)balanceBarCenterPosition.Y, Player.combatBalance, balanceBarYellowSourceRect.Height),
                    balanceBarYellowSourceRect,
                    Color.White);
            }
            else
            {
                spriteBatch.Draw(actionbarTexture,
                    new Rectangle((int)balanceBarCenterPosition.X, (int)balanceBarCenterPosition.Y, Player.combatBalance * -1, balanceBarPurpleSourceRect.Height),
                    balanceBarPurpleSourceRect,
                    Color.White);
            }
            spriteBatch.Draw(actionbarTexture, actionbarPosition, Color.White);

            foreach (UIElement b in actionbarButtons)
            {
                if(b != null)
                    b.Draw(spriteBatch);
            }

            if (charWindowOpen)
                charWindow.Draw(spriteBatch);

            if (Orbs.selectedInventorySlot >= 0)
                if(Player.inventoryGrid[Orbs.selectedInventorySlot] != null)
                    spriteBatch.Draw(Player.inventoryGrid[Orbs.selectedInventorySlot].IconTexture, mouseWindowPos, Color.White);

            if (actionBarClicked)
            {
                Vector2 apos = actionbarButtons.Where(slot => slot.Id == clickedActionBarSlot).First().UITexturePosition;
                spriteBatch.Draw(activationOverlay, apos, Color.White * 0.9f);
            }
            DrawActionbarShortcuts(spriteBatch);

            if (questWindowOpen)
                QuestSystem.QuestManager.DrawQuestWindow();

            foreach (UIElement t in textboxes)
                t.Draw(spriteBatch);

            if (textInput != null)
                if (textInput.IsActive)
                    textInput.Draw(spriteBatch);

            spriteBatch.Draw(statusbar_yellow, healthbarPosition, Color.White);
            spriteBatch.Draw(statusbar_yellow, healthbarPosition + manabarToHealthbarOffset, Color.White);

            spriteBatch.Draw(
                playerFrame, 
                new Rectangle((int)healthbarPosition.X, (int)healthbarPosition.Y, (int)(healthbar_SourceRectangle.Width * (Player.health / 100f)), healthbar_SourceRectangle.Height), 
                healthbar_SourceRectangle, Color.White);
            spriteBatch.Draw(
                playerFrame, 
                new Rectangle((int)healthbarPosition.X + (int)manabarToHealthbarOffset.X, (int)healthbarPosition.Y + (int)manabarToHealthbarOffset.Y, (int)(manabar_SourceRectangle.Width * (Player.mana / 100f)), manabar_SourceRectangle.Height), 
                manabar_SourceRectangle, Color.White);

            if (Player.selectedOrb != null)
                spriteBatch.Draw(Player.selectedOrb.IconTexture, playerFramePosition + new Vector2(6, 22), Color.White);

            spriteBatch.Draw(playerFrame, playerFramePosition, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, Player.health + " / " + Player.MAX_HEALTH, healthtextPostion, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, Player.mana + " / " + Player.MAX_MANA, healthtextPostion + manabarToHealthbarOffset, Color.White);

            if(!Player.glyphReady)
            {
                int secondsRemaining = 60 - (Orbs.currentTimeSpan - Player.glypActivatedOn).Seconds;

                spriteBatch.DrawString(Orbs.spriteFontBig, secondsRemaining.ToString(), actionBarPosition_2 + new Vector2(20, 15), Color.Red);
            }

            if (Player.currentBuff != null)
                Player.currentBuff.Draw(spriteBatch);

            foreach (QuestSystem.TimedEvent te in QuestSystem.QuestManager.timedEvents)
                te.Draw(spriteBatch);

            foreach(UIEnemyNameplate enp in enemyNameplates)
                enp.Draw(spriteBatch);

            if (spellSelectionOpen)
                DrawSpellSelectionBar(spriteBatch);

            if (weaponSelectionOpen)
                DrawWeaponSelectionBar(spriteBatch);

            if (tradeWindow != null)
                tradeWindow.Draw(spriteBatch);
        }

        public static void DrawSettingsWindow(SpriteBatch spriteBatch, bool playerExists)
        {
            string curLanguage = Thread.CurrentThread.CurrentUICulture == CultureInfo.GetCultureInfo("en-US") ? "English" : "German";

            if (playerExists)
            {
                DrawPauseMenu(spriteBatch);

                foreach (UIButton b in settingsMenuButtons)
                    b.Draw(spriteBatch);

                //Language
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu, curLanguage,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 250),
                    Color.Brown);
                //Music
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu,
                    Audio.MusicManager.GetMusicVolume() == 0.0f ? Localization.strings.system_off : Localization.strings.system_on,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 305),
                    Color.Brown);
                //Sounds
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu,
                    Audio.SoundManager.soundMuted ? Localization.strings.system_off : Localization.strings.system_on,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 365),
                    Color.Brown);
            }
            else
            {
                DrawStartMenu(spriteBatch);

                foreach (UIButton b in settingsMenuButtons)
                    b.Draw(spriteBatch);

                //Language
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu, curLanguage,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 250),
                    Color.Brown);
                //Music
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu,
                    Audio.MusicManager.GetMusicVolume() == 0.0f ? Localization.strings.system_off : Localization.strings.system_on,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 305),
                    Color.Brown);
                //Sounds
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu,
                    Audio.SoundManager.soundMuted ? Localization.strings.system_off : Localization.strings.system_on,
                    new Vector2(gameMenuWindowPosition.X + 350, gameMenuWindowPosition.Y + 365),
                    Color.Brown);
            }
        }

        public static void ToggleWeaponSelection()
        {
            weaponSelectionSlots.Clear();
            weaponSelectionOpen = !weaponSelectionOpen;

            if (weaponSelectionOpen)
                spellSelectionOpen = false;
        }

        public static void ToggleSpellSelection()
        {
            spellSelectionSlots.Clear();
            spellSelectionOpen = !spellSelectionOpen;

            if (spellSelectionOpen)
                weaponSelectionOpen = false;
        }

        public static string CheckWeaponSelectionClick(Vector2 mouseWindowPos)
        {
            if (weaponSelectionSlots.Count < 1)
                return null;

            foreach(KeyValuePair<Item, Rectangle> slot in weaponSelectionSlots)
            {
                if (slot.Value.Contains(mouseWindowPos))
                {
                    ToggleWeaponSelection();
                    return slot.Key.Id;
                }
            }
            return null;
        }

        public static string CheckSpellSelectionClick(Vector2 mouseWindowPos)
        {
            if (spellSelectionSlots.Count < 1)
                return null;

            foreach (KeyValuePair<Spell, Rectangle> slot in spellSelectionSlots)
            {
                if (slot.Value.Contains(mouseWindowPos))
                {
                    ToggleSpellSelection();
                    return slot.Key.Id;
                }
            }
            return null;
        }

        public static void DrawUIElements(SpriteBatch spriteBatch)
        {
            foreach (UIElement uit in uiTextElements)
                uit.Draw(spriteBatch);
        }

        public static void DrawStartMenu(SpriteBatch spriteBatch)
        {
            Vector2 parallaxOffset =
                new Vector2(Orbs.graphics.PreferredBackBufferWidth / 2, Orbs.graphics.PreferredBackBufferHeight / 2)
                - Mouse.GetState().Position.ToVector2();

            spriteBatch.Draw(UIManager.menuBackground, new Vector2(0 + parallaxOffset.X / 20, 0), Color.White);
            spriteBatch.Draw(UIManager.pilarLeft, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(UIManager.pilarRight, new Vector2(Orbs.graphics.PreferredBackBufferWidth - pilarRight.Width, 0), Color.White);
            spriteBatch.Draw(UIManager.gameMenuWindow, UIManager.gameMenuWindowPosition, Color.White);
            spriteBatch.Draw(UIManager.orbsLogo, orbsLogoPosition, Color.White);

            foreach (UIButton b in startMenuButtons)
            {
                if (Orbs.curGameState == GameState.NewGame)
                {
                    b.Draw(spriteBatch);
                }
                else
                {
                    if (b.Id != "newGameMale" && b.Id != "newGameFemale" && !b.Id.StartsWith("select"))
                        b.Draw(spriteBatch);
                }
            }

            if (Orbs.curGameState == GameState.NewGame)
                spriteBatch.DrawString(Orbs.spriteFontMenu, Localization.strings.menu_SelectHero, gameMenuWindowPosition + NewHeroTextOffset, Color.Black);

            if (selectedHero == "male" && Orbs.curGameState == GameState.NewGame)
            {
                spriteBatch.Draw(LootTable.glyphs[0].GlyphTexture, heroSelectPosition + new Vector2(0,100), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1.0f);
                startNewGameMaleButton.Draw(spriteBatch);
            }
            if (selectedHero == "female" && Orbs.curGameState == GameState.NewGame)
            {
                spriteBatch.Draw(LootTable.glyphs[0].GlyphTexture, heroSelectPosition + new Vector2(200, 100), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1.0f);
                startNewGameFemaleButton.Draw(spriteBatch);
            }
        }

        public static void DrawPauseMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(UIManager.gameMenuWindow, UIManager.gameMenuWindowPosition, Color.White);
            spriteBatch.Draw(UIManager.orbsLogo, orbsLogoPosition, Color.White);
            spriteBatch.Draw(LootTable.glyphs[0].GlyphTexture, heroSelectPosition + new Vector2(80, -50), Color.White);

            foreach (UIButton b in pauseMenuButtons)
                b.Draw(spriteBatch);
        }

        internal static void DrawActionbarShortcuts(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Orbs.spriteFontDefault, GetKeyLabel(GameplayButtons.CHARACTER), buttonPosition_Character, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, GetKeyLabel(GameplayButtons.QUESTLOG), buttonPosition_Quests, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, "LM", actionBarPosition_0, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, "RM", actionBarPosition_1, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, GetKeyLabel(GameplayButtons.ACTIONBAR_2), actionBarPosition_2, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, GetKeyLabel(GameplayButtons.ACTIONBAR_3), actionBarPosition_3, Color.White);
        }

        internal static void DrawCombatText(SpriteBatch spriteBatch)
        {
            foreach (UICombatText ct in combatTexts)
                ct.Draw(spriteBatch);
        }

        internal static void UpdateCombatText(GameTime gameTime)
        {
            foreach (UICombatText ct in combatTexts)
                ct.Update(gameTime);
        }

        public static void DrawWeaponSelectionBar(SpriteBatch spriteBatch)
        {
            Vector2 mouseWindowPos = Mouse.GetState().Position.ToVector2();
            string nameToDisplay = "";

            int weaponCount = Player.inventory.Where(w => w.Type == Item.type.Weapon).Count();
            int barWidth = (64 + 5) * weaponCount + 5;
            Rectangle backgroundRect = new Rectangle((int)actionbarPosition.X + 75, (int)actionbarPosition.Y - 10, barWidth, 74);

            spriteBatch.Draw(selectionBarBackground, backgroundRect, Color.White);

            int curWeaponCount = 0;

            //Getting Weaponslots if not already done
            if(weaponSelectionSlots.Count == 0)
                for(int i = 0; i <= Player.inventory.Count() - 1; i++)
                {
                    if(Player.inventory[i].Type == Item.type.Weapon)
                    {
                        weaponSelectionSlots.Add(Player.inventory[i], new Rectangle(backgroundRect.X + 5 + (5 * curWeaponCount) + (64 * curWeaponCount), backgroundRect.Y + 5, 64, 64));
                        curWeaponCount++;
                    }
                }

            for(int j = 0; j <= weaponSelectionSlots.Count() - 1; j++)
            {
                spriteBatch.Draw(weaponSelectionSlots.ElementAt(j).Key.IconTexture, weaponSelectionSlots.ElementAt(j).Value, Color.White);
                if (weaponSelectionSlots.ElementAt(j).Value.Contains(mouseWindowPos))
                    nameToDisplay = weaponSelectionSlots.ElementAt(j).Key.Name;
            }

            if (nameToDisplay != "")
                DrawSelectionName(spriteBatch, nameToDisplay, mouseWindowPos);
        }

        public static void DrawSpellSelectionBar(SpriteBatch spriteBatch)
        {
            Vector2 mouseWindowPos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();
            string nameToDisplay = "";

            int spellCount = Player.learnedSpells.Count();
            int barWidth = (64 + 5) * spellCount + 5;
            Rectangle backgroundRect = new Rectangle((int)actionbarPosition.X + 160, (int)actionbarPosition.Y - 10, barWidth, 74);

            spriteBatch.Draw(selectionBarBackground, backgroundRect, Color.White);

            //Getting spellslots if not already done
            if(spellSelectionSlots.Count == 0)
                for (int i = 0; i <= Player.learnedSpells.Count() - 1; i++)
                {
                    spellSelectionSlots.Add(Player.learnedSpells[i], new Rectangle(backgroundRect.X + 5 + (5 * i) + (64 * i), backgroundRect.Y + 5, 64, 64));
                }

            for (int j = 0; j <= spellSelectionSlots.Count() - 1; j++)
            {
                spriteBatch.Draw(spellSelectionSlots.ElementAt(j).Key.IconTexture, spellSelectionSlots.ElementAt(j).Value, Color.White);
                if (spellSelectionSlots.ElementAt(j).Value.Contains(mouseWindowPos))
                    nameToDisplay = spellSelectionSlots.ElementAt(j).Key.Name;
            }

            if(nameToDisplay != "")
                DrawSelectionName(spriteBatch, nameToDisplay, mouseWindowPos);
        }

        private static void DrawSelectionName(SpriteBatch spriteBatch, string name, Vector2 mouseWindowPos)
        {
            Vector2 displayTextSize = Orbs.spriteFontDefault.MeasureString(name);
            int x = (int)mouseWindowPos.X + 10;
            int y = (int)mouseWindowPos.Y - 20;

            spriteBatch.Draw(UIManager.charLabelBackGround, new Rectangle(x, y, (int)displayTextSize.X, (int)displayTextSize.Y), Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, name, new Vector2(x, y), Color.Black);

        }

        public static void InitiateTrade(string merchant, string items, Vector2 initiatedFrom)
        {
            tradeWindow = new UITradeWindow(merchant, items, initiatedFrom);
        }

        private static void CheckMerchantProximity()
        {
            if (Vector2.Distance(tradeWindow.initiatedFromWorldPosition, Player.PlayerPosition) > 200)
                tradeWindow = null;
        }
    }
}
