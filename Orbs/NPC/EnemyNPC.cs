using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Orbs.World;
using Orbs.Animations;
using Orbs.LootSystem;
using System.Linq;
using System.Collections.Generic;
using System;
using Orbs.Combat;
using Orbs.Interface.TextInput;
using Orbs.Effects;
using Orbs.Interface;

namespace Orbs.NPC
{
    //public enum DebuffType {Bleed, Maim, Blind };
    

    public class EnemyNPC : SpriteAnimation, INPC
    {
        private const int AGGRESSIONRANGE = 300; //max range between player and enemy before enemy attacks
        private const int ATTACK_RANGE_OFFSET = 7; //for making sure enemy is not off for less than 5px
        //private const int SECONDSBETWEENATTACKS = 1; //seconds between attack
        private const int RETREATRANGE = 600; //range you kite an enemy
        private const int RETREATSPEED = 4;
        private const int SECONDSBETWEENBLEED = 2;
        private const float INJURYDAMAGE = 0.03f; // equals 3% of max health
        private const int SECONDSBETWEENGLYPHEFFECT = 1;
        private Rectangle aggressionArea = new Rectangle();
        private EnemyAttackAnimation attackAnimation;
        public UIEnemyNameplate namePlate;

        public int attackSpeed; //seconds between attacks
        public int secondsSinceLastAttack = 0;
        private TimeSpan lastAttackAt;
        private TimeSpan lastInjuredBleed;
        private TimeSpan lastGlyphEffect;
        private EnemyAttack attack = null;
        public int minDmg, maxDmg;
        private Color? tintColor;

        #region Physical Character Atributes
        private int movementspeed;
        private const int DEFAULTMOVEMENTSPEED = 2;
        private int interactRange = 25;
        public string Name { get; set; }
        private Vector2 nameSize;

        public int MaxHealth { get; set; }
        private int curHealth;
        private bool isAlive;
        private float hitChance = 0.9f;
        private ElementType ElementType { get; set; }


        public bool IsAlive
        {
            get
            {
                return CurHealth > 0;
            }

            set
            {
                isAlive = value;
            }
        }

        public int CurHealth
        {
            get
            {
                return curHealth;
            }

            set
            {
                if (value > 0)
                    curHealth = value;
                else
                    curHealth = 0;
            }
        }

        #endregion

        #region Waypoint System

        private Vector2 targetPosition; //current waypointPosition
        private List<Waypoint> waypoints; //List of all waypoints orderd by WaypointId
        private int currentWaypoint = 0;
        private Vector2 NPCMovement = Vector2.Zero;

        //Pathfinding
        private Point attackPath_from = Point.Zero;
        private Point attackPath_to = Point.Zero;
        private List<Point> attackPath = null;
        private int currentPathStep = 0;

        #endregion

        private string curAnimation = "";
        private Rectangle interactRectangle;

        public bool isHostile = true; //true by default, will be set on map load
        public bool isAttacking = false;
        private bool isRetreating = false;
        private bool isInjured = false;
        private bool isGlyphAffected = false;
        private bool isAttackingDecoy = false;

        private string enemyID;

        public bool isHovered = false;

        public bool isBleeding = false;
        public bool isMaimed = false;
        public bool isBlinded = false;

