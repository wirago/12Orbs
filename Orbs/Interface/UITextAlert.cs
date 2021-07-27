using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public class UITextAlert : UIElement
    {
        private string content;
        private Vector2 contentPosition = new Vector2(480, 280);

        private TimeSpan createdAt;
        private int activeTime = 2; //in seconds
        private bool isActive = true;
        private int scrollSpeed = 60;

        private Color textColor = Color.Yellow;

        public UITextAlert(string id, Vector2 position) : base(id, position)
        {

        }

        public UITextAlert(string id, string content) : base (id, new Vector2(480, 280))
        {
            createdAt = Orbs.currentTimeSpan;
            this.content = content;
            this.contentPosition.X = (Orbs.graphics.PreferredBackBufferWidth - Orbs.spriteFontBig.MeasureString(content).X) / 2;

            if (UIManager.uiTextElements.Count > 0)
                contentPosition.Y += UIManager.uiTextElements.Count * 20;
        }

        public override void Update(GameTime gameTime)
        {
            if (Orbs.currentTimeSpan.Seconds - createdAt.Seconds > activeTime)
                isActive = false;

            if (!isActive)
                FadeOut(gameTime);
        }

        private void FadeOut(GameTime gameTime)
        {
            textColor *= 0.97f;
            contentPosition.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * scrollSpeed;

            if(textColor.A < 10)
                toBeRemoved = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.DrawString(Orbs.spriteFontBig, content, contentPosition, textColor);
        }
    }
}
