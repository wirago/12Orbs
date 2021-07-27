using Penumbra;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Orbs.Effects
{
    public class Lightning
    {
        public static Color AMBIENTCOLOR_NIGHT = new Color(138, 179, 243, 43);
        public static Color AMBIENTCOLOR_DAY = new Color(255, 242, 158, 0.75f);
        public static Dictionary<string, int> lightSourceIds = new Dictionary<string, int>(); //contains the hash value of each light source with their map ID
        public static List<LightSourceExtension> lightSourceExtensions = new List<LightSourceExtension>(); //contains additional info for lightsources with their map ID

        private Color startColor;
        private Color finalColor;
        private float colorAmount;

        private Vector2 shakeOffset = Vector2.Zero;
        private float shakeRadius = 20.0f;
        private int shakeAngle = Orbs.rand.Next() % 360;

        private TimeSpan lastDayNightCycleTick;
        private int secondsBetweenDayNightCycleTick = 2;
        private string targetDayTime = "night"; //"day" , "night"
        private bool lightsOn = false;

        public PenumbraComponent effectComponent;

        private Light playerLight = new PointLight
        {
            Scale = new Vector2(800, 600), // Range of the light source (how far the light will travel)
            ShadowType = ShadowType.Occluded,
            Color = AMBIENTCOLOR_DAY
        };

        public Lightning(Game game)
        {
            effectComponent = new PenumbraComponent(game);
            effectComponent.AmbientColor = AMBIENTCOLOR_DAY;
            startColor = effectComponent.AmbientColor;
            finalColor = startColor;

            effectComponent.Lights.Add(playerLight);
            lightSourceIds.Add("player", effectComponent.Lights[0].GetHashCode());

            lastDayNightCycleTick = Orbs.currentTimeSpan;
        }

        public void Update(Rectangle playerPositionRectangle, GameTime gameTime)
        {
            effectComponent.Transform = Orbs.camera.GetViewMatrix();

            float x = playerPositionRectangle.X + playerPositionRectangle.Width / 2;
            float y = playerPositionRectangle.Y + playerPositionRectangle.Height / 2;

            playerLight.Position = new Vector2(x, y);

            UpdateLightFlickering();

            //Day Night Cylce
            //if ((Orbs.currentTimeSpan - lastDayNightCycleTick).Seconds > secondsBetweenDayNightCycleTick)
            //{
            //    float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    colorAmount += deltaSeconds;

            //    lastDayNightCycleTick = Orbs.currentTimeSpan;
            //    if (targetDayTime == "night")
            //    {
            //        effectComponent.AmbientColor = Color.Lerp(effectComponent.AmbientColor, AMBIENTCOLOR_NIGHT, colorAmount / 4);
            //        if (effectComponent.AmbientColor == AMBIENTCOLOR_NIGHT)
            //            ToggleTargetDaytime();
            //    }

            //    else if (targetDayTime == "day")
            //     {
            //        effectComponent.AmbientColor = Color.Lerp(effectComponent.AmbientColor, AMBIENTCOLOR_DAY, colorAmount / 4);

            //        if (effectComponent.AmbientColor == AMBIENTCOLOR_DAY)
            //            ToggleTargetDaytime();
            //    }

            //    if (effectComponent.AmbientColor.R > 220 && lightsOn)
            //        ToggleLights();

            //    if (effectComponent.AmbientColor.R < 220 && !lightsOn)
            //        ToggleLights();
            //}

            if(effectComponent.AmbientColor != finalColor)
                effectComponent.AmbientColor = Color.Lerp(startColor, finalColor, 1f);
        }

        /// <summary>
        /// Moves every light source individualy by a little bit to simualte flickering and moves it back to its origin afterwards
        /// </summary>
        private void UpdateLightFlickering()
        {
            foreach(Light l in effectComponent.Lights)
            {
                int curHash = l.GetHashCode();

                if (!l.Enabled)
                    continue;

                if (curHash == lightSourceIds["player"])
                    continue;

                if (lightSourceIds.Where(x => x.Value == curHash).FirstOrDefault().Key == null)
                    continue;

                string id = lightSourceIds.Where(x => x.Value == curHash).First().Key;
                
                Vector2 curOrigin = lightSourceExtensions.Where(o => o.id == id).First().origin; //Check if Light with given ID has already moved

                if (l.Position != curOrigin)
                {
                    l.Position = curOrigin;
                    continue;
                }

                shakeOffset = new Vector2((float)(Math.Sin(shakeAngle) * shakeRadius), (float)(Math.Cos(shakeAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeAngle += (150 + Orbs.rand.Next(60));

                if (shakeRadius <= 0)
                {
                    shakeRadius = curHash % 10 == 0 ? 5 : curHash % 10; //To avoid 0 values
                    shakeAngle = Orbs.rand.Next() % 360;
                }

                l.Position += shakeOffset;
            }
        }

        public void BeginDraw()
        {
            effectComponent.BeginDraw();
        }

        public void Draw(GameTime gameTime)
        {
            effectComponent.Draw(gameTime);
        }

        /// <summary>
        /// Switching ambient color to given predefined color. If color = day color it will turn off the lights otherwise turn them on
        /// </summary>
        /// <param name="color"></param>
        public void SwitchAmbientColor(Color color)
        {
            startColor = effectComponent.AmbientColor;
            finalColor = color;
            colorAmount = 0.0f;

            if (color == AMBIENTCOLOR_DAY)
            {
                foreach (Light l in effectComponent.Lights)
                {
                    if(l.GetHashCode() == lightSourceIds["player"])
                        continue;

                    l.Enabled = false;
                }
            }
            else
            {
                foreach (Light l in effectComponent.Lights)
                    l.Enabled = true;
            }
        }

        private void ToggleLights()
        {
            lightsOn = !lightsOn;

            foreach (Light l in effectComponent.Lights)
            {
                if (l.GetHashCode() == lightSourceIds["player"])
                    continue;

                l.Enabled = lightsOn;
            }
        }

        private void ToggleTargetDaytime()
        {
            colorAmount = 0.0f;

            if (targetDayTime == "day")
                targetDayTime = "night";
            else
                targetDayTime = "day";
        }
    }
}