        public EnemyNPC(Texture2D texture, List<Waypoint> targetPositions, bool isHostile, EnemyTemplate enemyTemplate) : base(texture)
        {
            waypoints = targetPositions;
            Position = waypoints[0].GetCoords();
            targetPosition = waypoints[0].GetCoords();
            isAnimating = true;
            enemyID = enemyTemplate.uid;
            this.isHostile = isHostile;

            Name = enemyTemplate.name;
            nameSize = Orbs.spriteFontDefault.MeasureString(Name);

            switch(enemyTemplate.ElementType)
            {
                case ElementType.none:
                    tintColor = null;
                    break;

                case ElementType.fire:
                    tintColor = Color.Red;
                    break;

                case ElementType.water:
                    tintColor = Color.Blue;
                    break;

                case ElementType.earth:
                    tintColor = Color.Brown;
                    break;

                case ElementType.nature:
                    tintColor = Color.ForestGreen;
                    break;

                case ElementType.arcane:
                    tintColor = Color.Purple;
                    break;
            }

            #region Animation
            AddAnimation("Walk_South", 0, enemyTemplate.textureHeight * 0, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 4, 0.1f);
            AddAnimation("Walk_West", 0, enemyTemplate.textureHeight * 1, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 4, 0.1f);
            AddAnimation("Walk_East", 0, enemyTemplate.textureHeight * 2, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 4, 0.1f);
            AddAnimation("Walk_North", 0, enemyTemplate.textureHeight * 3, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 4, 0.1f);

            AddAnimation("Idle_South", 0, enemyTemplate.textureHeight * 0, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 1, 0.2f);
            AddAnimation("Idle_West", 0, enemyTemplate.textureHeight * 1, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 1, 0.2f);
            AddAnimation("Idle_East", 0, enemyTemplate.textureHeight * 2, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 1, 0.2f);
            AddAnimation("Idle_North", 0, enemyTemplate.textureHeight * 3, enemyTemplate.textureWidth, enemyTemplate.textureHeight, 1, 0.2f);

            CurrentAnimation = "Idle_South";
            #endregion

            interactRectangle = new Rectangle(CollisionBox.X - interactRange, CollisionBox.Y - interactRange, CollisionBox.Width + interactRange * 2, CollisionBox.Height + interactRange * 2);

            CurHealth = MaxHealth = enemyTemplate.maxHealth;
            maxDmg = enemyTemplate.maxDmg;
            minDmg = enemyTemplate.minDmg;
            attackSpeed = enemyTemplate.attackSpeed;
            movementspeed = enemyTemplate.movementSpeed;

            attackAnimation = new EnemyAttackAnimation(Orbs.content.Load<Texture2D>("Animations/Attack/" + enemyTemplate.attackAnimation), false);

            WorldCharacters.enemyWorldCharacters.Add(this);
            namePlate = new UIEnemyNameplate(enemyTemplate.EnemyType, Name, MaxHealth, curHealth);
        }

        public void UpdateEnemyNPC(GameTime gameTime)
        {
            curAnimation = "";
            NPCMovement = Vector2.Zero;

            if (!isAttacking) //is on patrol
            {
                //Has NPC reached his target? If yes send him back, if enemy was retreating reset him.
                if (Vector2.Distance(Position, targetPosition) <= 2)
                {
                    if (isRetreating)
                    {
                        if (Vector2.Distance(Position, waypoints[0].GetCoords()) <= 128)
                            ResetEnemy();
                        else
                        {
                            Point targetPoint = GetNextPathPoint();
                            targetPosition = new Vector2(targetPoint.X * 64, targetPoint.Y * 64);
                        }
                    }
                    else
                    {
                        currentWaypoint++;
                        if (currentWaypoint >= waypoints.Count())
                            currentWaypoint = 0;

                        targetPosition = waypoints[currentWaypoint].GetCoords();
                    }
                }

                //NPC not yet at his target
                if (Vector2.Distance(Position, targetPosition) > 2)
                    NPCMovement = Vector2.Normalize(targetPosition - Position);
            }
            
            if(isAttacking)
            {
                if (Vector2.Distance(Position, targetPosition) > 2)
                    NPCMovement = Vector2.Normalize(targetPosition - Position);
            }

            curAnimation = GetDirectionByMovement(NPCMovement);

            if (NPCCollidesWithPlayer(NPCMovement))
                NPCMovement = Vector2.Zero;

            if (NPCMovement.Length() != 0)
            {
                MoveBy(Vector2.Normalize(NPCMovement));

                if (CurrentAnimation != curAnimation)
                    CurrentAnimation = curAnimation;
            }
            else
            {
                if(CurrentAnimation != "Idle")
                    CurrentAnimation = "Idle_" + CurrentAnimation.Substring(5);
            }

            if (isAttacking && attack != null)
            {
                if(Player.selectedOrb != null)
                    attack.Update(gameTime, Player.selectedOrb.Id == "orb_7_luck" ? hitChance : 0.75f);
                else
                    attack.Update(gameTime, hitChance);
            }
                

            if (!isAttacking && attack != null)
                attack = null;

            if(!isInjured && (CurHealth / (float)MaxHealth) * 100 <= 20)
            {
                isInjured = true;
                lastInjuredBleed = Orbs.currentTimeSpan;
                namePlate.AddDebuff("injury", Localization.strings_buffs.debuff_injured);
            }

            CheckGlyphCollision();

            Update(gameTime);

            namePlate.Update(curHealth);

            UpdateDebuffs(gameTime);
        }

