using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Orbs.World;
using Orbs.Animations;
using Orbs.QuestSystem;
using System.Collections.Generic;
using System.Linq;
using Orbs.Audio;

namespace Orbs.NPC
{
    public class StandardNPC : SpriteAnimation, INPC
    {
        private string curAnimation = "";

        public string Name { get; set; }
        private Vector2 nameSize;
        private Texture2D portrait;

        private int movementspeed = 2;
        private Rectangle interactRectangle;
        private int interactRange = 20;

        private int proximityRange = 200;
        public Rectangle proximityRectangle;

        private Vector2 targetPosition;
        List<Waypoint> waypoints = new List<Waypoint>(); //Hold all the Waypoints Ordered by NameID

        int currentWaypointID = 0;

        private bool sentToOtherPosition = false;
        private Vector2 NPCMovement = Vector2.Zero;
        private float distance = 0f;
        private readonly Vector2 initialSpawnPosition = Vector2.Zero;
        public string ID {get; private set; }
        public bool isHovered = false;

        private bool isMovingChar;
        public string offeredItems = "";
        
		public float nameplateRange = 300f;
        public string soundfile; 

        //Static NPC
        public StandardNPC(Texture2D texture, List<Waypoint> targetPositions, int height, int width, string name, string id, int rotation, string portraitSource, string offeredItems, string soundfile) : base(texture)
        {
            waypoints = targetPositions;
            Position = waypoints[0].GetCoords();
            isAnimating = true;
            isMovingChar = false;
            Name = name;
            nameSize = Orbs.spriteFontDefault.MeasureString(Name);
            this.ID = id;
            this.soundfile = soundfile;

            AddAnimation("Idle_South", 0, height * 2, width, height, 2, 3f);
            AddAnimation("Idle_West", 0, height * 1, width, height, 2, 3f);
            AddAnimation("Idle_East", 0, height * 3, width, height, 2, 3f);
            AddAnimation("Idle_North", 0, height * 0, width, height, 2, 3f);

            if (rotation == 90)
                CurrentAnimation = "Idle_West";
            else if (rotation == 180)
                CurrentAnimation = "Idle_North";
            else if (rotation == 270)
                CurrentAnimation = "Idle_East";
            else
                CurrentAnimation = "Idle_South";

            interactRectangle = new Rectangle(CollisionBox.X - interactRange, CollisionBox.Y - interactRange, CollisionBox.Width + interactRange * 2, CollisionBox.Height + interactRange * 2);
            proximityRectangle = new Rectangle(CollisionBox.X - proximityRange, CollisionBox.Y - proximityRange, CollisionBox.Width + proximityRange * 2, CollisionBox.Height + proximityRange * 2);

            if (portraitSource != "")
                portrait = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\" + portraitSource);

            this.offeredItems = offeredItems;

            WorldCharacters.standardWorldCharacters.Add(this);
        }

