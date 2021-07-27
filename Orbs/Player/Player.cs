using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

using Orbs.Animations;
using Orbs.LootSystem;
using Orbs.Tiled;
using Orbs.NPC;
using Orbs.World;
using Orbs.Combat;
using Orbs.Interface;
using System.Linq;
using System;

namespace Orbs
{
    public enum PlayerDirection {N, NE, E, SE, S, SW, W, NW};

    public class Player : SpriteAnimation
    {
        protected static Dictionary<string, List<QuestSystem.Quest>> chapterProgression = new Dictionary<string, List<QuestSystem.Quest>>()
        {
            { "wuranforest" , new List<QuestSystem.Quest>() },
            { "hollowrock" , new List<QuestSystem.Quest>() },
            { "pub" , new List<QuestSystem.Quest>() }
        };

        public const int DEFAULT_MOVEMENTSPEED = 5;
        private const int INTERACTION_OFFSET = 20;
        public static int MAX_HEALTH = 100;
        public static int MAX_MANA = 100;
        private const int FRAMEHEIGHT = 96; 
        private const int FRAMEWIDTH = 96;

        public static int movementspeed = 5; //Default = 5
        public static Rectangle PlayerPositionRectangle { get; set; }
        public static Vector2 PlayerPosition = Vector2.Zero;
        public static Rectangle PlayerInteractionRectangle { get; set; }
        public static bool notificationUp = false;
        public static int notificationResponse = -1;
        public static bool isAttacking = false;
        public static bool isCasting = false;
        public static bool isAlive = true;
        public static string curDirection = "";
        public static int health = 100;
        public static int mana = 100;
        public static Item selectedOrb = null;
        public static Weapon selectedWeapon = null;
        public static Vector2 respawnAt = Vector2.Zero;
        public static Buff currentBuff = null;
        public static Glyph currentGlyph = null;
        public static TimeSpan glypActivatedOn;
        public static bool glyphReady = true;
        public static int currentAttackCombo = 0;
        public static int combatBalance = 0;

        private Attack attack = null;
        private List<SpellAttack> castedSpells = new List<SpellAttack>();
        private int minDmg = 0;
        private int maxDmg = 0;
        //private TimeSpan lastDamageTakenAt;
        //private int minTimeBetweenHits = 1; //in seconds
        private int manaRegPerSecond = 2;
        private TimeSpan lastManaRegenaration;

        public static int money = 999;
        public static List<Item> inventory = new List<Item>();
        public static List<Spell> learnedSpells = new List<Spell>();
        public static int maxInventorySize = 12;
        public static Dictionary<int, Item> inventoryGrid = new Dictionary<int, Item>();
        public static Dictionary<int, Item> claimedOrbs = new Dictionary<int, Item>();
        public static string itemToBeApplied = "";

        private SoundEffect soundEffect_walk;
        private SoundEffectInstance soundEffectInstance;

        public static PlayerDecoy playerDecoy = null;
        public static PlayerPet playerPet;

        //public Texture2D deadPlayer;

        SpineObject playerSpine;
        PlayerDirection playerDirection = PlayerDirection.S;

        public Player(Texture2D texture, Vector2 spawnPosition) : base(texture)
        {
            Position = spawnPosition;
            isAnimating = true;

            AddAnimation("Walk_South", 0, FRAMEHEIGHT * 10, FRAMEWIDTH, FRAMEHEIGHT, 9, 0.04f);
            AddAnimation("Walk_West", 0, FRAMEHEIGHT * 9, FRAMEWIDTH, FRAMEHEIGHT, 9, 0.04f);
            AddAnimation("Walk_East", 0, FRAMEHEIGHT * 11, FRAMEWIDTH, FRAMEHEIGHT, 9, 0.04f);
            AddAnimation("Walk_North", 0, FRAMEHEIGHT * 8, FRAMEWIDTH, FRAMEHEIGHT, 9, 0.04f);

            AddAnimation("Idle_South", 0, FRAMEHEIGHT * 2, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_West", 0, FRAMEHEIGHT * 1, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_East", 0, FRAMEHEIGHT * 3, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);
            AddAnimation("Idle_North", 0, FRAMEHEIGHT * 0, FRAMEWIDTH, FRAMEHEIGHT, 2, 3f);


            CurrentAnimation = "Idle_South";

            soundEffect_walk = Orbs.content.Load<SoundEffect>("Sounds/step_cloth1");
            soundEffectInstance = soundEffect_walk.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Volume = .1f;

            PlayerInteractionRectangle = new Rectangle(
                PlayerPositionRectangle.X - 20,
                PlayerPositionRectangle.Y - 20,
                PlayerPositionRectangle.Width + 40,
                PlayerPositionRectangle.Height + 40);

            playerSpine = new SpineObject("Characters/player_male/skeleton", spawnPosition);
            playerPet = new PlayerPet(Position);
        }

