using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbs
{
    public class Camera2D
    {
        private readonly Viewport viewport;
        private Vector2 position;

        public int viewWidth;
        public int viewHeight;
        public int worldWidth;
        public int worldHeight;

        public Camera2D(Viewport viewport)
        {
            this.viewport = viewport;

            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(viewport.Width / 2, viewport.Height / 2);
            Position = Vector2.Zero;

            viewWidth = viewport.Width;
            viewHeight = viewport.Height;
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = new Vector2(
                    MathHelper.Clamp(value.X, 0, worldWidth - viewport.Width),
                    MathHelper.Clamp(value.Y, 0, worldHeight - viewport.Height));
            }
        }

        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0f));
        }

        public void Update(int xPos, int yPos)
        {
            Position = new Vector2(xPos - viewWidth / 2, yPos - viewHeight / 2);
        }
    }
}
