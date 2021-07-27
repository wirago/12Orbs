using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbs.Animations
{
    public class SpriteAnimation
    {
        Texture2D spriteTexture;

        public bool isAnimating = true;
        public bool isHit = false;
        Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;

        public TimeSpan hitTimestamp;
        private int hitShadeMilliSeconds = 200;

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        bool rotateByPosition = false;
        float rotationInRad = 0f;

        public float drawDepth;

        public Vector2 Position
        {
            get { return spritePosition; }
            set
            {
                lastSpritePosition = spritePosition;
                spritePosition = value;
                X = (int)value.X;
                Y = (int)value.Y;
                //UpdateRotation();
            }
        }

        public int X
        {
            get { return (int)spritePosition.X; }
            set
            {
                lastSpritePosition.X = spritePosition.X;
                spritePosition.X = value;
                //UpdateRotation();
            }
        }

        public int Y
        {
            get { return (int)spritePosition.Y; }
            set
            {
                lastSpritePosition.Y = spritePosition.Y;
                spritePosition.Y = value;
                //UpdateRotation();
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

        // Screen coordinates of the collision box surrounding this sprite
        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(X + 18, Y + 15, spriteWidth - 36, spriteHeight - 15); // TODO Variables
            }

            set
            {
                CollisionBox = value;
            }
        }

        // Screen coordinates of the Bounding Box surrounding this sprite
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(X, Y, spriteWidth, spriteHeight);
            }
        }

        public Texture2D Texture
        {
            get { return spriteTexture; }
            set { spriteTexture = value; }
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
                    frameAnimations[currentAnimation].CurrentFrame = 0;
                    frameAnimations[currentAnimation].playCount = 0;
                }
            }
        }

        public SpriteAnimation(Texture2D Texture)
        {
            drawDepth = 0.0f;
            spriteTexture = Texture;
            spriteHeight = Texture.Height;
            spriteWidth = Texture.Width;
        }

        protected void UpdateRotation()
        {
            if (rotateByPosition)
                rotationInRad = (float)Math.Atan2(spritePosition.Y - lastSpritePosition.Y, spritePosition.X - lastSpritePosition.X);
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

        public void MoveBy(Vector2 movement)
        {
            lastSpritePosition = spritePosition;
            spritePosition += movement;
            UpdateRotation();
        }

        public void Update(GameTime gameTime)
        {
            if (isAnimating)
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

                if (!String.IsNullOrEmpty(CurrentFrameAnimation.nextAnimation))
                    if (CurrentFrameAnimation.playCount > 0)
                        CurrentAnimation = CurrentFrameAnimation.nextAnimation;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color? colorShade)
        {
            if (colorShade != null)
                tintColor = (Color)colorShade;

            if (isAnimating)
            {
                if (!isHit)
                    spriteBatch.Draw(spriteTexture, spritePosition, CurrentFrameAnimation.FrameRectangle, tintColor);
                else
                {
                    spriteBatch.Draw(spriteTexture, spritePosition, CurrentFrameAnimation.FrameRectangle, Color.Crimson);

                    if ((Orbs.currentTimeSpan - hitTimestamp).TotalMilliseconds > hitShadeMilliSeconds)
                        isHit = false;
                }
            }
            //For Debug
            //int bw = 2; // Border width
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(CollisionBox.Left, CollisionBox.Top, bw, CollisionBox.Height), Color.Black); // Left
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(CollisionBox.Right, CollisionBox.Top, bw, CollisionBox.Height), Color.Black); // Right
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(CollisionBox.Left, CollisionBox.Top, CollisionBox.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Orbs.pixel, new Rectangle(CollisionBox.Left, CollisionBox.Bottom, CollisionBox.Width, bw), Color.Black); // Bottom
        }
    }
}

