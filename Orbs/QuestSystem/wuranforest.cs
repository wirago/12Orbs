using Orbs.Interface;
using System.Linq;
using Microsoft.Xna.Framework;
using Orbs.Animations;
using Orbs.LootSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Orbs.World;
using System.Collections.Generic;

namespace Orbs.QuestSystem
{
    public class wuranforest : ILevel
    {
        private bool canEnterTown = false;
        private bool stoneTableLooted = false;
        private bool notePickedUp = false;
        private bool bushNotePickedUp = false;
        private bool jarFilled = false;
        private bool signPostRead = false;

        public wuranforest()
        {
            ApplyChapterProgression();
        }

        public void InteractWithNPC(string name)
        {
            switch(name)
            {
                case "Guard Nisol":
                    InteractWithGuard();
                    break;

                case "Angus":
                    InteractWithAngus();
                    break;
            }
        }

        public void InteractWithObject(string iObject)
        {
            if(iObject == "SignPost01")
            {
                if(!notePickedUp)
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_SignPost01_01);
                    return;
                }
                else
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_SignPost01_02);
                    signPostRead = true;
                    return;
                }
            }

            if (iObject == "SignPostCave")
            {
                UIManager.ShowNotification(Localization.strings_wuranforest.level01_SignPostCave);
                return;
            }

            if (iObject == "SignPostField")
            {
                UIManager.ShowNotification(Localization.strings_wuranforest.level01_SignPostField);
                return;
            }

            if (iObject == "SignPostLake")
            {
                UIManager.ShowNotification(Localization.strings_wuranforest.level01_SignPostLake);
                return;
            }

            if(iObject == "BlueJars")
            {
                if(UIManager.charWindow.HasItem("lvl1_jar"))
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_01);
                    return;
                }
                
                if(Player.GetChapterProgression("wuranforest").Any(q => q.id == "blueJars"))
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_06);
                    return;
                }

                UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_02);

                Item itemToAdd = new Item();
                itemToAdd.IconTexture = Orbs.content.Load<Texture2D>("Textures/Icons/QuestItems/lvl1_jar");
                itemToAdd.Id = "lvl1_jar";
                itemToAdd.Name = "Blue Jar";
                itemToAdd.Type = Item.type.QuestItem;

                UIManager.charWindow.AddItemToInventory(itemToAdd);
                return;
            }

            if(iObject == "Well")
            {
                if(jarFilled)
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_03);
                    return;
                }

                if(UIManager.charWindow.HasItem("lvl1_jar"))
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_04);
                    jarFilled = true;
                    return;
                }
                else
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Jar_05);
                    return;
                }
            }

            if(iObject == "RedBush")
            {
                if(!signPostRead || bushNotePickedUp || Player.IsOnQuest("wuranforest", "redbush"))
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_RedBush_01);
                    return;
                }

                if (!jarFilled)
                {
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_RedBush_02);
                    return;
                }
                HandleItemPickup("bushNote");
                return;
            }
        }

        public void HandleItemPickup(string itemName)
        {
            switch(itemName)
            {
                case "Stone Tablet":
                    stoneTableLooted = true;
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_StoneTablet);
                    QuestManager.AddQuestToQuestLog(Localization.strings_names.location_wuran, Localization.strings_questlog.level_01_story_1);
                    SpawnObject("lvl1_note1", "lvl1_note1", 64, 64, "Note", new Vector2(900, 130), "QItem");
                    Player.SetChapterProgression("wuranforest", "stoneTablet");
                    break;

                case "Note":
                    notePickedUp = true;
                    Player.CompleteQuest("wuranforest", "stoneTablet");
                    Player.SetChapterProgression("wuranforest", "waterfallNote");
                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_Note_01);
                    break;

                case "bushNote":
                    bushNotePickedUp = true;

                    UIManager.ShowNotification(Localization.strings_wuranforest.level01_BushNote_01);
                    Item itemToAdd = LootTable.questItems.Where(item => item.Id == "lvl1_bushNote").First();

                    UIManager.charWindow.AddItemToInventory(itemToAdd);
                    Player.SetChapterProgression("wuranforest", "redbush");
                    break;

                case "orb_justice":
                    UIManager.ShowNotification(Localization.strings_wuranforest.orbPickUp);
                    QuestManager.AddQuestToQuestLog(Localization.strings_names.location_wuran, Localization.strings_questlog.level_01_story_2);
                    Player.SetChapterProgression("wuranforest", "orbPickedUp");
                    break;
            }
        }

        /// <summary>
        /// Returns and array of itemIDs on enemy kill
        /// </summary>
        /// <param name="enemyId"></param>
        /// <returns>string[itemID]</returns>
        public string[] HandleEnemyKill(string enemyId)
        {
            if (enemyId == "lvl1_angus")
            {
                Player.SetChapterProgression("wuranforest", "angusDead");
                return new string[] { "orb_1_justice", "corruptionGift" };
            }

            if (enemyId.StartsWith("spider"))
            {
                //if(!Player.HasItem("spiderEyes"))
                //    UIManager.ShowNotification(Localization.strings_wuranforest.eyePickup);

                return new string[] { "spiderEyes" };
            }


            return new string[] { string.Empty };
        }

        private void InteractWithAngus()
        {
            if(UIManager.charWindow.HasItem("lvl1_bushNote"))
            {
                UIManager.ShowNotification("Angus\n" + Localization.strings_wuranforest.level01_AngusTriggered);
                WorldCharacters.standardWorldCharacters.First(npc => npc.ID == "lvl1_angus").MakeNPCHostile();
            }
            else
            {
                UIManager.ShowNotification("Angus:\n" + Localization.strings_wuranforest.level01_AngusNeutral);
            }
        }

        private void InteractWithGuard()
        {
            if(canEnterTown)
            {
                UIManager.ShowNotification(Localization.strings_wuranforest.level01_Guard_01);
                
                return;
            }

            if (UIManager.charWindow.HasItem("corruptionGift"))
            {
                UIManager.ShowNotification(Localization.strings_wuranforest.level01_Guard_02);
                OpenTown();
                return;
            }

            QuestManager.AddQuestToQuestLog(Localization.strings_names.location_wuran, Localization.strings_questlog.level_01_guard_noEntry);
            UIManager.ShowNotification(Localization.strings_wuranforest.level01_Guard_03);
        }

        private void OpenTown()
        {
            if (UIManager.charWindow.HasItem("corruptionGift"))
                UIManager.charWindow.RemoveItemFromInventory(Player.inventoryGrid.Where(item => item.Value != null ? item.Value.Id == "corruptionGift" : false).First().Key);

            canEnterTown = true;
            WorldCharacters.standardWorldCharacters.Where(npc => npc.Name == "Guard Nisol").First().MoveNPCTo(new Vector2(6060, 2020));
            Player.SetChapterProgression("wuranforest", "townOpen");
        }

        public void SpawnObject(string itemId, string textureName, int width, int height, string objectName, Vector2 spawnPosition, string objectType)
        {
            ObjectAnimation itemToAdd = new ObjectAnimation(itemId, textureName, (int)spawnPosition.X, (int)spawnPosition.Y, width, height, objectName, objectType, textureName);
            WorldObjects.animatedObjects.Add(itemToAdd);
        }

        public void Update(GameTime gameTime)
        {
            if (!Player.GetChapterProgression("wuranforest").Any(q => q.id == "entry"))
            {
                UIManager.ShowStoryTextbox(Localization.strings_wuranforest.level01_Story_01, new Vector2(600, 100), "lvl1_01");
                QuestManager.AddQuestToQuestLog(Localization.strings_names.location_wuran, Localization.strings_questlog.level_01_story_0);
                Player.SetChapterProgression("wuranforest", "entry");
            }
        }

        public void HandleTextInput(string text)
        {

        }

        private void ApplyChapterProgression()
        {
            List<Quest> progress = Player.GetChapterProgression("wuranforest");

            if(progress.Any(q => q.id == "townOpen"))
                OpenTown();

            if(progress.Any(q => q.id == "stoneTablet"))
            {
                if (!progress.Where(o => o.id == "stoneTablet").First().isCompleted)
                {
                    stoneTableLooted = true;
                    SpawnObject("lvl1_note1", "lvl1_note1", 64, 64, "Note", new Vector2(900, 130), "QItem");
                }
            }

            if (progress.Any(q => q.id == "waterfallNote"))
                bushNotePickedUp = true;

            if (progress.Any(q => q.id == "angusDead"))
                WorldCharacters.standardWorldCharacters.Remove(WorldCharacters.standardWorldCharacters.Where(n => n.Name == "Angus").First());
        }

        public void HandleNPCProximity(string npcId)
        {
            
        }
    }
}
