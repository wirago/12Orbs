using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.Interface
{
    public class UIEnemyNameplate
    {
        private Texture2D nameplateBackground;
        private static Texture2D healthbar_red = Orbs.content.Load<Texture2D>(@"UserInterface\healthbar_red_px");
        private static Texture2D healthbar_green = Orbs.content.Load<Texture2D>(@"UserInterface\healthbar_green_px");
        private Vector2 nameOffset;
        private Vector2 healthBarOffset;

        public Combat.EnemyDebuff[] debuffs = new Combat.EnemyDebuff[4] { null, null, null, null };

        private string name;
        public int maxHealth, curHealth;
        public Vector2 position;
        public LootSystem.EnemyType type;

        public UIEnemyNameplate(LootSystem.EnemyType type, string name, int maxHealth, int curHealth)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.curHealth = curHealth;
            this.type = type;

            if (type == LootSystem.EnemyType.Creep)
            {
                nameplateBackground = Orbs.content.Load<Texture2D>(@"UserInterface\enemy_PlateDefault");
                nameOffset = new Vector2(80, 30);
                healthBarOffset = new Vector2(80, 60);
            }
            else if (type == LootSystem.EnemyType.Elite)
            {
                nameplateBackground = Orbs.content.Load<Texture2D>(@"UserInterface\enemy_PlateElite");
                nameOffset = new Vector2(90, 20);
                healthBarOffset = new Vector2(90, 50);

            }
            else if (type == LootSystem.EnemyType.Boss)
            {
                nameplateBackground = Orbs.content.Load<Texture2D>(@"UserInterface\enemy_PlateBoss");
                nameOffset = new Vector2(100, 50);
                healthBarOffset = new Vector2(100, 80);
            }
            position = new Vector2(0, 100);
        }

        public void AddDebuff(string debuffId, string debuffName)
        {
            int freeIndex = 0;

            for(int i=0; i <= 3; i++)
            {
                if(debuffs[i] == null)
                {
                    freeIndex = i;
                    break;
                }
                else
                {
                    if (debuffs[i].debuffId == debuffId)
                    {
                        freeIndex = i;
                        break;
                    }
                }
            }

            debuffs[freeIndex] = new Combat.EnemyDebuff(debuffId, debuffName, new Vector2(75 + 20 * (freeIndex + 1), position.Y + 75));
        }

        public void Update(int curHealth)
        {
            this.curHealth = curHealth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(nameplateBackground, position, Color.White);
            spriteBatch.DrawString(Orbs.spriteFontDefault, name, position + nameOffset, Color.White);

            float percent = (float)curHealth / maxHealth;
            spriteBatch.Draw(healthbar_red,
                new Rectangle((int)position.X + (int)healthBarOffset.X, (int)position.Y + (int)healthBarOffset.Y, 150, 4), Color.White);
            spriteBatch.Draw(healthbar_green,
                new Rectangle((int)position.X + (int)healthBarOffset.X, (int)position.Y + (int)healthBarOffset.Y, (int)(150 * percent), 4), Color.White);

            foreach(Combat.EnemyDebuff debuff in debuffs)
            {
                if (debuff != null)
                {
                    debuff.Draw(spriteBatch);

                    if (debuff.isHovered)
                        spriteBatch.DrawString(
                            Orbs.spriteFontDefault, debuff.displayName, 
                            new Vector2(debuff.boundingBox.X + 20, debuff.boundingBox.Y + 20), 
                            Color.Yellow);
                }
            }
        }

        public void CheckDebuffTooltip(Vector2 mouseWindowPos)
        {
            foreach(Combat.EnemyDebuff debuff in debuffs)
            {
                if (debuff != null)
                {
                    if (debuff.boundingBox.Contains(mouseWindowPos))
                        debuff.isHovered = true;
                    else
                        debuff.isHovered = false;
                }
            }
        }
    }
}
