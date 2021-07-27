using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbs.Effects
{
    public class FlashLine
    {
        public Vector2 endpoint_A;
        public Vector2 endpoint_B;
        public float thickness;

        public FlashLine() { }

        public FlashLine(Vector2 a, Vector2 b, float thickness = 1)
        {
            endpoint_A = a;
            endpoint_B = b;
            this.thickness = thickness;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 tangent = endpoint_B - endpoint_A;
            float rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            const float ImageThickness = 8;
            float thicknessScale = thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(EffectManager.flash_HalfCircle.Width, EffectManager.flash_HalfCircle.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, EffectManager.flash_Segment.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            spriteBatch.Draw(EffectManager.flash_Segment, endpoint_A, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(EffectManager.flash_HalfCircle, endpoint_A, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(EffectManager.flash_HalfCircle, endpoint_B, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }


    }
}
