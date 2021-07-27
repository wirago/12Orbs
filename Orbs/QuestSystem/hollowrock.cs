using Microsoft.Xna.Framework;
using Orbs.Effects;
using Orbs.Interface;
using Orbs.LootSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orbs.QuestSystem
{
    class hollowrock : ILevel
    {
        public string[] HandleEnemyKill(string enemyType)
        {
            return new string[] {" "};
        }

        public void HandleItemPickup(string itemName)
        {

        }

        public void HandleTextInput(string text)
        {
            switch(text.ToLower())
            {
                case "shadow": //TODO input need to be bound to quest
                case "schatten":
                    UIManager.ShowNotification(Localization.strings_hollowrock.Jongar_3);
                    Player.CompleteQuest("hollowrock", "jongar_01");
                    Player.SetChapterProgression("hollowrock", "merchant_03");
                    QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_jongar_01);
                    break;
            }
        }

        public void InteractWithNPC(string name)
        {
            switch(name)
            {
                case "Trisha Bell":
                    List<string> dialogContent = new List<string>();
                    dialogContent.Add("You:\nHey, how are you?");
                    dialogContent.Add("Trisha Bell:\nHi. Nice day for fishin' ain't it?!");
                    dialogContent.Add("You:\nHave you seen some weird bearded guy?");
                    dialogContent.Add("Trisha Bell:\nNo, just a bald one.");
                    dialogContent.Add("You:\nOk, thanks.");

                    UIManager.ShowDialogWindow("dlg_trisha2", name, "dlg_default", dialogContent);
                    break;

                case "Gindella":
                    if (!Player.IsOnQuest("hollowrock", "gindella_1"))
                    {
                        UIManager.ShowNotification(Localization.strings_hollowrock.Gindella_1);
                        QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_story_02);
                        Player.SetChapterProgression("hollowrock", "gindella_1");
                    }
                    else
                    {
                        UIManager.ShowNotification(GetRandomTextFromGindella());
                    }
                        
                    break;

                case "Serina":

                    if(Player.HasQuestCompleted("hollowrock", "merchant_02")) //Player has finished both pre-quests
                    {
                        UIManager.ShowNotification(Localization.strings_hollowrock.Serina_5);
                        Player.SetChapterProgression("hollowrock", "jongar_01");
                        return;
                    }

                    if (!Player.IsOnQuest("hollowrock", "spiderEyes"))
                    {
                        UIManager.ShowNotification(Localization.strings_hollowrock.Serina_1);
                        QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_spiders_01);
                        Player.SetChapterProgression("hollowrock", "spiderEyes");
                        return;
                    }

                    if (Player.IsOnQuest("hollowrock", "spiderEyes"))
                    {
                        if (!Player.HasQuestCompleted("hollowrock", "spiderEyes"))
                        {

                            if (Player.CountItems("spiderEyes") >= 2)
                            {
                                Player.CompleteQuest("hollowrock", "spiderEyes");
                                Player.RemoveItemFromInventory("spiderEyes");
                                UIManager.ShowNotification(Localization.strings_hollowrock.Serina_2);
                                QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_merchant_01);
                                Player.SetChapterProgression("hollowrock", "merchant_01");
                                return;
                            }
                            else
                            {
                                UIManager.ShowNotification(Localization.strings_hollowrock.Serina_3);
                                return;
                            }
                        }
                    }

                    if (Player.IsOnQuest("hollowrock", "merchant_01"))
                    {
                        if (!Player.HasQuestCompleted("hollowrock", "merchant_01"))
                        {
                            UIManager.ShowNotification(Localization.strings_hollowrock.Serina_4);
                        }
                    }


                    break;

                case "Geddh":
                    if (Player.IsOnQuest("hollowrock", "merchant_03"))
                    {
                        UIManager.ShowNotification(Localization.strings_hollowrock.Geddh_5);
                        QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_geddh_01);
                        return;
                    }

                    if (Player.IsOnQuest("hollowrock", "merchant_01")) 
                    {
                        if (!Player.HasQuestCompleted("hollowrock", "merchant_01"))
                        {
                            UIManager.ShowNotification(Localization.strings_hollowrock.Geddh_2); //deliver pants
                            Player.AddItemToInventory("hollowrock_pants");
                            Player.CompleteQuest("hollowrock", "merchant_01");
                            Player.SetChapterProgression("hollowrock", "merchant_02");
                            return;
                        }

                        if (!Player.HasQuestCompleted("hollowrock", "merchant_02")) //Player didnt finish quest yet
                        {
                            UIManager.ShowNotification(Localization.strings_hollowrock.Geddh_3);
                            return;
                        }
                            

                        if (Player.HasQuestCompleted("hollowrock", "merchant_02")) //Player finished his quest
                        {
                            UIManager.ShowNotification(Localization.strings_hollowrock.Geddh_4);
                            return;
                        }                           
                    }

                    UIManager.ShowNotification(Localization.strings_hollowrock.Geddh_1);

                    break;


                case "Borderline Betty":
                    if (Player.IsOnQuest("hollowrock", "merchant_02"))
                    {
                        if (Player.HasItem("hollowrock_pants"))
                        {
                            UIManager.ShowNotification(Localization.strings_hollowrock.Betty_1);
                            Player.RemoveItemFromInventory("hollowrock_pants");
                            Player.CompleteQuest("hollowrock", "merchant_02");
                            QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_betty_01);
                            return;
                        }
                    }

                    UIManager.ShowNotification(Localization.strings_hollowrock.Betty_2);
                    break;


                case "Jongar":
                    if(Player.HasQuestCompleted("hollowrock", "jongar_01"))
                    {
                        UIManager.ShowNotification(Localization.strings_hollowrock.Jongar_4);
                        return;
                    }

                    if (Player.IsOnQuest("hollowrock", "jongar_01")) //Player has finished both pre-quests
                    {
                        UIManager.GetPlayerResponse(Localization.strings_hollowrock.Jongar_2);
                        return;
                    }

                    UIManager.ShowNotification(Localization.strings_hollowrock.Jongar_1);
                    break;

                case "Guard Worn":
                    UIManager.GetPlayerResponse("test");
                    break;

                case "Calidur":
                    UIManager.ShowNotification(Localization.strings_hollowrock.Calidur_1);
                    break;

                case "Markus Brakk":
                    UIManager.InitiateTrade(
                        "Markus Brakk", 
                        World.WorldCharacters.standardWorldCharacters.Where(x => x.ID == "hollowrock_markusbrakk").First().offeredItems,
                        World.WorldCharacters.standardWorldCharacters.Where(x => x.ID == "hollowrock_markusbrakk").First().Position);
                    break;
            }
        }

        private string GetRandomTextFromGindella()
        {
            Random rand = new Random();
            int temp = rand.Next(2, 10);

            if (temp == 2)
                return Localization.strings_hollowrock.Gindella_2;
            if (temp == 3)
                return Localization.strings_hollowrock.Gindella_3;
            if (temp == 4)
                return Localization.strings_hollowrock.Gindella_4;
            if (temp == 5)
                return Localization.strings_hollowrock.Gindella_5;
            if (temp == 6)
                return Localization.strings_hollowrock.Gindella_6;
            if (temp == 7)
                return Localization.strings_hollowrock.Gindella_7;
            if (temp == 8)
                return Localization.strings_hollowrock.Gindella_8;
            if (temp == 9)
                return Localization.strings_hollowrock.Gindella_9;

            return "";
        }

        public void InteractWithObject(string iObject)
        {
            if (iObject == "SignPost01")
            {
                UIManager.ShowNotification(Localization.strings_hollowrock.hollowrock_SignPost01);
            }

            if (iObject == "SignPost02")
            {
                UIManager.ShowNotification(Localization.strings_hollowrock.hollowrock_SignPost02);
            }

            if (iObject == "SignPost03")
            {
                UIManager.ShowNotification(Localization.strings_hollowrock.hollowrock_SignPost03);
            }

            if (iObject == "abandonedHouse")
            {
                if(!Player.GetChapterProgression("hollowrock").Any(q => q.id == "abandonedHouse_1"))
                {
                    UIManager.ShowNotification(Localization.strings_hollowrock.abandonedHouse_01);
                    Player.SetChapterProgression("hollowrock", "abandonedHouse_1");
                    return;
                }
                if (Player.GetChapterProgression("hollowrock").Any(q => q.id == "abandonedHouse_1"))
                    EffectManager.CreateEffect("house", EffectType.FlashScreen, 2, new string[] {"hface1", "hface2"}, "i_see_you_voice", Lightning.AMBIENTCOLOR_NIGHT, Localization.strings_hollowrock.abandonedHouse_02);

            }
        }

        public void SpawnObject(string itemId, string textureName, int width, int height, string objectName, Vector2 spawnPosition, string objectType)
        {

        }

        public void Update(GameTime gameTime)
        {
            if (!Player.GetChapterProgression("hollowrock").Any(q => q.id == "entry"))
            {
                UIManager.ShowStoryTextbox(Localization.strings_hollowrock.hollowrock_story_01, new Vector2(600, 100), "hollowrock");
                QuestManager.AddQuestToQuestLog(Localization.strings_names.location_hollowrock, Localization.strings_questlog.hollowrock_story_01);
                Player.SetChapterProgression("hollowrock", "entry");
            }
        }

        public void HandleNPCProximity(string npcId)
        {
            if (npcId == "hollowrock_carlacalidi")
            {
                if (QuestManager.triggeredNPCs.Contains(npcId))
                    return;

                QuestManager.triggeredNPCs.Add(npcId);
                UIManager.ShowAlert("triggered " + npcId, "Help us! The house is on fire!");

                QuestManager.timedEvents.Add(new TimedEvent("hollowRockBurn", 30));
            }
        }
    }
}
