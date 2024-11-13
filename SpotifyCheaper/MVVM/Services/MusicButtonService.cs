using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicButtonService
    {
        private readonly MediaPlayer _mediaPlayer;
        private readonly ObservableCollection<Song> _songs;
        private int _currentSongIndex;
        private bool _isPlaying;
        private bool _isShuffling;
        private bool _isLooping;

        public MusicButtonService(MediaPlayer mediaPlayer, ObservableCollection<Song> songs)
        {
            _mediaPlayer = mediaPlayer;
            _songs = songs;
            _currentSongIndex = -1;
        }

        public void PlayPause()
        {
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
            }
            else
            {
                _mediaPlayer.Play();
            }
            _isPlaying = !_isPlaying;
        }

        public void PlaySelectedSong(Song song)
        {
            _currentSongIndex = _songs.IndexOf(song);
            PlayCurrentSong();
        }

        private void PlayCurrentSong()
        {
            if (_currentSongIndex < 0 || _currentSongIndex >= _songs.Count)
                return;

            _mediaPlayer.Stop();
            _mediaPlayer.Open(new Uri(_songs[_currentSongIndex].FilePath));
            _mediaPlayer.Play();
            _isPlaying = true;
        }

        public void Next()
        {
            if (_isShuffling)
            {
                PlayRandomSong();
            }
            else
            {
                _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
                PlayCurrentSong();
            }
        }

        public void Previous()
        {
            _currentSongIndex = (_currentSongIndex - 1 + _songs.Count) % _songs.Count;
            PlayCurrentSong();
        }

        public void Shuffle(bool isShuffling)
        {
            _isShuffling = isShuffling;
        }

        public void Loop(bool isLooping)
        {
            _isLooping = isLooping;
        }

        public void PlayRandomSong()
        {
            var random = new Random();
            int randomIndex;
            do
            {
                randomIndex = random.Next(_songs.Count);
            } while (randomIndex == _currentSongIndex);

            _currentSongIndex = randomIndex;
            PlayCurrentSong();
        }
    }
}
