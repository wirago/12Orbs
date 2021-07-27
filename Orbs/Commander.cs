using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Orbs.Interface
{
    public static class Commander
    {
        public static List<string> commandHistory = new List<string>();

        public static void executeCommand(string input)
        {
            if (input.StartsWith("player."))
            {
                input = input.Substring(7);
                //Adds an Item to player's Inventory
                if (input.StartsWith("addItem "))
                {
                    string itemId = input.Substring(input.IndexOf(' ') + 1);
                    UIManager.charWindow.AddItemToInventory(itemId);
                    return;
                }
                //Sets the walking speed
                if (input.StartsWith("setSpeed "))
                {
                    int speed = int.Parse(input.Substring(input.IndexOf(' ') + 1));
                    Player.movementspeed = speed;
                }
                //Heals the player to full life
                if (input.StartsWith("heal"))
                {
                    Player.health = 100;
                }

                if (input.StartsWith("changeWorld"))
                {
                    //TODO to be implemented
                }

                if (input.StartsWith("setHealth "))
                {
                    int health = int.Parse(input.Substring(input.IndexOf(' ') + 1));
                    Player.health = health;
                }
            }

            if (input.StartsWith("game."))
            {
                input = input.Substring(5);
                if (input.StartsWith("stopMusic"))
                {
                    Audio.MusicManager.ShutDownMusic();
                    return;
                }

                if(input.StartsWith("flash"))
                {
                    MouseState mouseState = Mouse.GetState();
                    Vector2 mousePos = mouseState.Position.ToVector2();

                    Effects.EffectManager.flashBolt = new Effects.FlashBolt(Player.PlayerPosition, mousePos);
                }

                if(input.StartsWith("shake"))
                {
                    int duration;
                    bool result = int.TryParse(input.Substring(input.IndexOf(' ') + 1), out duration);

                    if (!result)
                        return;

                    Effects.EffectManager.CreateShakeEffect("consoleShake", duration);
                    return;
                }

                if (input.StartsWith("ambient_day"))
                {
                    World.WorldObjects.ChangeAmbienteColor(Effects.Lightning.AMBIENTCOLOR_DAY);
                    
                    if(Orbs.curLevel == "hollowrock")
                        Orbs.musicManager.ChangeBackgroundMusic("HollowrockAmbientDay");

                    return;
                }

                if (input.StartsWith("ambient_night"))
                {
                    World.WorldObjects.ChangeAmbienteColor(Effects.Lightning.AMBIENTCOLOR_NIGHT);

                    if (Orbs.curLevel == "hollowrock")
                        Orbs.musicManager.ChangeBackgroundMusic("HollowrockAmbientNight");

                    return;
                }

                if(input.StartsWith("ambient_red"))
                {
                    World.WorldObjects.ChangeAmbienteColor(Color.Crimson);
                    return;
                }

                if (input.StartsWith("ambient_green"))
                {
                    World.WorldObjects.ChangeAmbienteColor(Color.Green);
                    return;
                }

                if (input.StartsWith("ambient_violet"))
                {
                    World.WorldObjects.ChangeAmbienteColor(Color.BlueViolet);
                    return;
                }
            }
            else
                QuestSystem.QuestManager.HandleTextInput(input);
        }
    }
}