        private void UpdateDebuffs(GameTime gameTime)
        {
            foreach(EnemyDebuff debuff in namePlate.debuffs)
            {
                if(debuff != null)
                {
                    if (debuff.debuffId == "injury")
                        ApplyInjuryDamage();
                    else if (debuff.debuffId == "bleed")
                        isBleeding = true;
                    else if (debuff.debuffId == "maim")
                        isMaimed = true;
                    else if (debuff.debuffId == "blind")
                        isBlinded = true;
                }
            }

            if (isMaimed)
                movementspeed = DEFAULTMOVEMENTSPEED / 2;

            if (isBleeding)
                ApplyInjuryDamage();

            if (isBlinded)
                hitChance = 0.5f;
        }

        private bool CheckGlyphCollision()
        {
            foreach (Glyph glyph in WorldObjects.placedGlyphs)
            {
                if (glyph.GetEffectCircle().Contains(Position))
                {
                    if(!isGlyphAffected)
                        lastGlyphEffect = Orbs.currentTimeSpan;

                    isGlyphAffected = true;
                    ApplyGlyphEffect(glyph);
                    return true;
                }
            }

            isGlyphAffected = false;
            return false;
        }

        private void ApplyGlyphEffect(Glyph appliedGlyph)
        {
            if (Orbs.currentTimeSpan.Subtract(lastGlyphEffect).Seconds >= SECONDSBETWEENGLYPHEFFECT)
            {
                lastGlyphEffect = Orbs.currentTimeSpan;

                if (appliedGlyph.Id == "rn08") //light orb
                {
                    GotHit((int)(MaxHealth * ((appliedGlyph.MinDmg + appliedGlyph.MaxDmg) / 2) / 100));

                    EffectManager.flashBolts.Add(
                            new FlashBolt(appliedGlyph.worldPosition, new Vector2(Position.X + BoundingBox.Width / 2, Position.Y + BoundingBox.Height / 2)));
                    Orbs.soundEffects.PlaySound(@"Effects\zap_electro", 1.0f);
                }
                else if (appliedGlyph.Id == "rn06") //water orb
                {
                    EffectManager.flashBolts.Add(
                            new FlashBolt(appliedGlyph.worldPosition, new Vector2(Position.X + BoundingBox.Width / 2, Position.Y + BoundingBox.Height / 2)));
                    Orbs.soundEffects.PlaySound(@"Effects\zap_electro", 1.0f);
                }
            }
        }

        /// <summary>
        /// Bleeding when below 20% health
        /// </summary>
        private void ApplyInjuryDamage()
        {
            if (Orbs.currentTimeSpan.Subtract(lastInjuredBleed).Seconds >= SECONDSBETWEENBLEED)
            {
                lastInjuredBleed = Orbs.currentTimeSpan;
                EffectManager.CreateGroundEffect(Name + "bleed", "bleeding", 5, Position + new Vector2(16, 16));
                GotHit((int)(MaxHealth * INJURYDAMAGE));
            }
        }

        public void ApplyDebuff(Weapon.weaponClass weaponClass)
        {
            switch(weaponClass)
            {
                case Weapon.weaponClass.bleed:
                    namePlate.AddDebuff("bleed", Localization.strings_buffs.debuff_bleeding);
                    break;

                case Weapon.weaponClass.maim:
                    namePlate.AddDebuff("maim", Localization.strings_buffs.debuff_maimed);
                    break;

                case Weapon.weaponClass.blind:
                    namePlate.AddDebuff("blind", Localization.strings_buffs.debuff_blinded);
                    break;
            }
        }

        private void ResetEnemy()
        {
            isRetreating = false;
     
            if(isBleeding)
            {
                isBleeding = false;

                for (int i = 0; i <= namePlate.debuffs.Count() - 1; i++)
                {
                    if (namePlate.debuffs[i] != null)
                        if (namePlate.debuffs[i].debuffId == "bleed")
                            namePlate.debuffs[i] = null;
                }
            }

            movementspeed = DEFAULTMOVEMENTSPEED;
            CurrentAnimation = "Idle";
            targetPosition = waypoints[0].GetCoords();
            currentPathStep = 0;
            attackPath = null;
        }

        private void CheckAttackRange()
        {
            if (aggressionArea.Intersects(Player.PlayerPositionRectangle) && !isRetreating && !isAttacking)
                StartAttackPlayer();

            if(Player.playerDecoy != null)
                if (aggressionArea.Intersects(Player.playerDecoy.BoundingBox) && !isRetreating && !isAttacking)
                    StartAttackPlayerDecoy();
        }