        //Moving NPC
        public StandardNPC(Texture2D texture, List<Waypoint> targetPositions, int height, int width, string name, string id, int rotation, string portraitSource, bool isMoving = true) : base(texture)
        {
            waypoints = targetPositions;
            Position = waypoints[0].GetCoords();
            this.ID = id;
            this.Name = name;
            nameSize = Orbs.spriteFontDefault.MeasureString(Name);
            targetPosition = waypoints[0].GetCoords();

            isAnimating = true;
            isMovingChar = true;
            #region animation


            AddAnimation("Walk_South", 0, height * 10, width, height, 9, 0.07f);
            AddAnimation("Walk_West", 0, height * 9, width, height, 9, 0.07f);
            AddAnimation("Walk_East", 0, height * 11, width, height, 9, 0.07f);
            AddAnimation("Walk_North", 0, height * 8, width, height, 9, 0.07f);

            AddAnimation("Idle_South", 0, height * 2, width, height, 2, 3f);
            AddAnimation("Idle_West", 0, height * 1, width, height, 2, 3f);
            AddAnimation("Idle_East", 0, height * 3, width, height, 2, 3f);
            AddAnimation("Idle_North", 0, height * 0, width, height, 2, 3f);

            #endregion

            if (rotation == 90)
                CurrentAnimation = "Idle_West";
            else if (rotation == 180)
                CurrentAnimation = "Idle_North";
            else if (rotation == 270)
                CurrentAnimation = "Idle_East";
            else
                CurrentAnimation = "Idle_South";

            if (portraitSource != "")
                portrait = Orbs.content.Load<Texture2D>(@"UserInterface\Artwork\" + portraitSource);

            interactRectangle = new Rectangle(CollisionBox.X - interactRange, CollisionBox.Y - interactRange, Width + interactRange * 2, Height + interactRange * 2);
            proximityRectangle = new Rectangle(CollisionBox.X - proximityRange, CollisionBox.Y - proximityRange, CollisionBox.Width + proximityRange * 2, CollisionBox.Height + proximityRange * 2);

            WorldCharacters.standardWorldCharacters.Add(this);
        }

        public void MoveNPCTo(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
            //waypoints.Clear();
            waypoints.Add(new Waypoint(targetPosition.X, targetPosition.Y));
            sentToOtherPosition = true;
        }

        public void UpdateNPC(GameTime gameTime)
        {
            CheckSoundProximity();

            //static NPC
            if (!isMovingChar && !sentToOtherPosition)
            {
                base.Update(gameTime);
                return;
            }

            curAnimation = "";

            if (Vector2.Distance(Position, targetPosition) <= 2)
            {
                if (!isMovingChar)
                {
                    sentToOtherPosition = false;
                    return;
                }
                currentWaypointID++;
                if (currentWaypointID >= waypoints.Count)
                    currentWaypointID = 0;
                targetPosition = waypoints[currentWaypointID].GetCoords();
            }

            if (Position != targetPosition && Vector2.Distance(Position, targetPosition) > 2)
            {
                distance = Vector2.Distance(Position, targetPosition);
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
                if (CurrentAnimation != "Idle")
                    CurrentAnimation = "Idle_" + CurrentAnimation.Substring(5);
            }

            

            base.Update(gameTime);
        }

        public void Interact(string npcName)
        {
            if (!Player.PlayerPositionRectangle.Intersects(interactRectangle))
                return;

            QuestManager.InteractWithNPC(npcName);
        }

        public void MakeNPCHostile()
        {
            //new EnemyNPC(Texture, waypoints, Height, Width, 2, ID, true, Name, null); //TODO fix null value
            new EnemyNPC(Texture, waypoints, true, LootSystem.EnemyTable.enemyTemplates.Where(x => x.uid == ID).First());
            WorldCharacters.standardWorldCharacters.Remove(this);
        }

        public string GetDirectionByMovement(Vector2 movement)
        {
            Vector2 m = Vector2.Normalize(movement);

            if (movement.X > 0.1f)
                return "Walk_East";

            if (movement.Y > 0.1f)
                return "Walk_South";

            if (movement.X < 0)
                return "Walk_West";

            if (movement.Y < 0)
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

            proximityRectangle.X += (int)normalizedMove.X * movementspeed;
            proximityRectangle.Y += (int)normalizedMove.Y * movementspeed;
        }

        private bool NPCCollidesWithPlayer(Vector2 move)
        {
            Rectangle targetRect = new Rectangle(CollisionBox.X + (int)move.X, CollisionBox.Y + (int)move.Y, CollisionBox.Width, CollisionBox.Height);
            return targetRect.Intersects(Player.PlayerPositionRectangle);
        }

        private void CheckSoundProximity()
        {
            if (Vector2.Distance(Position, Player.PlayerPosition) < 600 && soundfile != "")
            {
                Orbs.soundEffects.PlayProximitySound(soundfile, 1 - Vector2.Distance(Position, Player.PlayerPosition) / 600);
                Orbs.soundEffects.UpdateProximityVolume(soundfile, 1 - (Vector2.Distance(Position, Player.PlayerPosition) / 600));
            }
        }

        public void DrawName(SpriteBatch spriteBatch)
        {
            float tmp_distance = Vector2.Distance(
                new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y),
                Position);

            if (isHovered || tmp_distance <= nameplateRange)
            {
                Color statusTextColor = Color.DarkGreen;
                float xOffset = (96 - nameSize.X) / 2;

                spriteBatch.Draw(Interface.UIManager.charLabelBackGround, 
                    new Rectangle((int)Position.X + (int)xOffset, (int)Position.Y - 10, (int)nameSize.X, (int)nameSize.Y), Color.White);
                spriteBatch.DrawString(Orbs.spriteFontDefault, Name, new Vector2(Position.X + xOffset, Position.Y - 10), statusTextColor);
                isHovered = false;
            }
        }
    }
}
