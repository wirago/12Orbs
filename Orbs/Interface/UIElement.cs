using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public abstract class UIElement
    {
        public Vector2 UITexturePosition;
        public Texture2D UITexture;
        public Rectangle UITextureBoundingbox;
        public bool toBeRemoved = false;
        public bool isHovered = false;
        private string id;

        public string Id { get => id; set => id = value; }

        public UIElement(string id, string source, Vector2 position)
        {
            UITexture = Orbs.content.Load<Texture2D>(source);
            UITexturePosition = position;
            this.Id = id;
            UITextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, UITexture.Width, UITexture.Height);
        }

        public UIElement(string id, string source)
        {
            UITexturePosition = new Vector2(0, 0);
            this.Id = id;
        }

        public UIElement(string id, Vector2 position)
        {
            UITexturePosition = position;
            this.Id = id;
        }

        public UIElement() { }

        public bool IsHovered(Vector2 v)
        {
            return UITextureBoundingbox.Contains(v);
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
