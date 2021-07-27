using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Combat
{
    public class EnemyDebuff
    {
        private const int ICONSIZE = 16; //16px width and height

        private Texture2D iconTexture;
        public string displayName;
        public string debuffId;
        public Rectangle boundingBox;
        public bool isHovered;

        public EnemyDebuff(string debuffId, string name, Vector2 position)
        {
            displayName = name;
            this.debuffId = debuffId;
            iconTexture = Orbs.content.Load<Texture2D>(@"UserInterface/Icons/enemy_debuff_" + debuffId);
            boundingBox = new Rectangle((int)position.X, (int)position.Y, ICONSIZE, ICONSIZE);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(iconTexture, new Vector2(boundingBox.X, boundingBox.Y), Color.White);
        }
    }
}
