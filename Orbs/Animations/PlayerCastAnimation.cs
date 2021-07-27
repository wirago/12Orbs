using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Orbs.Animations
{
    public class PlayerCastAnimation
    {
        private Texture2D spriteTexture;
        private const int FRAMEHEIGHT = 96;
        private const int FRAMEWIDTH = 96;
        private Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        private Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;

        private const int ANIMATIONFRAMESCOUNT = 7;
        private const float ANIMATIONFRAMELENGTH = 0.1f;

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        public PlayerCastAnimation()
        {
            spriteTexture = Orbs.content.Load<Texture2D>(@"Animations/Attack/player_male_cast");

            //Player cast animation
            AddAnimation("cast_east", 0, FRAMEHEIGHT * 3, FRAMEWIDTH, FRAMEHEIGHT, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("cast_west", 0, FRAMEHEIGHT * 1, FRAMEWIDTH, FRAMEHEIGHT, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("cast_north", 0, FRAMEHEIGHT * 0, FRAMEWIDTH, FRAMEHEIGHT, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);
            AddAnimation("cast_south", 0, FRAMEHEIGHT * 2, FRAMEWIDTH, FRAMEHEIGHT, ANIMATIONFRAMESCOUNT, ANIMATIONFRAMELENGTH);

            CurrentAnimation = "cast_east";
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
            if (!Player.isCasting)
                return;

            CurrentAnimation = "cast_" + Player.curDirection.ToLower();

            Position = Player.PlayerPosition;

            if (frameAnimations[currentAnimation].CurrentFrame >= ANIMATIONFRAMESCOUNT - 1)
            {
                Player.StopCast();
                frameAnimations[currentAnimation].CurrentFrame = 0;
                return;
            }

            if (Player.isCasting)
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Player.isCasting)
                spriteBatch.Draw(spriteTexture, Position, CurrentFrameAnimation.FrameRectangle, tintColor);

            //For Debugging
            //int bw = 2; // Border width
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Left
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Right, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Right
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, BoundingBox.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Bottom, BoundingBox.Width, bw), Color.Black); // Bottom

        }
    }
}
