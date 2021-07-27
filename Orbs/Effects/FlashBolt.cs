using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbs.Effects
{
    public class FlashBolt
    {
        public List<FlashLine> Segments = new List<FlashLine>();

        public float Alpha { get; set; }
        public float FadeOutRate { get; set; }
        public Color Tint { get; set; }

        public bool IsComplete { get { return Alpha <= 0; } }

        public FlashBolt(Vector2 source, Vector2 dest) : this(source, dest, new Color(0.9f, 0.8f, 1f)) { }

        public FlashBolt(Vector2 source, Vector2 dest, Color color)
        {
            Segments = EffectManager.CreateBolt(source, dest, 2);

            Tint = color;
            Alpha = 1f;
            FadeOutRate = 0.03f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(spriteBatch, Tint * (Alpha * 0.6f));
        }

        public void Update()
        {
            if (IsComplete)
            {
                EffectManager.flashBolt = null;
                return;
            }

            Alpha -= FadeOutRate;
        }

        public static float Rand(float min, float max)
        {
            return (float)Orbs.rand.NextDouble() * (max - min) + min;
        }

    }
}
