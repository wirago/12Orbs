using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Orbs.Effects
{
    public static class EffectManager
    {
        public static List<Effect> effects = new List<Effect>();
        public static Lightning lightning;

        public static Texture2D flash_HalfCircle = Orbs.content.Load<Texture2D>(@"Textures\Effects\Flash_HalfCircle");
        public static Texture2D flash_Segment = Orbs.content.Load<Texture2D>(@"Textures\Effects\Flash_Segment");
        public static Texture2D flash_Pixel = Orbs.content.Load<Texture2D>(@"Textures\Effects\Flash_Pixel");

        public static FlashBolt flashBolt;
        public static List<FlashBolt> flashBolts = new List<FlashBolt>();

        public static void UpdateEffects(GameTime gameTime)
        {
            foreach (Effect e in effects)
            {
                if (e.isActiveEffect)
                    e.UpdateEffect(gameTime);
            }

            effects.RemoveAll(e => !e.isActiveEffect);

            if (flashBolt != null)
                flashBolt.Update();

            if(flashBolts.Count > 0)
            {
                for (int i = 0; i <= flashBolts.Count - 1; i++)
                    if (flashBolts[i].IsComplete)
                        flashBolts.RemoveAt(i);

                foreach (FlashBolt fb in flashBolts)
                    fb.Update();
            }
        }

        public static void CreateEffect(string id, EffectType effectType, int duration, string[] textures, string soundfile, Color ambientChange)
        {
            if (effects.Any(e => e.id == id))
                return;

            effects.Add(new Effect(id, effectType, duration, textures, soundfile, null, ambientChange));
        }

        public static void CreateEffect(string id, EffectType effectType, int duration, string[] textures, string soundfile, Color ambientChange, string textAfterEffect)
        {
            if (effects.Any(e => e.id == id))
                return;

            effects.Add(new Effect(id, effectType, duration, textures, soundfile, textAfterEffect, ambientChange));
        }

        public static void CreateShakeEffect(string id, int duration)
        {
            if (effects.Any(e => e.id == id))
                return;

            effects.Add(new Effect(id, EffectType.ShakeScreen, duration, null, null, null, null));
        }

        public static void CreateGroundEffect(string id, string texture, int duration, Vector2 position)
        {
            if (effects.Any(e => e.id == id))
                return;

            effects.Add(new Effect(id, EffectType.GroundEffect, texture, duration, position));
        }

        public static List<FlashLine> CreateBolt(Vector2 source, Vector2 dest, float thickness)
        {
            var results = new List<FlashLine>();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add(FlashBolt.Rand(0, 1));

            positions.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = FlashBolt.Rand(-Sway,  Sway);

                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new FlashLine(prevPoint, point, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new FlashLine(prevPoint, dest, thickness));

            return results;
        }

        public static void DrawGroundEffects(SpriteBatch spriteBatch)
        {
            foreach (Effect e in effects)
                if(e.effectType == EffectType.GroundEffect)
                    e.DrawEffect(spriteBatch);
        }

        public static void DrawEffects(SpriteBatch spriteBatch)
        {
            foreach (Effect e in effects)
                if (e.effectType != EffectType.GroundEffect)
                    e.DrawEffect(spriteBatch);

            if (flashBolt != null)
                flashBolt.Draw(spriteBatch);

            if(flashBolts.Count > 0)
            {
                foreach (FlashBolt fb in flashBolts)
                    fb.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Clears all lights before loading new map
        /// </summary>
        public static void ClearLights()
        {
            for (int i = Lightning.lightSourceIds.Count - 1; i >= 0; i--)
            {
                if (Lightning.lightSourceIds.ElementAt(i).Key != "player")
                    Lightning.lightSourceIds.Remove(Lightning.lightSourceIds.ElementAt(i).Key);
            }

            Lightning.lightSourceExtensions.Clear();

            //Lights[0] reserved by player light
            for (int j = EffectManager.lightning.effectComponent.Lights.Count - 1; j >= 1; j--)
            {
                EffectManager.lightning.effectComponent.Lights.RemoveAt(j);
            }
                
        }
    }
}