        private void StartAttackPlayer()
        {
            isAttacking = true;
            movementspeed = DEFAULTMOVEMENTSPEED;
            UpdateAttackPath();

            UIManager.enemyNameplates.Add(namePlate);
        }

        private void StartAttackPlayerDecoy()
        {
            isAttacking = true;
            isAttackingDecoy = true;
            movementspeed = DEFAULTMOVEMENTSPEED;
            UpdateAttackPath();

            UIManager.enemyNameplates.Add(namePlate);
        }

        public void KnockBack(Vector2 v)
        {
            Position += v;
        }

        /// <summary>
        /// Creates and update attack path
        /// </summary>
        private void UpdateAttackPath()
        {
            if(attackPath_from == Point.Zero)
                attackPath_from = new Point((int)Position.X / 64, (int)Position.Y / 64);

            attackPath_to = GetTargetPoint();

            if(attackPath == null || attackPath_to != attackPath[attackPath.Count - 1] ) //path net yet calculated or player moved
            {
                currentPathStep = 0;
                attackPath_from = new Point((int)Position.X / 64, (int)Position.Y / 64);
                attackPath = Pathfinding.Pathfinding.FindPath(Tiled.TileMap.pathfindGrid, attackPath_from, attackPath_to);
            }

            if(attackPath.Count == currentPathStep) //reached last path point
            {
                if (attackPath.Count < 1)
                {

                    if (attackPath.Count == 0)
                        attackPath = Pathfinding.Pathfinding.FindPath(Tiled.TileMap.pathfindGrid, attackPath_from, attackPath_to);
                    attackPath_from = attackPath[0];
                }
                else
                    attackPath_from = attackPath[attackPath.Count - 1];

                currentPathStep = 0;
                attackPath = Pathfinding.Pathfinding.FindPath(Tiled.TileMap.pathfindGrid, attackPath_from, attackPath_to);
            }

            if (attackPath.Count < 1)
                return;

            targetPosition = new Vector2(attackPath[currentPathStep].X * 64, attackPath[currentPathStep].Y * 64);
        }

        private Point GetTargetPoint()
        {
            if (Player.playerDecoy != null && !isAttackingDecoy)
                if (Vector2.Distance(Position, Player.PlayerPosition) > Vector2.Distance(Position, Player.playerDecoy.Position))
                    AttackDecoy();

            if (isAttackingDecoy && Player.playerDecoy != null)
                return new Point(
                ((int)Player.playerDecoy.Position.X + Player.playerDecoy.BoundingBox.Width / 2) / 64,
                ((int)Player.playerDecoy.Position.Y + Player.playerDecoy.BoundingBox.Height / 2) / 64
                );

            return new Point(
                (Player.PlayerPositionRectangle.X + Player.PlayerPositionRectangle.Width / 2) / 64,
                (Player.PlayerPositionRectangle.Y + Player.PlayerPositionRectangle.Height / 2)/ 64
                );
        }

        /// <summary>
        /// Gets the next path point from current attack path
        /// </summary>
        /// <returns>Point of next targetposition</returns>
        private Point GetNextPathPoint()
        {
            if (isRetreating && currentPathStep == attackPath.Count - 1)
                return attackPath[currentPathStep];

            if (Vector2.Distance(Position, targetPosition) > 2)
                return attackPath[currentPathStep];

            return attackPath[currentPathStep++];
        }

        private void AttackPlayer()
        {
            UpdateAttackPath();

            Rectangle attackRange = new Rectangle(
                CollisionBox.X - ATTACK_RANGE_OFFSET, CollisionBox.Y - ATTACK_RANGE_OFFSET, 
                CollisionBox.Width + 2 * ATTACK_RANGE_OFFSET, CollisionBox.Height + 2 * ATTACK_RANGE_OFFSET);

            //follow player
            if (!attackRange.Intersects(Player.PlayerPositionRectangle))
            {
                Point targetPoint = GetNextPathPoint();
                targetPosition = new Vector2(targetPoint.X * 64, targetPoint.Y * 64);
            }

            //enemy kited too far away
            if (Vector2.Distance(Position, waypoints[currentWaypoint].GetCoords()) > RETREATRANGE )
                Retreat();

            if (attackRange.Intersects(Player.PlayerPositionRectangle))
            {
                secondsSinceLastAttack = Orbs.currentTimeSpan.Subtract(lastAttackAt).Seconds;

                if (secondsSinceLastAttack >= attackSpeed)
                {
                    attack = new EnemyAttack(this, attackAnimation);
                    lastAttackAt = Orbs.currentTimeSpan;
                }
            }
        }

