using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Orbs.NPC;
using Orbs.Interface;
using Orbs.Animations;
using System;

namespace Orbs.Combat
{
    public class EnemyAttack
    {
        public bool attackInProgress = false;
        public EnemyNPC attackingEnemy;

        //TODO get from equipment/spell
        private EnemyAttackAnimation attackAnimation;

        public EnemyAttack(EnemyNPC attackingEnemy, EnemyAttackAnimation attackAnimation)
        {
            if (attackAnimation == null)
                this.attackAnimation = new EnemyAttackAnimation(Orbs.content.Load<Texture2D>("Animations/Attack/sword"), true);
            else
                this.attackAnimation = attackAnimation;

            this.attackingEnemy = attackingEnemy;

        }


        internal void Update(GameTime gameTime, float hitChance)
        {

            attackAnimation.BoundingBox = attackingEnemy.BoundingBox;

            if (PlayerInBounds() && !attackInProgress && attackingEnemy.secondsSinceLastAttack >= attackingEnemy.attackSpeed)
            {
                attackInProgress = true;

                if (Orbs.rand.NextDouble() > hitChance) //missed
                {
                    UIManager.ShowCombatText(
                    "playerHit" + gameTime.TotalGameTime.Seconds,
                    "Missed",
                    new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y));
                    Orbs.soundEffects.PlaySound("hit_1", 0.2f);
                }
                else
                {
                    int damage = CausedDamage();
                    Player.PlayerGotHit(damage);

                    UIManager.ShowCombatText(
                        "playerHit" + gameTime.TotalGameTime.Seconds,
                        damage.ToString(),
                        new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y));
                    Orbs.soundEffects.PlaySound("hit_1", 0.2f);
                }
            }

            if (PlayerDecoyInBounds() && !attackInProgress && attackingEnemy.secondsSinceLastAttack >= attackingEnemy.attackSpeed)
            {
                attackInProgress = true;

                if (Orbs.rand.NextDouble() > hitChance) //missed
                {
                    UIManager.ShowCombatText(
                    "decoyHit" + gameTime.TotalGameTime.Seconds,
                    "Missed",
                    new Vector2(Player.playerDecoy.Position.X, Player.playerDecoy.Position.Y));
                    Orbs.soundEffects.PlaySound("hit_1", 0.2f);
                }
                else
                {
                    int damage = CausedDamage();
                    Player.playerDecoy.GotHit(damage);

                    UIManager.ShowCombatText(
                        "decoyHit" + gameTime.TotalGameTime.Seconds,
                        damage.ToString(),
                        new Vector2(Player.playerDecoy.Position.X + Player.playerDecoy.BoundingBox.Width / 4, Player.playerDecoy.Position.Y));
                    Orbs.soundEffects.PlaySound("hit_1", 0.2f);
                }
            }
            //TODO add sound if not hit

            attackAnimation.Update(gameTime, this);
        }


        public void StopAttackAnimation()
        {
            attackInProgress = false;
        }

        private int CausedDamage()
        {
            return new Random().Next(attackingEnemy.minDmg, attackingEnemy.maxDmg);
        }

        private bool PlayerInBounds()
        {
            return attackAnimation.BoundingBox.Intersects(Player.PlayerPositionRectangle);
        }

        private bool PlayerDecoyInBounds()
        {
            if (Player.playerDecoy != null)
                return attackAnimation.BoundingBox.Intersects(Player.playerDecoy.CollisionBox);
            else
                return false;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            attackAnimation.Draw(spriteBatch, attackingEnemy.isAttacking);
        }
    }
}
