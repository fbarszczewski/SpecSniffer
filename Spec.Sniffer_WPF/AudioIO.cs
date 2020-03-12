using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SpecSniffer.Model;

namespace Spec.Sniffer_WPF
{
    public partial class AudioIO : Form
    {
        public AudioIO(string dir)
        {
            InitializeComponent();
            _playTest = new AudioTest($"{dir}\\Resources\\ShortTone.mp3");
            StartMic();
            _playTest.Play();
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            var cplPath = Path.Combine(Environment.SystemDirectory, "control.exe");
            Process.Start(cplPath, "/name Microsoft.Sound");
        }

        private void AudioIO_FormClosing(object sender, FormClosingEventArgs e)
        {
            _playTest.Stop();
        }

        private void AudioIO_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    PlayButton.PerformClick();
                    break;
                case Keys.F2:
                    StopButton.PerformClick();
                    break;
                case Keys.F3:
                    SoundButton.PerformClick();
                    break;
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        #region Mic variables

        private static double audioValueMax;
        private static double audioValueLast;
        private static readonly int RATE = 44100;
        private static readonly int BUFFER_SAMPLES = 1024;

        #endregion

        #region Audio variables

        private MMDevice _device;

        private readonly AudioTest _playTest;

        #endregion

        #region Audio Level

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            _device = GetDefaultAudioEndpoint();

            progressBar1.Value = (int) Math.Round(_device.AudioMeterInformation.MasterPeakValue * 100);
        }

        private static MMDevice GetDefaultAudioEndpoint()
        {
            var enumerator = new MMDeviceEnumerator();
            return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            _playTest.Play();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            _playTest.Stop();
        }

        #endregion

        #region Mic Level

        private void RecTimer_Tick(object sender, EventArgs e)
        {
            var frac = audioValueLast / audioValueMax;
            pictureBox_front.Width = (int) (frac * pictureBox_back.Width);
            //MicLevelLabel.Text = string.Format("Mic-level: {0:00.00}%", frac * 100.0);
        }

        private void StartMic()
        {

            var waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new WaveFormat(RATE, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.BufferMilliseconds = (int) (BUFFER_SAMPLES / (double) RATE * 1000.0);

            try
            {
                waveIn.StartRecording();
            }
            catch (Exception)
            {

            }

        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            float max = 0;

            // interpret as 16 bit audio
            for (var index = 0; index < args.BytesRecorded; index += 2)
            {
                var sample = (short) ((args.Buffer[index + 1] << 8) |
                                      args.Buffer[index + 0]);
                var sample32 = sample / 32768f; // to floating point
                if (sample32 < 0) sample32 = -sample32; // absolute value 
                if (sample32 > max) max = sample32; // is this the max value?
            }

            // calculate what fraction this peak is of previous peaks
            if (max > audioValueMax) audioValueMax = max;
            audioValueLast = max;
        }

        #endregion
    }
}