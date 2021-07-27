using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.Animations;

namespace Orbs
{
    public class PlayerDecoy : SpriteAnimation
    {
        public int decoyHealth = Player.MAX_HEALTH / 2;

        private const int FRAMEHEIGHT = 96;
        private const int FRAMEWIDTH = 96;

        public PlayerDecoy(Texture2D texture, Vector2 spawnPosition) : base(texture)
        {
            Position = spawnPosition;
            isAnimating = true;

            AddAnimation("Idle_South", 0, FRAMEHEIGHT * 2, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_West", 0, FRAMEHEIGHT * 1, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_East", 0, FRAMEHEIGHT * 3, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_North", 0, FRAMEHEIGHT * 0, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);

            CurrentAnimation = "Idle_South";
        }

        public void GotHit(int dmg)
        {
            decoyHealth = decoyHealth - dmg <= 0 ? 0 : decoyHealth - dmg;
        }

        public void Update()
        {
            if (decoyHealth <= 0)
                Player.DespawnDecoy();
        }
    }
}
