using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;


namespace Orbs.LootSystem
{
    public class Glyph : Item
    {
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public int Manacost { get; set; }
        public string SoundEffect { get; set; }
        public Texture2D GlyphTexture { get; set; }
        public TimeSpan PlacedAt { get; set; }
        public Vector2 position = Vector2.Zero;

        public bool expired = false;

        private float rotation = 0.0f;
        private float scale = 1.0f;
        private bool rising = true;
        private Color color = Color.Purple;
        
        public bool isPlaced = false;
        public Vector2 worldPosition = Vector2.Zero;

        public Helper.Circle effectCircle;

        public Glyph() { }

        //Copy Constructor
        public Glyph(string glyphId)
        {
            Glyph temp = LootTable.glyphs.Where(g => g.Id == glyphId).First();

            new Glyph
            {
                Id = temp.Id,
                Name = temp.Name,
                MinDmg = temp.MinDmg,
                MaxDmg = temp.MaxDmg,
                Manacost = temp.Manacost,
                IconTexture = temp.IconTexture,
                Type = temp.Type,
                UseEffect = temp.UseEffect,
                SoundEffect = temp.SoundEffect,
                GlyphTexture = temp.GlyphTexture
            };
        }

        public Helper.Circle GetEffectCircle()
        {
            return new Helper.Circle(new Vector2(worldPosition.X, worldPosition.Y), GlyphTexture.Width / 2);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            color.G = (byte)Orbs.rand.Next(200, 255);

            if (UseEffect.EndsWith("fire"))
                color.R = (byte)Orbs.rand.Next(200, 255);

            if (UseEffect.EndsWith("water"))
                color.B = (byte)Orbs.rand.Next(200, 255);

            if (GlyphTexture == null)
                return;

            if (!isPlaced)
                spriteBatch.Draw(GlyphTexture, position, null, color, rotation, new Vector2(GlyphTexture.Width / 2, GlyphTexture.Height / 2), scale, SpriteEffects.None, 1.0f);
            else
                spriteBatch.Draw(GlyphTexture, worldPosition, null, color, rotation, new Vector2(GlyphTexture.Width / 2, GlyphTexture.Height / 2), scale, SpriteEffects.None, 1.0f);
        }

        public void Update(GameTime gameTime, Vector2 mousePos, Vector2 mouseWorldPos)
        {
            position.X = mousePos.X;
            position.Y = mousePos.Y;
            rotation -= 0.005f;

            worldPosition.X = mouseWorldPos.X;
            worldPosition.Y = mouseWorldPos.Y;

            if (scale <= 1.05f && rising)
                scale += 0.002f;
            if (scale >= 1.05f)
                rising = false;
            if (!rising)
                scale -= 0.002f;
            if (scale <= 0.95f)
                rising = true;
        }
    }
}
