using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public class Icon
    {
        private Texture2D texture;
        private Vector2 texturePosition;
        private string id;
        public Rectangle TextureBoundingbox { private set; get; }

        public Icon(string id, string source, Vector2 position)
        {
            texture = Orbs.content.Load<Texture2D>(source);
            texturePosition = position;
            this.id = id;
            TextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public Icon(string id, Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            texturePosition = position;
            this.id = id;
            TextureBoundingbox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, texturePosition, Color.White);
        }
    }
}
