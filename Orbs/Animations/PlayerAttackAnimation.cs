using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Orbs.Animations
{
    public class PlayerAttackAnimation
    {
        private Texture2D spriteTexture;

        private Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        private Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;

        private Rectangle attackRectangle;
        private int attackRange = 30; // TODO get from weapon

        private const int ANIMATIONFRAMESCOUNT = 6;
        private const float ANIMATIONFRAMELENGTH = 0.1f;

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        public PlayerAttackAnimation(Texture2D texture)
        {
            spriteTexture = texture;

            //Player attack animation
            AddAnimation("swing_east", 0, 288 * 3, 288, 288, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("swing_west", 0, 288 * 1, 288, 288, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("swing_north", 0, 288 * 0, 288, 288, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("swing_south", 0, 288 * 2, 288, 288, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);

            //CurrentAnimation = "swing_east";
            CurrentAnimation = "swing_" + Player.curDirection.ToLower();

            if(CurrentAnimation == "swing_east")
                AttackRectangle = new Rectangle((int)Player.PlayerPosition.X + 64, (int)Player.PlayerPosition.Y + 32, attackRange, 64);
            if (CurrentAnimation == "swing_west")
                AttackRectangle = new Rectangle((int)Player.PlayerPosition.X - 10, (int)Player.PlayerPosition.Y + 32, attackRange, 64);
            if (CurrentAnimation == "swing_north")
                AttackRectangle = new Rectangle((int)Player.PlayerPosition.X + 16, (int)Player.PlayerPosition.Y, 64, attackRange);
            if (CurrentAnimation == "swing_south")
                AttackRectangle = new Rectangle((int)Player.PlayerPosition.X + 16, (int)Player.PlayerPosition.Y + 96, 64, attackRange);
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

        public Rectangle AttackRectangle
        {
            get => attackRectangle;
            private set => attackRectangle = value;
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


        public void Update(GameTime gameTime)
        {
            if (!Player.isAttacking)
                return;

            //CurrentAnimation = "swing_" + Player.curDirection.ToLower();

            X = Player.PlayerPositionRectangle.X - 115; //TODO needs to be calculated
            Y = Player.PlayerPositionRectangle.Y - 110;

            if (frameAnimations[currentAnimation].CurrentFrame >= ANIMATIONFRAMESCOUNT - 1)
            {
                Player.StopAttack();
                frameAnimations[currentAnimation].CurrentFrame = 0;
            }

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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Player.isAttacking)
                spriteBatch.Draw(spriteTexture, Position, CurrentFrameAnimation.FrameRectangle, tintColor);

            //For Debugging
            //int bw = 2; // Border width
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(AttackRectangle.Left, AttackRectangle.Top, bw, AttackRectangle.Height), Color.Black); // Left
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(AttackRectangle.Right, AttackRectangle.Top, bw, AttackRectangle.Height), Color.Black); // Right
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(AttackRectangle.Left, AttackRectangle.Top, AttackRectangle.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(AttackRectangle.Left, AttackRectangle.Bottom, AttackRectangle.Width, bw), Color.Black); // Bottom

        }
    }
}
