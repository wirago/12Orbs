using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Orbs.Animations
{
    class SpellAnimation
    {
        private Texture2D spriteTexture;

        private Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        private Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;
        private int projectileSpeed = 500;
        private Vector2 projectileDirection = Vector2.Zero;

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        public SpellAnimation(Texture2D texture)
        {
            spriteTexture = texture;

            //EXAMPLE!
            AddAnimation("fireball_east", 0, 64 * 4, 64, 64, 8, 0.1f);
            AddAnimation("fireball_west", 0, 64 * 0, 64, 64, 8, 0.1f);
            AddAnimation("fireball_north", 0, 64 * 2, 64, 64, 4, 0.1f);
            AddAnimation("fireball_south", 0, 64 * 6, 64, 64, 4, 0.1f);

            currentAnimation = "fireball_" + Player.curDirection.ToLower();

            switch(Player.curDirection.ToLower())
            {
                case "east":
                    projectileDirection = new Vector2(1, 0);
                    break;
                case "west":
                    projectileDirection = new Vector2(-1, 0);
                    break;
                case "north":
                    projectileDirection = new Vector2(0, -1);
                    break;
                case "south":
                    projectileDirection = new Vector2(0, 1);
                    break;
            }

            spritePosition = new Vector2(Player.PlayerPositionRectangle.X, Player.PlayerPositionRectangle.Y);
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
                    currentAnimation = value;
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


        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += (projectileDirection * projectileSpeed * elapsedTime);

            if (frameAnimations[currentAnimation].CurrentFrame >= 7)
                frameAnimations[currentAnimation].CurrentFrame = 0;

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
            //if (Player.isAttacking)
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
