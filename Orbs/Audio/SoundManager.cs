using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Orbs.Audio
{
    public class SoundManager
    {
        public static bool soundMuted = false;

        private SoundEffectInstance soundEffectInstance;
        private Dictionary<string, SoundEffectInstance> proximitySounds = new Dictionary<string, SoundEffectInstance>();
        private Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        /// <summary>
        /// Plays given sound in Sounds dir
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="volume"></param>
        public void PlaySound(string sound, float volume)
        {
            if (soundMuted)
                return;

            if (sound.EndsWith(@"\"))
                return;

            if (!soundEffects.ContainsKey(sound))
            {
                try
                {
                    soundEffects.Add(sound, Orbs.content.Load<SoundEffect>("Sounds/" + sound));
                }
                catch
                {
                    System.Console.WriteLine("Failed to load sound " + sound + " into dictionary");
                    return;
                }
            }

            soundEffectInstance = soundEffects[sound].CreateInstance();
            soundEffectInstance.IsLooped = false;
            soundEffectInstance.Volume = volume;
            soundEffectInstance.Play();

        }

        public void PlayProximitySound(string sound, float volume)
        {
            if (soundMuted)
                return;

            if (sound == null)
                return;
            if (sound.EndsWith(@"\"))
                return;

            if (!proximitySounds.ContainsKey(sound))
            {
                try
                {
                    proximitySounds.Add(sound, Orbs.content.Load<SoundEffect>("Sounds/" + sound).CreateInstance());
                }
                catch
                {
                    System.Console.WriteLine("Failed to load sound " + sound + " into dictionary");
                    return;
                }
            }
            else
            {
                return;
            }

            proximitySounds[sound].IsLooped = true;
            proximitySounds[sound].Volume = volume;
            proximitySounds[sound].Play();
        }

        public void UpdateProximityVolume(string sound, float volume)
        {
            if (sound == null)
                return;

            if(proximitySounds.ContainsKey(sound))
                proximitySounds[sound].Volume = volume;
        }

        public void ToggleSound()
        {
            soundMuted = !soundMuted;
        }
    }
}
