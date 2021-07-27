using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Orbs.Interface;

namespace Orbs.QuestSystem
{
    public static class QuestManager
    {
        public static ILevel currentLevel;

        static Texture2D questWindowTexture;
        static Vector2 questWindowPosition;
        static Vector2 firstQTitleOffset = new Vector2(140, 80);
        static Vector2 firstQTextOffset = new Vector2(580, 80);
        static SortedDictionary<string, List<string>> questLogContent = new SortedDictionary<string, List<string>>();
        static Dictionary<string, Rectangle> questTitlePositions = new Dictionary<string, Rectangle>();

        public static List<string> triggeredNPCs = new List<string>(); //to track NPC who are already triggered to avoid trigger spam
        public static List<TimedEvent> timedEvents = new List<TimedEvent>();

        private const int QUESTTEXTSIZE = 18;
        private const int QUESTTEXTSPACING = 40;
        private const int PROXIMITYTRANGE = 300;

        private static string selectedQuest = "";

        public static void InitializeLevel()
        {

            switch (Orbs.curLevel)
            {
                case "wuranforest":
                    currentLevel = new wuranforest();
                    break;
                case "hollowrock":
                    currentLevel = new hollowrock();
                    break;
                case "mayorhouse":
                    currentLevel = new hollowrock();
                    break;
                case "pub":
                    currentLevel = new hollowrock();
                    break;
                default:
                    currentLevel = new wuranforest();
                    break;
            }

            questWindowTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Questbook");
            questWindowPosition = new Vector2((Orbs.graphics.PreferredBackBufferWidth - questWindowTexture.Width) / 2, (Orbs.graphics.PreferredBackBufferHeight - questWindowTexture.Height) / 2);
        }

        public static void AddQuestToQuestLog(string title, string content)
        {
            if(questLogContent.ContainsKey(title))
            {
                if (questLogContent[title].Contains(content))
                    return;

                questLogContent[title].Add(content);
            }
            else
            {
                questLogContent.Add(title, new List<string>() {content});
            }

            UIManager.ShowAlert("newQuestlogEntry", Localization.strings.system_addedQuest);
            Orbs.soundEffects.PlaySound("sfx_scribble", 1.0f);
        }

        public static void InteractWithNPC(string name)
        {
            currentLevel.InteractWithNPC(name);
        }

        public static void InteractWithObject(string iObject)
        {

            currentLevel.InteractWithObject(iObject);
        }

        public static void HandleTextInput(string text)
        {
            currentLevel.HandleTextInput(text);
        }

        public static void HandleItemPickup(string itemName)
        {
            currentLevel.HandleItemPickup(itemName);
        }

        public static void CheckNPCProximity()
        {
            foreach(NPC.StandardNPC npc in World.WorldCharacters.standardWorldCharacters)
                if(npc.proximityRectangle.Intersects(Player.PlayerPositionRectangle))
                    currentLevel.HandleNPCProximity(npc.ID);
        }

        public static void DrawQuestWindow()
        {
            Orbs.spriteBatch.Draw(questWindowTexture, questWindowPosition, Color.White);

            if(questLogContent.Count == 0) //Questlog empty
                return;

            if (selectedQuest == "")
                selectedQuest = questTitlePositions.Keys.First();

            int curQTitle = 0;

            foreach(string questTitle in questLogContent.Keys)
            {

                float xPos = questWindowPosition.X + firstQTitleOffset.X;
                float yPos = questWindowPosition.Y + firstQTitleOffset.Y + QUESTTEXTSPACING * curQTitle;

                Orbs.spriteBatch.DrawString(Orbs.spriteFontHandwriting, questTitle.ToString(), new Vector2(xPos, yPos), Color.Black);

                //For Postioning Debug
                //int bw = 2; // Border width
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(questTitlePositions[questTitle].Left, questTitlePositions[questTitle].Top, bw, questTitlePositions[questTitle].Height), Color.Black); // Left
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(questTitlePositions[questTitle].Right, questTitlePositions[questTitle].Top, bw, questTitlePositions[questTitle].Height), Color.Black); // Right
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(questTitlePositions[questTitle].Left, questTitlePositions[questTitle].Top, questTitlePositions[questTitle].Width, bw), Color.Black); // Top
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle(questTitlePositions[questTitle].Left, questTitlePositions[questTitle].Bottom, questTitlePositions[questTitle].Width, bw), Color.Black); // Bottom

                curQTitle++;

                if(questTitle == selectedQuest)
                {
                    int questtextYOffset = 0;

                    foreach(string questText in questLogContent[selectedQuest])
                    {
                        float xQTxtPos = questWindowPosition.X + firstQTextOffset.X;
                        float yQTxtPos = questWindowPosition.Y + firstQTextOffset.Y + questtextYOffset;
                        questtextYOffset += 
                            (int)Orbs.spriteFontHandwriting.MeasureString(questText).Y + 
                            (int)Orbs.spriteFontHandwriting.MeasureString(Localization.strings.system_questTextSeparator).Y;

                        Orbs.spriteBatch.DrawString(Orbs.spriteFontHandwriting, questText + Localization.strings.system_questTextSeparator, new Vector2(xQTxtPos, yQTxtPos), Color.Black);
                    }
                }
            }
        }

        public static void HandleQuestSelection(Vector2 mouseWindowPosition)
        {
            foreach(KeyValuePair<string, Rectangle> questTitlePosition in questTitlePositions)
            {
                if(questTitlePosition.Value.Contains(mouseWindowPosition))
                {
                    selectedQuest = questTitlePosition.Key;
                    return;
                }
            }
        }

        public static string[] HandleEnemyKill(string enemyID)
        {
            return currentLevel.HandleEnemyKill(enemyID);
        }

        public static void GetQuestTitlePositions()
        {
            int i = 0;
            Rectangle position = new Rectangle();
            position.X = (int)questWindowPosition.X + (int)firstQTitleOffset.X;
            position.Height = QUESTTEXTSIZE;
            position.Width = 370; //Max Line Length

            foreach(string questTitle in questLogContent.Keys)
            {
                position.Y = (int)questWindowPosition.Y + (int)firstQTitleOffset.Y + QUESTTEXTSIZE * i + (QUESTTEXTSPACING - QUESTTEXTSIZE) * i;
                i++;

                if(!questTitlePositions.ContainsKey(questTitle))
                    questTitlePositions.Add(questTitle, position);
            }
        }

        internal static void Update(GameTime gameTime)
        {
            currentLevel.Update(gameTime);
            CheckNPCProximity();

            foreach (TimedEvent te in timedEvents)
                te.Update(gameTime);
        }
    }
}