        private void AttackDecoy()
        {
            isAttackingDecoy = true;

            UpdateAttackPath();

            Rectangle attackRange = new Rectangle(
                CollisionBox.X - ATTACK_RANGE_OFFSET, CollisionBox.Y - ATTACK_RANGE_OFFSET,
                CollisionBox.Width + 2 * ATTACK_RANGE_OFFSET, CollisionBox.Height + 2 * ATTACK_RANGE_OFFSET);

            //follow decoy
            if (!attackRange.Intersects(Player.playerDecoy.CollisionBox))
            {
                Point targetPoint = GetNextPathPoint();
                targetPosition = new Vector2(targetPoint.X * 64, targetPoint.Y * 64);
            }

            //enemy kited too far away
            if (Vector2.Distance(Position, waypoints[currentWaypoint].GetCoords()) > RETREATRANGE)
                Retreat();

            if (attackRange.Intersects(Player.playerDecoy.CollisionBox))
            {
                secondsSinceLastAttack = Orbs.currentTimeSpan.Subtract(lastAttackAt).Seconds;

                if (secondsSinceLastAttack >= attackSpeed)
                {
                    attack = new EnemyAttack(this, attackAnimation);
                    lastAttackAt = Orbs.currentTimeSpan;
                }
            }
        }

        private void Retreat()
        {
            movementspeed = RETREATSPEED;
            isAttacking = false;
            isRetreating = true;
            isAttackingDecoy = false;

            currentPathStep = 0;
            Point targetPosition = new Point((int)waypoints[currentWaypoint].GetCoords().X / 64, (int)waypoints[currentWaypoint].GetCoords().Y / 64);
            Point evadePosition = new Point((int)Position.X / 64, (int)Position.Y / 64);

            attackPath = Pathfinding.Pathfinding.FindPath(Tiled.TileMap.pathfindGrid, evadePosition, targetPosition);

            UIManager.enemyNameplates.Remove(namePlate);
        }

        private void UpdateAggression()
        {
            aggressionArea.X = (int)Position.X - AGGRESSIONRANGE;
            aggressionArea.Y = (int)Position.Y - AGGRESSIONRANGE;
            aggressionArea.Width = Width + 2 * AGGRESSIONRANGE;
            aggressionArea.Height = Height + 2 * AGGRESSIONRANGE;

            if (!isRetreating && aggressionArea.Intersects(Player.PlayerPositionRectangle))
                isHostile = true;
        }

        public string GetOrientation()
        {
            return CurrentAnimation.Split('_')[1];
        }

        public string GetDirectionByMovement(Vector2 movement)
        {
            Vector2 m = Vector2.Normalize(movement);

            if (movement.X > 0.1)
                return "Walk_East";

            if (movement.X < -0.1)
                return "Walk_West";

            if (movement.Y > 0.1)
                return "Walk_South";

            if (movement.Y < -0.1)
                return "Walk_North";

            return "";
        }

        new public void MoveBy(Vector2 movement)
        {
            lastSpritePosition = spritePosition;
            Vector2 normalizedMove = Vector2.Normalize(movement);

            spritePosition += normalizedMove * movementspeed;
            interactRectangle.X += (int)normalizedMove.X * movementspeed;
            interactRectangle.Y += (int)normalizedMove.Y * movementspeed;
        }

        private bool NPCCollidesWithPlayer(Vector2 move)
        {
            Rectangle targetRect = new Rectangle(CollisionBox.X + (int)move.X, CollisionBox.Y + (int)move.Y, CollisionBox.Width, CollisionBox.Height);
            return targetRect.Intersects(Player.PlayerPositionRectangle);
        }

        public void GotHit(int damageTaken)
        {
            Interface.UIManager.ShowCombatText(
                    "hit",
                    damageTaken.ToString(),
                    Position);

            CurHealth -= damageTaken;

            if (!IsAlive)
                EnemyKilled();
        }

        private void EnemyKilled()
        {
            Interface.UIManager.enemyNameplates.Remove(namePlate);

            WorldObjects.enemyDrops.AddRange(GetEnemyDrop());
            WorldCharacters.enemyWorldCharacters.Remove(this);

            Player.currentAttackCombo = 0;
        }

