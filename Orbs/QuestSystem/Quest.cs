using System;
using System.Collections.Generic;

namespace Orbs.QuestSystem
{
    public class Quest
    {
        public string id;
        public bool isCompleted;

        private List<Quest> affectingQuest;

        public Quest(string id)
        {
            this.id = id;
            isCompleted = false;
        }

        public void CompleteQuest()
        {
            isCompleted = true;
        }

        //Function to Add an affectingQuest

        //Function to End all affektingQuests on Ending (differ between Broken and all in On completed quests
    }
}
