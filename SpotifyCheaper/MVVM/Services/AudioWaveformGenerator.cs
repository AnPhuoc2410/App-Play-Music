using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class AudioWaveformGenerator
    {
        private WasapiLoopbackCapture _loopbackCapture;

        public event Action<List<float>> OnSamplesCaptured;

        public AudioWaveformGenerator()
        {
            _loopbackCapture = new WasapiLoopbackCapture();
            _loopbackCapture.DataAvailable += LoopbackCapture_DataAvailable;
        }

        private void LoopbackCapture_DataAvailable(object sender, WaveInEventArgs e)
        {
            List<float> samples = new List<float>();
            for (int i = 0; i < e.BytesRecorded; i += 4)
            {
                float sample = BitConverter.ToSingle(e.Buffer, i);
                samples.Add(sample);
            }
            OnSamplesCaptured?.Invoke(samples);
        }

        public void StartCapturing() => _loopbackCapture.StartRecording();
        public void StopCapturing() => _loopbackCapture.StopRecording();
    }
}
