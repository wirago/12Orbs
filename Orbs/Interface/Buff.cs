using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbs.Interface
{
    public class Buff
    {
        public Icon buffIcon;
        public int duration; //in seconds

        private int buffTimer;
        private Vector2 buffTimerPosition;
        private string buffId;
        private TimeSpan startedAt;
        private bool isActive;
        private string soundEffect;

        public Buff(Icon buffIcon, int duration, string buffId, string soundEffect)
        {
            this.buffIcon = buffIcon;
            this.duration = duration;
            this.buffId = buffId;
            this.soundEffect = soundEffect;

            buffTimer = duration;
            buffTimerPosition = new Vector2(
                buffIcon.TextureBoundingbox.X + (buffIcon.TextureBoundingbox.Width - Orbs.spriteFontDefault.MeasureString(duration.ToString()).X) / 2, 
                buffIcon.TextureBoundingbox.Y + (buffIcon.TextureBoundingbox.Height - Orbs.spriteFontDefault.MeasureString(duration.ToString()).Y) / 2);
            startedAt = Orbs.currentTimeSpan;
            isActive = true;
            ActivateBuff();
        } 

        private void ActivateBuff()
        {
            switch (buffId)
            {
                case "haste":
                    Player.movementspeed += 5;
                    break;
            }

            Orbs.soundEffects.PlaySound(soundEffect, 0.8f);
        }

        public void DeactivateBuff()
        {
            switch (buffId)
            {
                case "haste":
                    Player.movementspeed = Player.DEFAULT_MOVEMENTSPEED;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            buffTimer = (Orbs.currentTimeSpan - startedAt).Seconds;

            if (isActive)
            {
                if (buffTimer >= duration)
                {
                    isActive = false;
                    DeactivateBuff();
                }

                buffTimerPosition.X =
                    buffIcon.TextureBoundingbox.X + (buffIcon.TextureBoundingbox.Width - Orbs.spriteFontDefault.MeasureString((duration - buffTimer).ToString()).X) / 2;
                buffTimerPosition.Y =
                    buffIcon.TextureBoundingbox.Y + (buffIcon.TextureBoundingbox.Height - Orbs.spriteFontDefault.MeasureString((duration - buffTimer).ToString()).Y) / 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            buffIcon.Draw(spriteBatch);
            spriteBatch.DrawString(Orbs.spriteFontDefault, (duration - buffTimer).ToString(), buffTimerPosition, Color.Red);
        }
    }
}
