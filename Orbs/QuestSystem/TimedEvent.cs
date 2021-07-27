using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbs.QuestSystem
{
    public class TimedEvent
    {
        private int eventTimer; //passed duration in seconds
        private int eventDuration; //planned duration
        private string eventId;
        private TimeSpan startedAt;
        private bool isActive;
        private Vector2 eventTimerPosition = new Vector2(50, Orbs.graphics.PreferredBackBufferWidth / 2);

        public TimedEvent(string eventId, int eventDuration)
        {
            this.eventId = eventId;
            this.eventDuration = eventDuration;
            startedAt = Orbs.currentTimeSpan;
            isActive = true;
        }

        private void DeactivateEvent()
        {
            //switch (eventId)
            //{
                
            //}
        }

        public void Update(GameTime gameTime)
        {
            eventTimer = (Orbs.currentTimeSpan - startedAt).Seconds;

            if (isActive)
            {
                if (eventTimer >= eventDuration)
                {
                    isActive = false;
                    DeactivateEvent();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            spriteBatch.DrawString(Orbs.spriteFontDefault, (eventDuration - eventTimer).ToString(), new Vector2(960, 50), Color.Red);
        }
    }
}
