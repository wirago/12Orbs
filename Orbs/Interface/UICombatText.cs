using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbs.Interface
{
    public class UICombatText : UIElement
    {
        private string content;
        private Vector2 contentPosition;

        private TimeSpan createdAt;
        private int activeTime = 0; //in seconds
        private bool isActive = true;
        private int scrollSpeed = 60;

        private Color textColor = Color.Red;

        public UICombatText(string id, string content, Vector2 position) : base (id, position) 
        {
            createdAt = Orbs.currentTimeSpan;
            this.content = content;
            contentPosition.X = position.X + 10;
            contentPosition.Y = position.Y - 15;

            if (UIManager.uiTextElements.Count > 0)
                contentPosition.Y += UIManager.uiTextElements.Count * 20;
        }

        public override void Update(GameTime gameTime)
        {
            if (Orbs.currentTimeSpan.Seconds - createdAt.Seconds > activeTime)
                isActive = false;

            if (UIManager.combatTexts.Count > 1)
                isActive = false;

            if (!isActive)
                FadeOut(gameTime);
        }

        private void FadeOut(GameTime gameTime)
        {
            textColor *= 0.99f;
            contentPosition.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * scrollSpeed;
            if (textColor.A < 10)
                toBeRemoved = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Orbs.spriteFontBig, content, contentPosition, textColor);
        }
    }
}
