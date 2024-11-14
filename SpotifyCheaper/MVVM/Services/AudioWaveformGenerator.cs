using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;

namespace SpotifyCheaper.MVVM.Services
{
    public class AudioWaveformGenerator
    {
        private WasapiLoopbackCapture _loopbackCapture;
        private List<float> _sampleBuffer;
        private const int BufferSize = 1024; // Number of samples to process at once

        public event Action<List<float>> OnSamplesCaptured;

        public AudioWaveformGenerator()
        {
            _loopbackCapture = new WasapiLoopbackCapture();
            _loopbackCapture.DataAvailable += LoopbackCapture_DataAvailable;
            _sampleBuffer = new List<float>();
        }

        private void LoopbackCapture_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int i = 0; i < e.BytesRecorded; i += 4)
            {
                float sample = BitConverter.ToSingle(e.Buffer, i);
                _sampleBuffer.Add(sample);

                // If the buffer reaches the specified size, process the samples
                if (_sampleBuffer.Count >= BufferSize)
                {
                    // Create a copy of the current samples
                    List<float> samplesToProcess = new List<float>(_sampleBuffer);
                    _sampleBuffer.Clear(); // Clear the buffer for new samples
                    OnSamplesCaptured?.Invoke(samplesToProcess);
                }
            }
        }

        public void StartCapturing() => _loopbackCapture.StartRecording();
        public void StopCapturing() => _loopbackCapture.StopRecording();
    }
}