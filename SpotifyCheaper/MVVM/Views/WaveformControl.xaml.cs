using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for WaveformControl.xaml
    /// </summary>
    public partial class WaveformControl : UserControl
    {
        private AudioFileReader _audioFileReader;
        private string _audioFilePath;

        public WaveformControl()
        {
            InitializeComponent();
        }
        public string AudioFilePath
        {
            get { return _audioFilePath; }
            set
            {
                _audioFilePath = value;
                LoadAudioFile();
            }
        }
        private void LoadAudioFile()
        {
            if (!string.IsNullOrEmpty(_audioFilePath))
            {
                _audioFileReader = new AudioFileReader(_audioFilePath);

                // Clear the canvas
                WaveformCanvas.Children.Clear();

                // Get the audio data and sample rate
                float[] audioData = new float[_audioFileReader.Length / sizeof(float)];
                _audioFileReader.Read(audioData, 0, audioData.Length);
                int sampleRate = _audioFileReader.WaveFormat.SampleRate;

                // Draw the waveform
                DrawWaveform(audioData, sampleRate);
            }
        }

        private void DrawWaveform(float[] audioData, int sampleRate)
        {
            double width = WaveformCanvas.ActualWidth;
            double height = WaveformCanvas.ActualHeight;

            for (int i = 0; i < audioData.Length; i++)
            {
                double x = (i / (double)sampleRate) * width;
                double y = (1 + audioData[i]) * height / 2; // Normalize value to fit in canvas height

                Line line = new Line
                {
                    X1 = x,
                    Y1 = height / 2,
                    X2 = x,
                    Y2 = y,
                    Stroke = Brushes.Green,
                    StrokeThickness = 1
                };

                WaveformCanvas.Children.Add(line);
            }
        }
    }
}