        private List<EnemyDrop> GetEnemyDrop()
        {
            List<Item> droppedItems = new List<Item>();
            List<EnemyDrop> returnValue = new List<EnemyDrop>();

            //Is Enemy Quest relevant? Returns string.Empty if no
            string[] droppedItemIDs = QuestSystem.QuestManager.HandleEnemyKill(enemyID);

            if (droppedItemIDs[0] != string.Empty)
            {
                foreach(string id in droppedItemIDs)
                {
                    droppedItems.Add(LootTable.questItems.Where(item => item.Id == id).First());
                }
            }
            else
            {
                droppedItems.Add(LootTable.miscItems.Where(item => item.Id == "misc01").First());
            }

            int xOffset = 0;
            int yOffset = 0;
            foreach(Item item in droppedItems)
            {
                returnValue.Add(new EnemyDrop(item, new Vector2(Position.X + xOffset, Position.Y + yOffset)));
                xOffset += 16;
                yOffset += 16;
            }

            return returnValue;
        }

        [Obsolete]
        public void DrawStatus(SpriteBatch spriteBatch)
        {
            if (isHovered || CurHealth < MaxHealth || isAttacking)
            {
                Vector2 displayTextSize = Orbs.spriteFontDefault.MeasureString(Name);
                float xOffset = (Texture.Width / 8 - displayTextSize.X) / 2;
                spriteBatch.Draw(Interface.UIManager.charLabelBackGround,
                    new Rectangle((int)Position.X + (int)xOffset, (int)Position.Y - 22, (int)displayTextSize.X, (int)displayTextSize.Y), Color.White);
                spriteBatch.DrawString(Orbs.spriteFontDefault, Name, new Vector2(Position.X + xOffset, Position.Y - 24), Color.White);

                //float percent = (float)curHealth / MaxHealth;
                //spriteBatch.Draw(healthbar_red,
                //    new Rectangle((int)Position.X + (int)xOffset, (int)Position.Y - 2, (int)displayTextSize.X, 4), Color.White);
                //spriteBatch.Draw(healthbar_green,
                //    new Rectangle((int)Position.X + (int)xOffset, (int)Position.Y - 2, (int)(displayTextSize.X * percent), 4), Color.White);
            }


            //Rectangle of Enemy aggressionarea, for debug
            //int bw = 2;
            //spriteBatch.Draw(Game1.pixel, new Rectangle(aggressionArea.Left, aggressionArea.Top, bw, aggressionArea.Height), Color.Black); // Left
            //spriteBatch.Draw(Game1.pixel, new Rectangle(aggressionArea.Right, aggressionArea.Top, bw, aggressionArea.Height), Color.Black); // Right
            //spriteBatch.Draw(Game1.pixel, new Rectangle(aggressionArea.Left, aggressionArea.Top, aggressionArea.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Game1.pixel, new Rectangle(aggressionArea.Left, aggressionArea.Bottom, aggressionArea.Width, bw), Color.Black); // Bottom
        }

        private new void Update(GameTime gameTime)
        {
            UpdateAggression();

            if (isHostile)
                CheckAttackRange();

            if (isAttacking)
            {
                if (Player.playerDecoy == null)
                    AttackPlayer();
                else
                {
                    if (Vector2.Distance(Position, Player.PlayerPosition) > Vector2.Distance(Position, Player.playerDecoy.Position))
                        AttackDecoy();
                    else
                        AttackPlayer();
                }
            }

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (attack == null)
            {
                if (IsVisible())
                    base.Draw(spriteBatch, tintColor);
            }
            else
            {
                if (attackAnimation != null)
                {
                    if (attack.attackInProgress)
                        attack.Draw(spriteBatch);
                    else
                        base.Draw(spriteBatch, tintColor);
                }
                else
                {
                    base.Draw(spriteBatch, tintColor);
                    if (attack.attackInProgress)
                        attack.Draw(spriteBatch);
                    else
                        base.Draw(spriteBatch, tintColor);
                }
            }

            //For Debug. Shows each point of an existing attack path
            //if (attackPath != null)
            //    foreach(Point p in attackPath)
            //    spriteBatch.DrawString(Game1.spriteFontDefault, "o", new Vector2(p.X * 64, p.Y * 64), Color.Red);
        }

        /// <summary>
        /// Checks if the enemy is withing camera bounds
        /// </summary>
        /// <returns></returns>
        private bool IsVisible()
        {
            if (Position.X < Orbs.camera.Position.X - 50 || Position.Y < Orbs.camera.Position.Y - 50 ||
                    Position.X > Orbs.camera.Position.X + Orbs.camera.viewWidth + 50 || Position.Y > Orbs.camera.Position.Y + Orbs.camera.viewHeight + 50)
                return false;

            return true;
        }
    }
}