        /// <summary>
        /// Static method to get players' game progression in the given chapter
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>List of quests currently active or completed quest</returns>
        public static List<QuestSystem.Quest> GetChapterProgression(string chapter)
        {
            if (chapterProgression.ContainsKey(chapter))
                return chapterProgression[chapter];
            else
                return new List<QuestSystem.Quest>() { };
        }

        /// <summary>
        /// Static method to add an entry to players' game progression.
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="id"></param>
        public static void SetChapterProgression(string chapter, string id)
        {
            if(chapterProgression.ContainsKey(chapter))
                if(!chapterProgression[chapter].Any(q => q.id == id))
                    chapterProgression[chapter].Add(new QuestSystem.Quest(id));
        }

        /// <summary>
        /// Returns true or false if the player is or was on the given quest in given chapter
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="id"></param>
        /// <returns>True|False</returns>
        public static bool IsOnQuest(string chapter, string id)
        {
            return chapterProgression[chapter].Any(q => q.id == id);
        }

        /// <summary>
        /// Returns true or false if the player has completed given quest in given chapter
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="id"></param>
        /// <returns>True || False</returns>
        public static bool HasQuestCompleted(string chapter, string id)
        {
            if (chapterProgression[chapter].Any(q => q.id == id))
                return chapterProgression[chapter].Where(q => q.id == id).First().isCompleted;

            return false;
        }

        /// <summary>
        /// Completes given quest in qiven chapter.
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="id"></param>
        public static void CompleteQuest(string chapter, string id)
        {
            if(chapterProgression.ContainsKey(chapter))
                chapterProgression[chapter].Where(q => q.id == id).First().CompleteQuest();
        }

        public void LoadEmptyInventoryGrid() //needs to be called on new game
        {
            Player.inventory.Clear();

            for (int i = 0; i < 12; i++)
                inventoryGrid[i] = null;

            for (int i = 0; i < 12; i++)
                claimedOrbs[i] = null;
        }

        public void LoadInventoryItems()
        {
            //Default Items
            selectedWeapon = LootTable.weapons.Where(item => item.Id == "w01").First();

            UIManager.charWindow.AddItemToInventory(selectedWeapon);
            minDmg = selectedWeapon.MinDmg;
            maxDmg = selectedWeapon.MaxDmg;

            UIManager.charWindow.AddItemToInventory(LootTable.consumables.Where(item => item.Id == "c05").First()); //manapot
            UIManager.charWindow.AddItemToInventory(LootTable.consumables.Where(item => item.Id == "c06").First()); //healthpot

            UIManager.charWindow.AddItemToInventory(LootTable.weapons.Where(item => item.Id == "w02").First()); //spear

            LearnSpell("sf01");
            LearnSpell("sa01");
        }

        public static void LearnSpell(string spellId)
        {
            Player.learnedSpells.Add(LootTable.spells.Where(s => s.Id == spellId).First());
        }

        /// <summary>
        /// Adds item to players invntory if not yet in inventory
        /// </summary>
        /// <param name="id">ID of the item to add</param>
        public static void AddItemToInventory(string id)
        {
            UIManager.charWindow.AddItemToInventory(id);
        }

        /// <summary>
        /// Removes item from players invntory
        /// </summary>
        /// <param name="id">ID of the item to remove</param>
        public static void RemoveItemFromInventory(string itemId)
        {
            UIManager.charWindow.RemoveItemFromInventory(itemId);
        }

