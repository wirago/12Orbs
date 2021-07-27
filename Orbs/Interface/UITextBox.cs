using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Orbs.Interface
{
    public class UITextBox : UIElement
    {
        private string content;
        private static Texture2D textBoxTexture = Orbs.content.Load<Texture2D>(@"UserInterface\messagebox");
        private static Vector2 notificationPosition = new Vector2(
            (Orbs.graphics.PreferredBackBufferWidth - textBoxTexture.Width) / 2,
            (Orbs.graphics.PreferredBackBufferHeight - textBoxTexture.Height) / 2);

        private static Vector2 storyBoxPosition = new Vector2(
            (Orbs.graphics.PreferredBackBufferWidth - 1200) / 2,
            (Orbs.graphics.PreferredBackBufferHeight - 800) / 2);

        private UIButton okButton = new UIButton(
                "ok", @"UserInterface\button_OK",
                new Vector2(notificationPosition.X + (textBoxTexture.Width - 75) / 2, notificationPosition.Y + (textBoxTexture.Height - 100)),
                "ok");

        private Vector2 contentPosition;
        private Vector2 contentOffset = new Vector2(100, 80);

        private Rectangle yesButton = new Rectangle((int)notificationPosition.X + 190, (int)notificationPosition.Y + 320, 80, 30);
        private Rectangle noButton = new Rectangle((int)notificationPosition.X + 342, (int)notificationPosition.Y + 320, 80, 30);
        private Rectangle okButtonStoryBoxRect = new Rectangle((int)storyBoxPosition.X + 1040, (int)storyBoxPosition.Y + 740, 150, 50);

        private bool isYesNoBox = false;
        public bool isActive = false;
        private bool isStoryBox = false;

        private SpriteFont spriteFont = Orbs.spriteFontDefault;
        private Color textColor;

        private List<ContentPiece> multiContent = new List<ContentPiece>();

        public UITextBox(string content) : base("textbox", @"UserInterface\messagebox", notificationPosition)
        {
            string tempContent = content;
            //Cut the Content Into pieces based on some Metachars
            List<string> pieces = new List<string>();
            //Go around while the Content is not Empty
            while (tempContent != "")
            {
                if (tempContent.Contains("${")) //Check for a meta Beginning
                {
                    int a = tempContent.IndexOf("${");
                    int b = tempContent.IndexOf("}");
                    //string piece = tempContent.Substring(a, b);
                    if (a != 0) //The first meta start is not the beginning of the Content
                    {
                        pieces.Add(tempContent.Substring(0, a));
                    }
                    pieces.Add(tempContent.Substring(a, b-a+1));
                    tempContent = tempContent.Remove(0, b+1); //remove the piece from the content
                }
                else //Case that no meta is used
                {
                    pieces.Add(tempContent);
                    tempContent = "";
                }
            }

            float tempY = 0;

            for (int i = 0; i < pieces.Count; i++)//loop the pieces
            {
                Color col = new Color(210, 170, 150, 255);
                SpriteFont textFont_ = Orbs.spriteFontDefault;
                Vector2 pos = Vector2.Zero;
                if (pieces[i].Contains("${"))//just modify the meta pieces
                {
                    pieces[i] = pieces[i].Remove(0, 2); //remove the beginning
                    pieces[i] = pieces[i].Remove(pieces[i].Length - 1, 1); //remove the ending

                    //Check for meta and set the contentPiece
                    char meta = pieces[i][0];

                    switch (meta)
                    {
                        case '*':
                            col = Color.Red;
                            pieces[i] = pieces[i].Remove(0, 1);
                            break;
                        case '#':
                            col = Color.LightGreen;
                            pieces[i] = pieces[i].Remove(0, 1);
                            break;
                        case '§':
                            col = Color.Yellow;
                            textFont_ = Orbs.spriteFontHandwriting;
                            pieces[i] = pieces[i].Remove(0, 1);
                            break;
                        default:
                            col = Color.White;
                            break;
                    }
                }
                pos = notificationPosition + contentOffset;

                pos.Y += tempY;
                tempY += Orbs.spriteFontDefault.MeasureString(pieces[i]).Y;
                multiContent.Add(new ContentPiece(pieces[i], pos, col, textFont_));
            }

            this.content = "";
            contentPosition = Vector2.Zero;
            isActive = true;
            textColor = new Color(210, 170, 150, 255);

        }

        public UITextBox(string content, Vector2 position) : base("textbox", @"UserInterface\messagebox", position)
        {
            this.content = content;
            contentPosition = position + contentOffset;
            isActive = true;
            textColor = new Color(210, 170, 150, 255);
        }

        /// <summary>
        /// Used for asking questions
        /// </summary>
        public UITextBox(string content, string question, Vector2 position) : base("textbox", @"Textures\BackBaseColor_Blank", position)
        {
            this.content = content;
            contentPosition = position + contentOffset;
            isActive = true;
            textColor = new Color(210, 170, 150, 255);
        }

        public UITextBox(string content, bool isYesNoBox) : base("textbox", @"Textures\BackBaseColor_YN", notificationPosition)
        {
            this.isYesNoBox = isYesNoBox;
            this.content = content;
            contentPosition = notificationPosition + contentOffset;
            isActive = true;
            textColor = new Color(210, 170, 150, 255);
        }

        /// <summary>
        /// Used for story telling
        /// </summary>
        public UITextBox(string content, Vector2 contentPosition, string image) : base("textbox", @"Textures\QuestTextbox\" + image, storyBoxPosition)
        {
            this.content = content;
            this.contentPosition = contentPosition + storyBoxPosition;
            textColor = new Color(210, 170, 150, 10);
            isActive = true;
            isStoryBox = true;
            spriteFont = Orbs.spriteFontHandwriting;
        }

        /// <summary>
        /// Checks if Yes, No or Ok is clicked
        /// </summary>
        /// <param name="mouseWindowPosition"></param>
        /// <returns>1 = yes or ok, 0 = no, -1 no hit </returns>
        public int HandleMousClick()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = mouseState.Position.ToVector2();

            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                if (isYesNoBox)
                {
                    if (yesButton.Contains(mousePos))
                    {
                        Player.notificationResponse = 1;
                        Orbs.soundEffects.PlaySound("mouseclick", 1.0f);
                        isActive = false;
                        return 1;
                    }

                    if (noButton.Contains(mousePos))
                    {
                        Player.notificationResponse = 0;
                        Orbs.soundEffects.PlaySound("mouseclick", 1.0f);
                        isActive = false;
                        return 1;
                    }
                }
                else if (isStoryBox)
                {
                    if (okButtonStoryBoxRect.Contains(mousePos))
                    {
                        Player.notificationResponse = 1;
                        Orbs.soundEffects.PlaySound("mouseclick", 1.0f);
                        isActive = false;
                        return 1;
                    }
                }

                else if (okButton.IsClicked(mousePos))
                {
                    Player.notificationResponse = 1;
                    Orbs.soundEffects.PlaySound("mouseclick", 1.0f);
                    isActive = false;
                    return 1;
                };
            }

            return 0;
        }

        private void TextFadeIn()
        {
            if (textColor.A <= 240) // Text Fade In
            {
                textColor.A++;
            }
            else
                textColor.A = 255;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UIManager.textboxes.Count > 0)
            {
                spriteBatch.Draw(UITexture, UITexturePosition, Color.White);
                for (int i = 0; i < multiContent.Count; i++)
                {
                    spriteBatch.DrawString(multiContent[i].textFont, multiContent[i].text, multiContent[i].textPosition, multiContent[i].textColor);
                }
                spriteBatch.DrawString(spriteFont, content, contentPosition, textColor);

                if (!isStoryBox)
                    spriteBatch.Draw(okButton.UITexture, okButton.UITexturePosition, Color.White);
                else
                    spriteBatch.Draw(okButton.UITexture, new Vector2(okButtonStoryBoxRect.X, okButtonStoryBoxRect.Y), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(isStoryBox)
                TextFadeIn();
        }
    }
}
