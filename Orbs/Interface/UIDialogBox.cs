using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public class UIDialogBox : UIElement
    {
        public static bool nextText = false;
        public static bool isLastText = false;

        private static Texture2D dialogBoxTexture = Orbs.content.Load<Texture2D>(@"UserInterface\dialogbox");
        private static Vector2 dialogBoxPosition = new Vector2(
            (Orbs.graphics.PreferredBackBufferWidth - dialogBoxTexture.Width) / 2,
            (Orbs.graphics.PreferredBackBufferHeight - dialogBoxTexture.Height) / 2);

        private Color textColor = new Color(0, 0, 0, 10);

        private List<string> content;
        private int curTextNo = 0;
        private Vector2 contentPosition = dialogBoxPosition + new Vector2(330, 610);

        private Vector2 lastHistoryTextPosition = dialogBoxPosition + new Vector2(380, 40);
        private Dictionary<Vector2, string> dialogHistory = new Dictionary<Vector2, string>();

        private Texture2D charLeft;
        private Texture2D charRight;
        private Vector2 charLeftPosition = dialogBoxPosition + new Vector2(70, 110);
        private Vector2 charRightPosition = dialogBoxPosition + new Vector2(835, 100);

        private string targetName;
        private Vector2 targetNameplateOffset = new Vector2(110, 550);
        private Vector2 targetNamePosition;
        private Vector2 playerNamePosition = dialogBoxPosition + new Vector2(970, 570); 

        public UIButton okButton;
        private Texture2D button_ok = Orbs.content.Load<Texture2D>(@"Textures\Buttons\Ribbon_small");
        private Vector2 okButtonPosition = dialogBoxPosition + new Vector2(890, 630);

        private string textToMove = "";
        private Vector2 textToMovePosition;
        private Vector2 textToMoveTargetPosition;
        private bool textMoving = false;
        private int textMovementSpeed = 1500; //pixels per second

        public UIDialogBox(string charLeft, string targetName, string charRight, List<string> content) : base("dialogbox", dialogBoxPosition)
        {
            this.content = content;
            this.content.Add(Localization.strings.system_End); // Adds "End"
            this.charLeft =  Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\" + charLeft);
            this.charRight = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\" + charRight); //Usually the player
            this.targetName = targetName;

            targetNamePosition = GetNamePosition(targetName);

            okButton = new UIButton("ok", button_ok, okButtonPosition, "Ok", "ok", LabelOrientation.TopCenter);
        }

        /// <summary>
        /// Returns the absolute position for the name of the dialog partner
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Vector2 GetNamePosition(string name)
        {
            Vector2 nameSize = Orbs.spriteFontDefault.MeasureString(name);
            Vector2 centeredPosition = new Vector2((180 - nameSize.X) / 2, (55 - nameSize.Y) / 2); //size of nameplate = 180x50
            return dialogBoxPosition + targetNameplateOffset + centeredPosition;
        }

        /// <summary>
        /// Changes Text Color Alpha to black to 
        /// </summary>
        private void TextFadeIn()
        {
            if (textColor.A <= 240)
                textColor.A++;
            else
                textColor.A = 255;
        }

        /// <summary>
        /// Checks if there is more to display. If yes it pushes the current text to the history and sets the new text as current
        /// If no the dialog is marked as completed.
        /// </summary>
        private void GetNextText()
        {
            if (content.Count - 1 > curTextNo) //Check if there is another text to display
            {
                MoveTextToHistory();

                curTextNo++;
                nextText = false;
            }

            if (content.Count - 1 == curTextNo)
                isLastText = true;
        }

        /// <summary>
        /// Moves text up to the text history area
        /// </summary>
        private void MoveTextToHistory()
        {
            if (textMoving) //Clicked next before animation finished
                dialogHistory.Add(new Vector2(lastHistoryTextPosition.X, lastHistoryTextPosition.Y), content[curTextNo]);

            Vector2 tmp = Orbs.spriteFontDefault.MeasureString(content[curTextNo]);
            lastHistoryTextPosition.Y += tmp.Y + 10;

            textToMovePosition = contentPosition;
            textToMoveTargetPosition = lastHistoryTextPosition;

            textMoving = true;
            textToMove = content[curTextNo];
            textToMoveTargetPosition = lastHistoryTextPosition;
        }

        private void UpdateMovingText(GameTime gameTime)
        {
            if(Vector2.Distance(textToMovePosition, textToMoveTargetPosition) < 20)
            {
                textMoving = false;
                textToMove = "";
                dialogHistory.Add(new Vector2(lastHistoryTextPosition.X, lastHistoryTextPosition.Y), content[curTextNo - 1]);
            }
            else
            {
                Vector2 direction = lastHistoryTextPosition - textToMovePosition;
                direction.Normalize();

                textToMovePosition += direction * textMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(charLeft, charLeftPosition, Color.White);
            spriteBatch.Draw(charRight, charRightPosition, Color.White);
            spriteBatch.Draw(dialogBoxTexture, dialogBoxPosition, Color.White);

            spriteBatch.DrawString(Orbs.spriteFontDefault, content[curTextNo], contentPosition, textColor);

            spriteBatch.DrawString(Orbs.spriteFontDefault, targetName, targetNamePosition, Color.Black);
            spriteBatch.DrawString(Orbs.spriteFontDefault, "You", playerNamePosition, Color.Black);

            if (dialogHistory.Count > 0)
            {
                int i = 0;
                Color color;
                foreach (KeyValuePair<Vector2, string> dh in dialogHistory)
                {
                    color = i % 2 == 0 ? Color.Black : Color.White;
                    i++;

                    spriteBatch.DrawString(Orbs.spriteFontDefault, dh.Value, dh.Key, color);
                }
            }

            if (textToMove != "")
                spriteBatch.DrawString(Orbs.spriteFontDefault, textToMove, textToMovePosition, textColor);

            okButton.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if(textColor.A != 255)
                TextFadeIn();

            if (nextText)
                GetNextText();

            if (textMoving)
                UpdateMovingText(gameTime);
        }
    }
}