        /// <summary>
        /// Calculates the actual movement that can be processed
        /// </summary>
        /// <param name="movement"></param>
        /// <returns>processable movement as Vector2</returns>
        private Vector2 CalculatePlayerMovement(Vector2 movement)
        {
            if (isAttacking || isCasting)
                return new Vector2(0, 0);

            Rectangle destRect = new Rectangle(CollisionBox.X + (int)movement.X, CollisionBox.Y + (int)movement.Y, CollisionBox.Width, CollisionBox.Height); 
            Vector2 returnValue = movement;

            //Check Worldbounds
            if (destRect.X < 0 || destRect.X > Orbs.curMap.worldWidth + FRAMEWIDTH)
                returnValue.X = 0;
            if (destRect.Y < 0 || destRect.Y > Orbs.curMap.worldHeight - FRAMEHEIGHT)
                returnValue.Y = 0;

            //Check collision with objects
            foreach (Rectangle co in TileMap.collisionObjects)
                returnValue = GetReturnVector(destRect, returnValue, co);


            //Check collision with friendly NPCs
            foreach (StandardNPC npc in WorldCharacters.standardWorldCharacters)
                returnValue = GetReturnVector(destRect, returnValue, npc.CollisionBox);

            //Check collision with enemies
            foreach (EnemyNPC enpc in WorldCharacters.enemyWorldCharacters)
                returnValue = GetReturnVector(destRect, returnValue, enpc.CollisionBox);

            //Check map change
            foreach (Transit t in WorldObjects.transits)
                if (destRect.Intersects(t.positionRectangle))
                {
                    t.enterTarget();
                    return Vector2.Zero;
                }
            return returnValue;
        }

        private bool CollidesX(Rectangle destRect, Rectangle collisionRect)
        {
            //(a.left < b.right) && (a.right > b.left)
            return (destRect.X < collisionRect.X + collisionRect.Width) && (destRect.X + destRect.Width > collisionRect.X);
        }

        private bool CollidesY(Rectangle destRect, Rectangle collisionRect)
        {
            //(a.top < b.bottom) && (a.bottom > b.top)
            return (destRect.Y < collisionRect.Y + collisionRect.Height) && (destRect.Y + destRect.Height > collisionRect.Y);
        }

        private Vector2 GetReturnVector(Rectangle destRect, Vector2 movement, Rectangle collisionObject)
        {
            if (destRect.Intersects(collisionObject))
            {
                if (CollidesX(destRect, collisionObject))
                    movement.X = 0;

                Rectangle newDestRect = new Rectangle(CollisionBox.X + (int)movement.X, CollisionBox.Y + (int)movement.Y, Width, Height); // Variable

                if (CollidesY(newDestRect, collisionObject))
                    movement.Y = 0;
            }
            return movement;
        }

        /// <summary>
        /// Equips selected weapon by weaponId
        /// </summary>
        /// <param name="weaponId"></param>
        public static void EquipWeapon(string weaponId)
        {
            if (!LootTable.weapons.Any(item => item.Id == weaponId))
                return;

            selectedWeapon = LootTable.weapons.Where(item => item.Id == weaponId).First();

            if (UIManager.actionBar.ContainsKey("actionbar_0"))
                UIManager.BindItemToActionbar(selectedWeapon, "actionbar_0");
            else
                UIManager.actionBar.Add("actionbar_0", LootTable.weapons.Where(w => w.Id == weaponId).First());

            currentAttackCombo = 0;
        }
        public static void EquipSpell(string spellId)
        {
            Spell selectedSpell = LootTable.spells.Where(spell => spell.Id == spellId).First();
            UIManager.BindItemToActionbar(selectedSpell, "actionbar_1");

            currentAttackCombo = 0;
        }

        public void StartAttack(string actionToBeTriggered)
        {
            if (isAttacking || isCasting)
                return;

            //TODO remove after spine implementation
            CurrentAnimation = "Idle_" + Helper.Triangle.GetAndSetPlayerOrientationByClickedSector();

            //UIManager.ShowAlert("compass", "direction: " + Helper.Compass.GetAndSetPlayerOrientationByClickedSector());

            isAttacking = true;
            attack = new Attack(UIManager.actionBar["actionbar_0"].Id, minDmg, maxDmg);
        }

        public static void StopAttack()
        {
            isAttacking = false;
            Attack.attackInProgress = false;
        }

        public static void StopCast()
        {
            isCasting = false;
        }

        public void ConsumeItem(string itemId)
        {
            UIManager.charWindow.RemoveItemFromInventory(itemId);
            Item usedItem = LootTable.consumables.Where(i => i.Id == itemId).First();
            
            if(usedItem.UseEffect.StartsWith("health"))
            {
                int healAmount = int.Parse(usedItem.UseEffect.Substring(6, usedItem.UseEffect.Length - 6));
                HealPlayer(healAmount);
            }
            else if (usedItem.UseEffect.StartsWith("mana"))
            {
                //MANA
            }
        }

