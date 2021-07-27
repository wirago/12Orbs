using Microsoft.Xna.Framework;

namespace Orbs.Animations
{
    public class FrameAnimation
    {
        // The first frame of the Animation.  We will calculate other
        // frames on the fly based on this frame.
        private Rectangle rectInitialFrame;

        // Number of frames in the Animation
        private int frameCount = 1;

        // The frame currently being displayed. 
        // This value ranges from 0 to iFrameCount-1
        private int currentFrame = 0;

        // Amount of time (in seconds) to display each frame
        private float frameLength = 0.2f;

        // Amount of time that has passed since we last animated
        private float frameTimer = 0.0f;

        // The number of times this animation has been played
        public int playCount = 0;

        // The animation that should be played after this animation
        public string nextAnimation = null;

        /// The frame number currently being displayed
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frameCount - 1); }
        }

        public int FrameWidth
        {
            get { return rectInitialFrame.Width; }
        }

        public int FrameHeight
        {
            get { return rectInitialFrame.Height; }
        }

        /// The rectangle associated with the current animation frame.
        public Rectangle FrameRectangle
        {
            get
            {
                return new Rectangle(
                    rectInitialFrame.X + (rectInitialFrame.Width * currentFrame),
                    rectInitialFrame.Y, rectInitialFrame.Width, rectInitialFrame.Height);
            }
        }

        public FrameAnimation(Rectangle FirstFrame, int Frames)
        {
            rectInitialFrame = FirstFrame;
            frameCount = Frames;
        }

        public FrameAnimation(int X, int Y, int Width, int Height, int Frames)
        {
            rectInitialFrame = new Rectangle(X, Y, Width, Height);
            frameCount = Frames;
        }

        public FrameAnimation(int X, int Y, int Width, int Height, int Frames, float FrameLength)
        {
            rectInitialFrame = new Rectangle(X, Y, Width, Height);
            frameCount = Frames;
            frameLength = FrameLength;
        }

        public FrameAnimation(int X, int Y,
            int Width, int Height, int Frames,
            float FrameLength, string strNextAnimation)
        {
            rectInitialFrame = new Rectangle(X, Y, Width, Height);
            frameCount = Frames;
            frameLength = FrameLength;
            nextAnimation = strNextAnimation;
        }

        public void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (frameTimer > frameLength)
            {
                frameTimer = 0.0f;
                currentFrame = (currentFrame + 1) % frameCount;
                if (currentFrame == 0)
                    playCount = (int)MathHelper.Min(playCount + 1, int.MaxValue);
            }
        }
    }
}
