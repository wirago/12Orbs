using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.Animations;
using Orbs.Effects;
using Orbs.Interface;
using Orbs.LootSystem;
using Orbs.NPC;
using Orbs.World;
using Penumbra;
using System;

namespace Orbs.Combat
{
    public class SpellAttack
    {
        private SpellAnimation spellAnimation;
        private PlayerCastAnimation playerCastAnimation;

        public bool isActive;
        private string spellTarget; //projectile, aoe, self
        private int minDmg, maxDmg, range;
        private Vector2 startPosition;
        private EnemyNPC targetEnemy;
        private Light light;

        [Obsolete]
        public SpellAttack(string spellId, int minDmg, int maxDmg, string spellTarget)
        {
            spellAnimation = new SpellAnimation(Orbs.content.Load<Texture2D>(@"Animations\Spells\" + spellId));
            isActive = true;
            this.maxDmg = maxDmg;
            this.minDmg = minDmg;
            this.spellTarget = spellTarget;

            startPosition = new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y);
            Orbs.soundEffects.PlaySound(@"Spells\" + spellId, 0.5f);
        }

        public SpellAttack(Spell castedSpell)
        {
            spellAnimation = new SpellAnimation(Orbs.content.Load<Texture2D>(@"Animations\Spells\" + castedSpell.Id));
            playerCastAnimation = new PlayerCastAnimation();

            isActive = true;
            maxDmg = castedSpell.MaxDmg;
            minDmg = castedSpell.MinDmg;
            spellTarget = castedSpell.SpellTarget;
            range = castedSpell.Range;
            startPosition = new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y);

            light = new PointLight
            {
                Scale = new Vector2(150, 100), //Range of the light
                ShadowType = ShadowType.Solid,
                Position = new Vector2(startPosition.X, startPosition.Y),
                Color = Color.Yellow
            };
            EffectManager.lightning.effectComponent.Lights.Add(light);
            
            Orbs.soundEffects.PlaySound(@"Spells\" + castedSpell.Id, 0.5f);
        }

        public void Update(GameTime gameTime)
        {
            if (Vector2.Distance(startPosition, spellAnimation.Position) > range)
                SetInactive();

            if (isActive)
            {
                spellAnimation.Update(gameTime);
                Vector2 newPos = new Vector2(
                    spellAnimation.Position.X + spellAnimation.Width / 2,
                    spellAnimation.Position.Y + spellAnimation.Height / 2
                    );
                EffectManager.lightning.effectComponent.Lights[EffectManager.lightning.effectComponent.Lights.IndexOf(light)].Position = newPos;
            }
                
            targetEnemy = SpellHit();

            if (targetEnemy != null && targetEnemy.isHostile)
            {
                int damage = CausedDamage();
                targetEnemy.GotHit(damage);
                UIManager.ShowCombatText(
                    "enemyHit" + gameTime.TotalGameTime.Seconds,
                    damage.ToString(),
                    targetEnemy.Position);
                Orbs.soundEffects.PlaySound("hit_1", 0.2f);
            }

            playerCastAnimation.Update(gameTime);
        }

        private int CausedDamage()
        {
            return new Random().Next(minDmg, maxDmg);
        }

        private EnemyNPC SpellHit()
        {
            foreach (EnemyNPC enemy in WorldCharacters.enemyWorldCharacters)
            {
                if (spellAnimation.BoundingBox.Intersects(enemy.CollisionBox))
                {
                    SetInactive();
                    Player.ModifyCombatBalance(-5);
                    return enemy;
                }
            }

            foreach ( Rectangle r in Tiled.TileMap.collisionObjects)
                if (spellAnimation.BoundingBox.Intersects(r))
                {
                    SetInactive();
                    return null;
                }

            return null;
        }

        private void SetInactive()
        {
            isActive = false;
            EffectManager.lightning.effectComponent.Lights.Remove(light);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(isActive)
            {
                spellAnimation.Draw(spriteBatch);
                playerCastAnimation.Draw(spriteBatch);
            }
               
        }
    }
}