        public void CastSpell(string spellId)
        {
            if (isCasting || isAttacking)
                return;

            //TODO remove after spine implementation
            CurrentAnimation = "Idle_" + Helper.Triangle.GetAndSetPlayerOrientationByClickedSector();

            //UIManager.ShowAlert("compass", "direction: " + Helper.Compass.GetAndSetPlayerOrientationByClickedSector());
            

            Spell castedSpell = LootTable.spells.Where(spell => spell.Id == spellId).First();

            if(mana - castedSpell.Manacost < 0)
            {
                UIManager.ShowAlert("notEnoughMana", Localization.strings.system_notEnoughMana);
                return;
            }
            mana -= castedSpell.Manacost;
            isCasting = true;

            if (castedSpell.UseEffect == "buff")
            {
                if (currentBuff != null)
                    currentBuff.DeactivateBuff();

                currentBuff = new Buff(
                    new Icon(castedSpell.Name.ToLower(), castedSpell.IconTexture, new Vector2(Orbs.graphics.PreferredBackBufferWidth - 69, 10)),
                    12,
                    castedSpell.Name.ToLower(),
                    castedSpell.SoundEffect
                    );
            }
            else if (castedSpell.UseEffect == "damage")
            {
                castedSpells.Add(new SpellAttack(castedSpell));
            }
            else
                UIManager.ShowAlert("cast spell", "Casted Spell " + LootTable.spells.Where(spell => spell.Id == spellId).First().Name);
        }

        public void CastGlyph(string glyphId)
        {
            if(!glyphReady)
            {
                UIManager.ShowAlert("glyphNotReady", "You can not cast another glyh now!");
                return;
            }

            Glyph temp = LootTable.glyphs.Where(g => g.Id == glyphId).First();

            currentGlyph = new Glyph
            {
                Id = temp.Id,
                Name = temp.Name,
                MinDmg = temp.MinDmg,
                MaxDmg = temp.MaxDmg,
                Manacost = temp.Manacost,
                IconTexture = temp.IconTexture,
                Type = temp.Type,
                UseEffect = temp.UseEffect,
                SoundEffect = temp.SoundEffect,
                GlyphTexture = temp.GlyphTexture,
                PlacedAt = Orbs.currentTimeSpan
            };
        }

        public static void PlaceGlyph()
        {
            glyphReady = false;
            glypActivatedOn = Orbs.currentTimeSpan;

            UIManager.ShowAlert("glyp", "Placed Glyph " + currentGlyph.Name);

            currentGlyph.isPlaced = true;
            WorldObjects.placedGlyphs.Add(Player.currentGlyph);

            if (currentGlyph.Id == "rn04")
                SpawnDecoy(currentGlyph.worldPosition);

            Orbs.soundEffects.PlaySound(currentGlyph.SoundEffect, 1.0f);
            currentGlyph = null;
        }

        private static void SpawnDecoy(Vector2 position)
        {
            playerDecoy = new PlayerDecoy(Orbs.content.Load<Texture2D>(@"Animations\Characters\player_male"), position);
        }

        public static void DespawnDecoy()
        {
            playerDecoy = null;

            if (WorldObjects.placedGlyphs.Any(x => x.Id == "rn04"))
                WorldObjects.placedGlyphs.RemoveAll(x => x.Id == "rn04");
        }

        public bool PlayerHit(Rectangle otherRect)
        {
            return CollisionBox.Intersects(otherRect);
        }

        public string GetOrientation()
        {
            return CurrentAnimation.Split('_')[1];
        }

        public string GetDirectionByMovement(Vector2 movement) //TODO remove when spine done
        {
            Vector2 m = Vector2.Normalize(movement);

            if (movement.X > 0)
                return "Walk_East";

            if (movement.X < 0)
                return "Walk_West";

            if (movement.Y > 0)
                return "Walk_South";

            if (movement.Y < 0)
                return "Walk_North";

            return Player.curDirection;
        }

        public PlayerDirection GetSpineDirectionByMovement(Vector2 movement)
        {
            if (movement.X == 0 && movement.Y < 0)
                return PlayerDirection.N;
            if (movement.X > 0 && movement.Y < 0)
                return PlayerDirection.NE;
            if (movement.X > 0 && movement.Y == 0)
                return PlayerDirection.E;
            if (movement.X > 0 && movement.Y > 0)
                return PlayerDirection.SE;
            if (movement.X == 0 && movement.Y > 0)
                return PlayerDirection.S;
            if (movement.X < 0 && movement.Y > 0)
                return PlayerDirection.SW;
            if (movement.X < 0 && movement.Y == 0)
                return PlayerDirection.W;
            if (movement.X < 0 && movement.Y < 0)
                return PlayerDirection.NW;

            return playerDirection;
        }

