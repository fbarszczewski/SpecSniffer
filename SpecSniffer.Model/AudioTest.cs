using System;
using System.IO;
using System.Media;
using System.Windows.Media;

namespace SpecSniffer.Model
{
    public class AudioTest
    {
        private readonly string _audioPath;
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();

        public AudioTest(string audioPath)
        {
            _audioPath = audioPath;
        }

        public void Play()
        {
            if (File.Exists(_audioPath))
            {
                var toneUrl = new Uri(_audioPath);
                _mediaPlayer.Open(toneUrl);
                _mediaPlayer.Play();
            }
            else
            {
                SystemSounds.Asterisk.Play();
            }
        }

        public void Stop()
        {
            _mediaPlayer?.Stop();
        }
    }
}