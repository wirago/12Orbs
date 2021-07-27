using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public class ContentPiece
    {
        public string text = "";

        public Color textColor = Color.White;

        public Vector2 textPosition = new Vector2(0, 0);

        public SpriteFont textFont = Orbs.spriteFontDefault;

        public ContentPiece(string text, Vector2 position)
        {
            this.text = text;
            textPosition = position;
        }

        public ContentPiece(string text, Vector2 position, Color textColor)
        {
            this.text = text;
            textPosition = position;
            this.textColor = textColor;
        }

        public ContentPiece(string text, Vector2 position, SpriteFont textFont)
        {
            this.text = text;
            textPosition = position;
            this.textFont = textFont;
        }

        public ContentPiece(string text, Vector2 position, Color textColor, SpriteFont textFont)
        {
            this.text = text;
            textPosition = position;
            this.textColor = textColor;
            this.textFont = textFont;
        }
    }
}