        private void SetSpineAnimation()
        {
            switch(playerDirection)
            {
                case PlayerDirection.N:
                    playerSpine.SetAnimation("top");
                    break;
                case PlayerDirection.NE:
                    playerSpine.SetAnimation("topright");
                    break;
                case PlayerDirection.E:
                    playerSpine.SetAnimation("right");
                    break;
                case PlayerDirection.SE:
                    playerSpine.SetAnimation("botright");
                    break;
                case PlayerDirection.S:
                    playerSpine.SetAnimation("bottom");
                    break;
                case PlayerDirection.SW:
                    playerSpine.SetAnimation("botleft");
                    break;
                case PlayerDirection.W:
                    playerSpine.SetAnimation("left");
                    break;
                case PlayerDirection.NW:
                    playerSpine.SetAnimation("topleft");
                    break;
            }
        }

        private void SetSpineIdle()
        {
            switch (playerDirection)
            {
                case PlayerDirection.N:
                    playerSpine.SetAnimation("idlet");
                    break;
                case PlayerDirection.NE:
                    playerSpine.SetAnimation("idletr");
                    break;
                case PlayerDirection.E:
                    playerSpine.SetAnimation("idler");
                    break;
                case PlayerDirection.SE:
                    playerSpine.SetAnimation("idlebr");
                    break;
                case PlayerDirection.S:
                    playerSpine.SetAnimation("idleb");
                    break;
                case PlayerDirection.SW:
                    playerSpine.SetAnimation("idlebl");
                    break;
                case PlayerDirection.W:
                    playerSpine.SetAnimation("idlel");
                    break;
                case PlayerDirection.NW:
                    playerSpine.SetAnimation("idletl");
                    break;
            }
        }

        new public void MoveBy(Vector2 movement)
        {
            lastSpritePosition = spritePosition;
            spritePosition += Vector2.Normalize(movement) * movementspeed;
            PlayerPositionRectangle = CollisionBox;

            PlayerInteractionRectangle = new Rectangle(
                PlayerPositionRectangle.X - INTERACTION_OFFSET, 
                PlayerPositionRectangle.Y - INTERACTION_OFFSET, 
                PlayerPositionRectangle.Width + INTERACTION_OFFSET * 2, 
                PlayerPositionRectangle.Height + INTERACTION_OFFSET * 2);
        }

        public void SpawnAt(Vector2 spawnLocation)
        {
            spritePosition = spawnLocation;
            Position = spawnLocation;
            PlayerPositionRectangle = CollisionBox;
        }

        public void DamagePlayer(int damage)
        {
            Player.health = Player.health - damage >= 0 ? Player.health - damage : 0;


            //int secondsSinceLastHit = Orbs.currentTimeSpan.Subtract(lastDamageTakenAt).Seconds;

            //if (secondsSinceLastHit >= minTimeBetweenHits && Player.health > 0)
            //{
            //    Player.health = Player.health - damage >= 0 ? Player.health - damage : 0;
            //    lastDamageTakenAt = Orbs.currentTimeSpan;
            //    UIManager.ShowCombatText("dmgToPlayer", damage.ToString(), Position);

            //    //Play random hurt soundeffect
            //    Orbs.soundEffects.PlaySound("player/male/hurt" + Orbs.rand.Next(1,7).ToString(), 1.0f);
            //}  
        }

        /// <summary>
        /// Heals the player by the given amount or sets him to MAX_HEALTH
        /// </summary>
        /// <param name="heal">Amount of heal</param>
        public void HealPlayer(int heal)
        {
            Player.health = (health + heal >= MAX_HEALTH) ? MAX_HEALTH : health + heal;

            //TODO play refreshing sound
        }

        public static void PlayerGotHit(int damage)
        {
            Player.health = Player.health - damage >= 0 ? Player.health - damage : 0;
            UIManager.ShowCombatText("dmgToPlayer", damage.ToString(), new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y));

            //Play random hurt soundeffect
            Orbs.soundEffects.PlaySound("player/male/hurt" + Orbs.rand.Next(1, 7).ToString(), 1.0f);
        }
       
