using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Orbs.Interface;
using Orbs.World;

namespace Orbs
{
    public struct MovementButtons
    {
        const Keys MOVEUP = Keys.W;
        const Keys MOVEDOWN = Keys.S;
        const Keys MOVELEFT = Keys.A;
        const Keys MOVERIGHT = Keys.D;

        public Vector2 PlayerMove()
        {
            Vector2 r = Vector2.Zero;
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(MOVEUP))
                r.Y -= 1;
            if (ks.IsKeyDown(MOVEDOWN))
                r.Y += 1;
            if (ks.IsKeyDown(MOVELEFT))
                r.X -= 1;
            if (ks.IsKeyDown(MOVERIGHT))
                r.X += 1;

            return r;
        }
    }

    public struct GameplayButtons
    {
        public const Keys ESCAPE = Keys.Escape;

        public const Keys ATTACK = Keys.F;
        public const Keys SELFDMG = Keys.H;
        public const Keys CONSOLE = Keys.End;

        public const Keys CHARACTER = Keys.C;
        public const Keys INVENTORY = Keys.I;
        public const Keys MAP = Keys.M;
        public const Keys QUESTLOG = Keys.L;

        public const Keys ACTIONBAR_0 = Keys.D1;
        public const Keys ACTIONBAR_1 = Keys.D2;
        public const Keys ACTIONBAR_2 = Keys.Q;
        public const Keys ACTIONBAR_3 = Keys.E;

        private ButtonState GetState(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
                return ButtonState.Pressed;

            return ButtonState.Released;
        }

        public ButtonState Escape { get { return GetState(ESCAPE); } }
        public ButtonState Attack { get { return GetState(ATTACK); } }
        public ButtonState SelfDMG { get { return GetState(SELFDMG); } }
        public ButtonState Console { get { return GetState(CONSOLE); } }

        public ButtonState Character { get { return GetState(CHARACTER); } }
        public ButtonState Inventory { get { return GetState(INVENTORY); } }
        public ButtonState Map { get { return GetState(MAP); } }
        public ButtonState Questlog { get { return GetState(QUESTLOG); } }

        public ButtonState Actionbar_0 { get { return GetState(ACTIONBAR_0); } }
        public ButtonState Actionbar_1 { get { return GetState(ACTIONBAR_1); } }
        public ButtonState Actionbar_2 { get { return GetState(ACTIONBAR_2); } }
        public ButtonState Actionbar_3 { get { return GetState(ACTIONBAR_3); } }
    }

    public class InputHandler
    {
        public MovementButtons movementButtons = new MovementButtons();
        public GameplayButtons gameplayButtons = new GameplayButtons();

        public void HandleMouseOver(Vector2 mouseWorldPos, Vector2 mouseWindowPosition)
        {
            foreach (var npc in WorldCharacters.standardWorldCharacters)
                npc.isHovered = npc.BoundingBox.Contains(mouseWorldPos);

            foreach (var enpc in WorldCharacters.enemyWorldCharacters)
                enpc.isHovered = enpc.BoundingBox.Contains(mouseWorldPos);

            foreach (UIEnemyNameplate enp in UIManager.enemyNameplates)
                enp.CheckDebuffTooltip(mouseWindowPosition);

            if(Orbs.curGameState == GameState.Menu || Orbs.curGameState == GameState.NewGame)
            {
                foreach (UIButton uib in UIManager.startMenuButtons)
                    uib.isHovered = uib.IsHovered(mouseWindowPosition);

                UIManager.startNewGameFemaleButton.isHovered = UIManager.startNewGameFemaleButton.IsHovered(mouseWindowPosition);
                UIManager.startNewGameMaleButton.isHovered = UIManager.startNewGameMaleButton.IsHovered(mouseWindowPosition);
            }

            if (Orbs.curGameState == GameState.Paused)
            {
                foreach (UIButton uib in UIManager.pauseMenuButtons)
                    uib.isHovered = uib.IsHovered(mouseWindowPosition);
            }

            if (Orbs.curGameState == GameState.Settings)
            {
                foreach (UIButton uib in UIManager.settingsMenuButtons)
                    uib.isHovered = uib.IsHovered(mouseWindowPosition);
            }

            if (UIManager.currentDialogBox != null)
                UIManager.currentDialogBox.okButton.isHovered = UIManager.currentDialogBox.okButton.IsHovered(mouseWindowPosition);
        }

        public string HandleLeftMouseClick(Vector2 mouseWorldPos, Vector2 mouseWindowPosition)
        {
            if (Player.currentGlyph != null)
            {
                Player.PlaceGlyph();
                return null;
            }

            if(Orbs.curGameState == GameState.Menu || Orbs.curGameState == GameState.Paused || Orbs.curGameState == GameState.Settings || Orbs.curGameState == GameState.NewGame)
                return UIManager.CheckMouseClick(mouseWorldPos, mouseWindowPosition);

            string UIAction = UIManager.CheckMouseClick(mouseWorldPos, mouseWindowPosition);

            if (!string.IsNullOrEmpty(UIAction) && UIAction.StartsWith("button_"))
            {
                UIManager.ToggleWindowById(UIAction);
                return null;
            }

            if(UIAction == "weaponSelection")
            {
                UIManager.ToggleWeaponSelection();
                return null;
            }

            if (UIAction == "spellSelection")
            {
                UIManager.ToggleSpellSelection();
                return null;
            }

            if (UIAction == "dialogbox")
                return null;

            if (!string.IsNullOrEmpty(UIAction) && UIAction.StartsWith("actionbar_"))
                return UIManager.actionToBeTriggered(UIAction);
           
            if (WorldObjects.CheckMousClick(mouseWorldPos))
                return null;

            if (WorldCharacters.CheckLeftMouseClick(mouseWorldPos, mouseWindowPosition) == "npc")
                return null;

            if (UIManager.charWindowOpen)
            {
                if (UIManager.charWindow.CheckMouseClick(mouseWindowPosition) != "")
                    return null;
            }

            if (Player.notificationUp)
            {
                if (UIManager.CheckNotificationClick() == 1)
                    return null;
            }

            if (UIManager.tradeWindow != null)
                return null;

            return UIManager.actionToBeTriggered("actionbar_0");
        }

        public void HandleEscapePress()
        {
            if(UIManager.tradeWindow != null)
            {
                UIManager.tradeWindow = null;
                return;
            }

            if(UIManager.questWindowOpen)
            {
                UIManager.ToggleWindowById("button_Questlog");
                return;
            }

            if (UIManager.charWindowOpen)
            {
                UIManager.ToggleWindowById("button_Character");
                return;
            }


            if (Orbs.curGameState == GameState.Playing)
            {
                Orbs.curGameState = GameState.Paused;
                return;
            }
        }

        public string HandleRightMouseClick(Vector2 mouseWorldPos, Vector2 mouseWindowPosition)
        {
            if(UIManager.tradeWindow != null)
            {
                UIManager.tradeWindow.HandleRightClick(mouseWindowPosition);
                return null;
            }

            //if (WorldCharacters.CheckRightMouseClick(mouseWorldPos) == "enemy" && !Player.isCasting)
            return UIManager.actionToBeTriggered("actionbar_1");

            //return null;
        }

        public void HandleDragAndDrop(Vector2 mouseWindowPosition, bool buttonPressed)
        {
            if (UICharWindow.destroyWarning)
                UIManager.CheckNotificationClick();
    
            if (Player.notificationUp)
                UIManager.CheckNotificationClick();

            if (UIManager.charWindowOpen)
                UIManager.charWindow.DragAndDropItem(mouseWindowPosition, buttonPressed);

            if (UIManager.questWindowOpen)
                QuestSystem.QuestManager.HandleQuestSelection(mouseWindowPosition);
        }

        public void HandleItemUse(Vector2 mouseWindowPosition)
        {
            if (UIManager.charWindowOpen)
                UIManager.charWindow.UseItem(mouseWindowPosition);
        }

        public string HandleKeyStroke(KeyboardState ks, KeyboardState oldks)
        {
            if (UIManager.textInputOpen)
                return "";

            //if (ks.IsKeyDown(GameplayButtons.ACTIONBAR_0) && oldks.IsKeyUp(GameplayButtons.ACTIONBAR_0))
            //    return UIManager.actionToBeTriggered("actionbar_0");
            //if (ks.IsKeyDown(GameplayButtons.ACTIONBAR_1) && oldks.IsKeyUp(GameplayButtons.ACTIONBAR_1))
            //    return UIManager.actionToBeTriggered("actionbar_1");
            if (ks.IsKeyDown(GameplayButtons.ACTIONBAR_2) && oldks.IsKeyUp(GameplayButtons.ACTIONBAR_2))
                return UIManager.actionToBeTriggered("actionbar_2");
            if (ks.IsKeyDown(GameplayButtons.ACTIONBAR_3) && oldks.IsKeyUp(GameplayButtons.ACTIONBAR_3))
                return UIManager.actionToBeTriggered("actionbar_3");

            if (ks.IsKeyDown(GameplayButtons.CHARACTER) && oldks.IsKeyUp(GameplayButtons.CHARACTER))
                UIManager.ToggleWindowById("button_Character");

            if (ks.IsKeyDown(GameplayButtons.INVENTORY) && oldks.IsKeyUp(GameplayButtons.INVENTORY))
                UIManager.ToggleWindowById("button_Inventory");

            if (ks.IsKeyDown(GameplayButtons.QUESTLOG) && oldks.IsKeyUp(GameplayButtons.QUESTLOG))
                UIManager.ToggleWindowById("button_Questlog");

            if (ks.IsKeyDown(GameplayButtons.MAP) && oldks.IsKeyUp(GameplayButtons.MAP))
                UIManager.ToggleWindowById("button_Map");

            return "";
        }

        public static void HandleTextInput(string text, bool isConsoleCommand)
        {
            if(text != "" && isConsoleCommand)
            {
                Commander.commandHistory.Insert(0, text);
                Commander.executeCommand(text);
                return;
            }
            
            if(text != "" && !isConsoleCommand)
            {
                QuestSystem.QuestManager.HandleTextInput(text);
                return;
            }
                
        }


    }
}
