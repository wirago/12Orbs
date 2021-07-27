using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.Interface;
using System;
using System.Collections.Generic;

namespace Orbs.Effects
{
    public enum EffectType { FlashScreen, ShakeScreen, GroundEffect };

    public class Effect
    {
        public EffectType effectType;
        public string id;
        private int duration; //in seconds
        private const int FLASHSPEED = 300; //in milliseconds
        private List<Texture2D> textures;
        private Texture2D activeTexture;
        private string textAfterEffect;
        private float fader = 1.0f;

        private TimeSpan activeFrom;
        private TimeSpan lastFlashImageChange;
        private Vector2 curPosition;
        private Color ambienteStartColor = EffectManager.lightning.effectComponent.AmbientColor;

        public bool isActiveEffect = true;

        /// <summary>
        /// Generic Effect
        /// </summary>
        internal Effect(string id, EffectType effectType, int duration, string[] textures, string soundfile, string textAfterEffect, Color? ambientChange = null)
        {
            this.id = id;
            this.effectType = effectType;
            this.duration = duration;
            this.textAfterEffect = textAfterEffect;

            activeFrom = Orbs.currentTimeSpan;
            lastFlashImageChange = Orbs.currentTimeSpan;

            if (effectType == EffectType.FlashScreen)
            {
                this.textures = new List<Texture2D>();

                foreach(string texture in textures)
                    this.textures.Add(Orbs.content.Load<Texture2D>(@"Textures\Effects\" + texture));

                World.WorldObjects.ChangeAmbienteColor(Lightning.AMBIENTCOLOR_NIGHT);
                activeTexture = this.textures[0];

                curPosition = GetCenterPostion(activeTexture.Width, activeTexture.Height);
            }

            if (effectType == EffectType.ShakeScreen)
                Orbs.shakeViewport = true;

            if(ambientChange != null)
                World.WorldObjects.ChangeAmbienteColor(ambientChange.GetValueOrDefault(Color.Red));

            if(soundfile != null)
                Orbs.soundEffects.PlaySound(soundfile, 1.0f);
        }

        /// <summary>
        /// For Ground Effects
        /// </summary>
        internal Effect(string id, EffectType effectType, string texture, int duration, Vector2 position)
        {
            this.id = id;
            this.effectType = effectType;
            this.duration = duration;
            curPosition = position;

            activeFrom = Orbs.currentTimeSpan;
            activeTexture = Orbs.content.Load<Texture2D>(@"Textures\Effects\" + texture);
        }


        public void UpdateEffect(GameTime gameTime)
        {
            if (effectType == EffectType.GroundEffect)
            {
                if (Orbs.currentTimeSpan.TotalSeconds - activeFrom.TotalSeconds > duration)
                    FadeOut(gameTime);
            }
            else if (Orbs.currentTimeSpan.TotalSeconds - activeFrom.TotalSeconds > duration)
            {
                isActiveEffect = false;
                if(textAfterEffect != null)
                    UIManager.ShowNotification(Localization.strings_hollowrock.abandonedHouse_02); //placeholder

                if (effectType == EffectType.ShakeScreen)
                    Orbs.shakeViewport = false;

                World.WorldObjects.ChangeAmbienteColor(ambienteStartColor);
                return;
            }

            if(effectType == EffectType.FlashScreen)
            {
                TimeSpan temp = Orbs.currentTimeSpan - lastFlashImageChange;
                if (temp.Milliseconds > FLASHSPEED || temp.Milliseconds == 0)
                {
                    if (textures.Count > 1)
                    {
                        int curIndex = textures.IndexOf(activeTexture);
                        activeTexture = textures[(curIndex + 1) % textures.Count];
                        curPosition = GetCenterPostion(activeTexture.Width, activeTexture.Height);
                    }
                    else
                    {
                        activeTexture = activeTexture == null ? textures[0] : null;

                        if(activeTexture != null)
                            curPosition = GetCenterPostion(activeTexture.Width, activeTexture.Height);
                    }
                }
            }

            
        }

        private void FadeOut(GameTime gameTime)
        {
            fader -= 0.01f;
            
            if (fader < 0.1)
                isActiveEffect = false;
        }

        private Vector2 GetCenterPostion(int width, int height)
        {
            return new Vector2(
                    (Orbs.graphics.PreferredBackBufferWidth - width) / 2,
                    (Orbs.graphics.PreferredBackBufferHeight - height) / 2
                    );
        }

        public void DrawEffect(SpriteBatch spriteBatch)
        {
            if(isActiveEffect && effectType == EffectType.GroundEffect)
            {
                spriteBatch.Draw(activeTexture, curPosition, Color.White * fader);
                return;
            }

            if(isActiveEffect && activeTexture != null)
                spriteBatch.Draw(activeTexture, curPosition, Color.White * 0.75f);
        }
    }
}
