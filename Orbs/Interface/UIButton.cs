using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.LootSystem;

namespace Orbs.Interface
{
    public enum LabelOrientation {Center, TopLeft, TopCenter };

    public class UIButton : UIElement
    {
        public string label;
        private Vector2 labelSize;
        private Vector2 labelPosition;

        private Color defaultColor = Color.Black;
        private Color hoveredColor = Color.White;
        private Vector2 hoveredOffset = new Vector2(0, -3);

        public string onClickAction;
        private Texture2D onClickTexture; //TODO PlaceHolder for future implementation

        public UIButton(string id, string source, Vector2 position, string label) : base (id, source, position)
        {
            this.label = label == null ? "" : label;
            this.labelPosition = position;
            labelSize = Orbs.spriteFontMenu.MeasureString(label);
        }

        /// <summary>
        /// For blank buttons with single label only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="label"></param>
        public UIButton(string id, Rectangle clickArea, string label, string onClickAction) : base (id, new Vector2(clickArea.X, clickArea.Y))
        {
            this.label = label;
            this.onClickAction = onClickAction;
            labelSize = Orbs.spriteFontMenu.MeasureString(label);
            UITexture = null;
            UITextureBoundingbox = clickArea;
            labelPosition = GetLabelPosition(LabelOrientation.Center);
        }

        public UIButton(string id, string source, Vector2 position) : base(id, source, position)
        {
            this.label = "";
            this.labelPosition = position;
        }

        /// <summary>
        /// Creates a Button with given params setting a default texture if null
        /// </summary>
        /// <param name="id">UID</param>
        /// <param name="texture">Texture for Button, nullable</param>
        /// <param name="position">Windowposition</param>
        /// <param name="label">Label</param>
        public UIButton(string id, Texture2D texture, Vector2 position, string label, string onClickAction, LabelOrientation? labelOrientation) : base (id, position)
        {
            this.label = label == null ? "" : label;
            labelSize = Orbs.spriteFontMenu.MeasureString(this.label);
            UITexture = texture == null ? Orbs.content.Load<Texture2D>(@"UserInterface\button_0") : texture;
            this.labelPosition = GetLabelPosition(labelOrientation);
            this.UITextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, UITexture.Width, UITexture.Height);
            this.onClickAction = onClickAction;
        }

        public UIButton(string id, Item item, Vector2 position) : base (id, position)
        {
            this.UITexture = item.IconTexture;
            this.UITextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, UITexture.Width, UITexture.Height);
            this.label = "";
            this.labelPosition = Vector2.Zero;
        }

        public UIButton(string id, Vector2 position) : base(id, position)
        {
            this.UITexture = null;
            this.UITextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
        }

        public UIButton(string id, Vector2 position, int width, int height) : base(id, position)
        {
            this.UITexture = null;
            this.UITextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        private Vector2 GetLabelPosition(LabelOrientation? labelOrientation)
        {
            Vector2 v = Vector2.Zero;

            if(UITexture == null)
            {
                v.X = UITextureBoundingbox.X + ((float)UITextureBoundingbox.Width / 2 - labelSize.X / 2);
                v.Y = UITextureBoundingbox.Y + ((float)UITextureBoundingbox.Height / 2 - labelSize.Y / 2);
                return v;
            }

            if(labelOrientation == LabelOrientation.TopCenter)
            {
                v.X = UITexturePosition.X + ((float)UITexture.Width / 2 - labelSize.X / 2);
                v.Y = UITexturePosition.Y + 8;

                return v;
            }

            if (labelOrientation == null || labelOrientation == LabelOrientation.Center)
            {
                v.X = UITexturePosition.X + ((float)UITexture.Width / 2 - labelSize.X / 2);
                v.Y = UITexturePosition.Y + ((float)UITexture.Height / 2 - labelSize.Y / 2);
            }
            else if( labelOrientation == LabelOrientation.TopLeft)
            {
                v.X = UITexturePosition.X + 2;
                v.Y = UITexturePosition.Y + 2;
            }
            return v;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UITexture == null && label == null) //empty button
                return;

            if (UITexture == null)
            {
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu, label, isHovered ? labelPosition - hoveredOffset : labelPosition, isHovered ? hoveredColor : defaultColor); 
            }
            else
            {
                spriteBatch.Draw(UITexture, UITexturePosition, Color.White);
                spriteBatch.DrawString(
                    Orbs.spriteFontMenu, label, isHovered ? labelPosition - hoveredOffset : labelPosition, isHovered ? hoveredColor : defaultColor);
            }
        }

        public bool IsClicked(Vector2 clickPoint)
        {
            return UITextureBoundingbox.Contains(clickPoint);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
