using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Orbs.NPC;
using Orbs.World;
using Orbs.Interface;
using Orbs.Animations;
using System;
using System.Collections.Generic;

namespace Orbs.Combat
{
    //Attack Class for Players Attacks
    public class Attack
    {
        public static bool attackInProgress = false;
        private List<EnemyNPC> enemiesHit;
        private int attackMinDmg, attackMaxDmg;
        private PlayerAttackAnimation attackAnimation;

        public Attack(string weaponId, int minDmg, int maxDmg)
        {
            try
            {
                attackAnimation = new PlayerAttackAnimation(Orbs.content.Load<Texture2D>("Animations/Attack/" + weaponId));
            }
            catch(Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                attackAnimation = new PlayerAttackAnimation(Orbs.content.Load<Texture2D>("Animations/Attack/w01")); //Fallback to default attack animation
            }

            attackMinDmg = minDmg;
            attackMaxDmg = maxDmg;

            Orbs.soundEffects.PlaySound(@"Weapons\" + weaponId, 0.5f);
        }

        internal void Update(GameTime gameTime)
        {
            if (!Player.isAttacking)
                return;

            enemiesHit = EnemyHit();

            if (enemiesHit == null)
            {
                attackAnimation.Update(gameTime);
                return;
            }

            if (!attackInProgress)
            {
                attackInProgress = true;
                int damageCaused;

                for (int i = 0; i <= enemiesHit.Count - 1; i++)
                {
                    damageCaused = (int)(CausedDamage() * 1.0f * ((100.0f - i * 20) / 100));
                    enemiesHit[i].GotHit(damageCaused);

                    UIManager.ShowCombatText(
                        "enemyHit" + i + gameTime.TotalGameTime.Seconds,
                        damageCaused.ToString(),
                        enemiesHit[i].Position);

                    enemiesHit[i].isHit = true;
                    enemiesHit[i].hitTimestamp = Orbs.currentTimeSpan;

                    Orbs.soundEffects.PlaySound("hit_1", 0.2f);

                    Player.currentAttackCombo++;
                    if (Player.selectedWeapon.WeaponClass == LootSystem.Weapon.weaponClass.bleed && Player.currentAttackCombo == 3)
                    {
                        Player.currentAttackCombo = 0;
                        enemiesHit[i].ApplyDebuff(LootSystem.Weapon.weaponClass.bleed);
                    }
                    else if (Player.selectedWeapon.WeaponClass == LootSystem.Weapon.weaponClass.maim && Player.currentAttackCombo == 2)
                    {
                        Player.currentAttackCombo = 0;
                        enemiesHit[i].ApplyDebuff(LootSystem.Weapon.weaponClass.maim);
                    }
                    else if (Player.selectedWeapon.WeaponClass == LootSystem.Weapon.weaponClass.blind && Player.currentAttackCombo == 3)
                    {
                        Player.currentAttackCombo = 0;
                        enemiesHit[i].ApplyDebuff(LootSystem.Weapon.weaponClass.blind);
                    }

                }
            }
            attackAnimation.Update(gameTime);
        }

        private float CausedDamage()
        {
            Player.ModifyCombatBalance(5);
            return new Random().Next(attackMinDmg, attackMaxDmg);
        }

        private List<EnemyNPC> EnemyHit()
        {
            List<EnemyNPC> returnValue = new List<EnemyNPC>();

            foreach (EnemyNPC enemy in WorldCharacters.enemyWorldCharacters)
            {
                if (attackAnimation.AttackRectangle.Intersects(enemy.CollisionBox) && enemy.isHostile)
                {
                    enemy.KnockBack(GetKnockBackVector());
                    returnValue.Add(enemy);
                }
            }

            if(returnValue.Count == 0)
                return null;

            return returnValue;
        }

        private Vector2 GetKnockBackVector()
        {
            if (Player.curDirection.EndsWith("East"))
                return new Vector2(3, 0);
            if (Player.curDirection.EndsWith("South"))
                return new Vector2(0, 3);
            if (Player.curDirection.EndsWith("West"))
                return new Vector2(-3, 0);
            if (Player.curDirection.EndsWith("North"))
                return new Vector2(0, -3);

            return Vector2.Zero;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            attackAnimation.Draw(spriteBatch);
        }
    }
}
