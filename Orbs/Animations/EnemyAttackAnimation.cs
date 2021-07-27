using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.Combat;
using Orbs.NPC;
using System.Collections.Generic;

namespace Orbs.Animations
{
    public class EnemyAttackAnimation
    {
        private Texture2D spriteTexture;

        private Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        private Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;

        private const int ATTACK_RANGE_OFFSET = 7;

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        public EnemyAttackAnimation(Texture2D texture, bool usePlaceholder)
        {
            spriteTexture = texture;

            if (usePlaceholder)
            {
                AddAnimation("swing_South", 0, 64 * 0, 64, 64, 4, 0.1f);
                AddAnimation("swing_West", 0, 64 * 1, 64, 64, 4, 0.1f);
                AddAnimation("swing_East", 0, 64 * 1, 64, 64, 4, 0.1f);
                AddAnimation("swing_North", 0, 64 * 1, 64, 64, 4, 0.1f);
                CurrentAnimation = "swing_East";
            }
            else
            {
                AddAnimation("swing_South", 0, 64 * 0, 64, 64, 8, 0.1f);
                AddAnimation("swing_West", 0, 64 * 1, 64, 64, 8, 0.1f);
                AddAnimation("swing_East", 0, 64 * 2, 64, 64, 8, 0.1f);
                AddAnimation("swing_North", 0, 64 * 3, 64, 64, 8, 0.1f);

                CurrentAnimation = "swing_South";
            }
        }

        public Vector2 Position
        {
            get { return spritePosition; }
            set
            {
                lastSpritePosition = spritePosition;
                spritePosition = value;
            }
        }

        public int X
        {
            get { return (int)spritePosition.X; }
            set
            {
                lastSpritePosition.X = spritePosition.X;
                spritePosition.X = value;
            }
        }

        public int Y
        {
            get { return (int)spritePosition.Y; }
            set
            {
                lastSpritePosition.Y = spritePosition.Y;
                spritePosition.Y = value;
            }
        }

        public int Width
        {
            get { return spriteWidth; }
        }

        public int Height
        {
            get { return spriteHeight; }
        }

        //Screen coordinates of the bounding box surrounding this sprite
        public Rectangle BoundingBox
        {
            get { return new Rectangle(X, Y, spriteWidth, spriteHeight); }
            set
            {
                X = value.X - ATTACK_RANGE_OFFSET;
                Y = value.Y - ATTACK_RANGE_OFFSET;
                spriteWidth = value.Width + 2 * ATTACK_RANGE_OFFSET;
                spriteHeight = value.Height + 2 * ATTACK_RANGE_OFFSET;
            }
        }

        public Texture2D Texture
        {
            get { return spriteTexture; }
        }

        //FrameAnimation object of the currently playing animation
        public FrameAnimation CurrentFrameAnimation
        {
            get
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return frameAnimations[currentAnimation];
                else
                    return null;
            }
        }

        public string CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                if (frameAnimations.ContainsKey(value))
                {
                    currentAnimation = value;
                    //frameAnimations[currentAnimation].CurrentFrame = 0;
                    //frameAnimations[currentAnimation].playCount = 0;
                }
            }
        }

        public void AddAnimation(string Name, int X, int Y, int Width, int Height, int Frames, float FrameLength)
        {
            frameAnimations.Add(Name, new FrameAnimation(X, Y, Width, Height, Frames, FrameLength));
            spriteWidth = Width;
            spriteHeight = Height;
            spriteCenter = new Vector2(spriteWidth / 2, spriteHeight / 2);
        }

        public void AddAnimation(string Name, int X, int Y, int Width, int Height, int Frames,
           float FrameLength, string NextAnimation)
        {
            frameAnimations.Add(Name, new FrameAnimation(X, Y, Width, Height, Frames, FrameLength, NextAnimation));
            spriteWidth = Width;
            spriteHeight = Height;
            spriteCenter = new Vector2(spriteWidth / 2, spriteHeight / 2);
        }

        public FrameAnimation GetAnimationByName(string Name)
        {
            if (frameAnimations.ContainsKey(Name))
                return frameAnimations[Name];
            else
                return null;
        }


        public void Update(GameTime gameTime, EnemyAttack enemyAttack)
        {
            if (!enemyAttack.attackInProgress)
                return;

            switch (enemyAttack.attackingEnemy.GetOrientation())
            {
                case "South":
                    CurrentAnimation = "swing_South";
                    X = (int)enemyAttack.attackingEnemy.Position.X;
                    Y = (int)enemyAttack.attackingEnemy.Position.Y + 16;
                    break;
                case "West":
                    CurrentAnimation = "swing_West";
                    X = (int)enemyAttack.attackingEnemy.Position.X - 16;
                    Y = (int)enemyAttack.attackingEnemy.Position.Y + 4;
                    break;
                case "East":
                    CurrentAnimation = "swing_East";
                    X = (int)enemyAttack.attackingEnemy.Position.X + 16;
                    Y = (int)enemyAttack.attackingEnemy.Position.Y + 4;
                    break;
                case "North":
                    CurrentAnimation = "swing_North";
                    X = (int)enemyAttack.attackingEnemy.Position.X;
                    Y = (int)enemyAttack.attackingEnemy.Position.Y - 16;
                    break;
            }


            if (frameAnimations[currentAnimation].CurrentFrame >= 3)
            {
                enemyAttack.StopAttackAnimation();
                frameAnimations[currentAnimation].CurrentFrame = 0;
            }

            if (enemyAttack.attackInProgress)
            {
                if (CurrentFrameAnimation == null)
                {
                    if (frameAnimations.Count > 0)
                    {
                        string[] sKeys = new string[frameAnimations.Count];
                        frameAnimations.Keys.CopyTo(sKeys, 0);
                        CurrentAnimation = sKeys[0];
                    }
                    else
                        return;
                }

                CurrentFrameAnimation.Update(gameTime);

                if (!string.IsNullOrEmpty(CurrentFrameAnimation.nextAnimation))
                    if (CurrentFrameAnimation.playCount > 0)
                        CurrentAnimation = CurrentFrameAnimation.nextAnimation;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool isAttacking)
        {
            if (isAttacking)
                spriteBatch.Draw(spriteTexture, spritePosition, CurrentFrameAnimation.FrameRectangle, tintColor);

            //For Debugging
            //int bw = 2; // Border width
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Left
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Right, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Right
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, BoundingBox.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Bottom, BoundingBox.Width, bw), Color.Black); // Bottom

        }
    }
}
