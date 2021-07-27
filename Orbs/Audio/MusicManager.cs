using Microsoft.Xna.Framework.Media;

namespace Orbs.Audio
{
    public class MusicManager
    {
        Song backgroundMusic;

        public MusicManager(string file)
        {
            backgroundMusic = Orbs.content.Load<Song>("Music/" + file);
        }

        public MusicManager() { }

        public void PlayBackgroundMusic()
        {
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        public void ChangeBackgroundMusic(string file)
        {
            MediaPlayer.Stop();
            backgroundMusic = Orbs.content.Load<Song>("Music/" + file);
            PlayBackgroundMusic();
        }

        public void ToggleMusic()
        {
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }

        public static void ShutDownMusic()
        {
            MediaPlayer.Stop();
        }

        public static float GetMusicVolume()
        {
            if (MediaPlayer.IsMuted || MediaPlayer.State == MediaState.Stopped)
                return 0.0f;
            else
                return MediaPlayer.Volume;
        }
    }
}