        public void UpdatePlayer(GameTime gameTime, Vector2 playerMovement)
        {
            if(itemToBeApplied != "")
            {
                if (itemToBeApplied.StartsWith("c"))
                {
                    ConsumeItem(itemToBeApplied);
                    itemToBeApplied = "";
                }
            }

            if (castedSpells.Count > 0)
            {
                for (int i = 0; i < castedSpells.Count; i++)
                    if (!castedSpells[i].isActive)
                        castedSpells.RemoveAt(i);

                foreach (SpellAttack sa in castedSpells)
                    sa.Update(gameTime);
            }
            else
            {
                isCasting = false;
            }

            string curAnimation = "";
            curAnimation = GetDirectionByMovement(playerMovement * movementspeed); //TODO remove when spine done
            playerDirection = GetSpineDirectionByMovement(playerMovement * movementspeed);

            if (CurrentAnimation != curAnimation)
                CurrentAnimation = curAnimation;

            playerMovement = CalculatePlayerMovement(playerMovement * movementspeed);

            if (playerMovement.Length() != 0 && notificationUp == false)
            {
                MoveBy(Vector2.Normalize(playerMovement));
                soundEffectInstance.Play();
                SetSpineAnimation();
            }
            else
            {
                CurrentAnimation = "Idle_" + CurrentAnimation.Substring(5);
                SetSpineIdle();
                soundEffectInstance.Stop();
            }

            Player.curDirection = GetOrientation();
            Player.PlayerPositionRectangle = CollisionBox;
            Player.PlayerPosition.X = X;
            Player.PlayerPosition.Y = Y;

            if (isAttacking && attack != null)
               attack.Update(gameTime);

            if (!isAttacking && attack != null)
                attack = null;

            if (Player.health == 0)
            {
                //deadPlayer = Orbs.content.Load<Texture2D>(@"Textures\misc\gravestone");
                Orbs.musicManager.ChangeBackgroundMusic("Level1_Special");
                Orbs.curGameState = GameState.Dead;
                isAnimating = false;
                isAlive = false;
            }

            if(respawnAt != Vector2.Zero)
            {
                SpawnAt(respawnAt);
                respawnAt = Vector2.Zero;
            }

            if (currentBuff != null)
                currentBuff.Update(gameTime);

            int secondsSinceLastManaReg = Orbs.currentTimeSpan.Subtract(lastManaRegenaration).Seconds;

            if (secondsSinceLastManaReg >= 1 && Player.mana < MAX_MANA)
            {
                lastManaRegenaration = Orbs.currentTimeSpan;

                if (Player.mana + manaRegPerSecond >= MAX_MANA)
                    Player.mana = MAX_MANA;
                else
                    Player.mana += manaRegPerSecond;
            }

            if(playerDecoy != null)
                playerDecoy.Update();

            if (!glyphReady)
            {
                TimeSpan glyphCooldown = Orbs.currentTimeSpan - glypActivatedOn;

                if (glyphCooldown.Seconds >= 59)
                    glyphReady = true;
            }

            playerSpine.Update(gameTime);
            playerSpine.SetPosition(Position);
            playerPet.Update(gameTime);

            base.Update(gameTime);
        }

        public void DrawAttack(SpriteBatch spriteBatch)
        {
            if (isAttacking && attack != null)
                attack.Draw(spriteBatch);

            if (castedSpells.Count > 0)
                foreach (SpellAttack sa in castedSpells)
                    sa.Draw(spriteBatch);
        }

        public static bool HasItem(string id)
        {
            return inventory.Any(item => item.Id == id);
        }

        public static void ModifyCombatBalance(int value)
        {
            if(value < 0)
            {
                if (combatBalance - value < -130)
                    combatBalance = -130;
                else
                    combatBalance -= value * -1;
            }
            else
            {
                if (combatBalance + value > 130)
                    combatBalance = 130;
                else
                    combatBalance += value;
            }
        }

        /// <summary>
        /// Returns number of items with given item ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No. if items with given item ID or 0 if no items match the ID</returns>
        public static int CountItems(string id)
        {
            if (!inventory.Any(item => item.Id == id))
                return 0;

            return inventory.Count(item => item.Id == id);
        }

        public int GetMaxHealth()
        {
            return MAX_HEALTH;
        }

        public void DrawPlayer()
        {
            //TODO tedsm
            playerSpine.Draw();
        }
    }
}
