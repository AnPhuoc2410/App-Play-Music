﻿using Microsoft.Win32;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.MVVM.Views;
using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SpotifyCheaper
{
    public partial class MainWindow : Window
    {
        private MediaPlayer _mediaPlayer = new();
        private bool _isPlaying = false;
        private ObservableCollection<Song> _songs = new();
        private DispatcherTimer _timer;
        private MusicGetDataService _musicService = new();
        private int _currentSongIndex = -1;
        private bool _isShuffling = false;
        private bool _isRepeating = false;
        private int _songIndex = 1;
        private int _lastSongIndex = -1;
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSongs();
        }
        private void InitializePlayer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        private void LoadSongs()
        {
            SongListView.Items.Clear();
            SongListView.ItemsSource = _songs;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                PlayButton.Content = "⏯️";
            }
            else
            {
                _mediaPlayer.Play();
                PlayButton.Content = "⏸️";
            }
            _isPlaying = !_isPlaying;
        }


        private void SongListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongListView.SelectedItem is Song selectedSong)
            {
                _currentSongIndex = _songs.IndexOf(selectedSong);
                PlaySelectedSong(selectedSong);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlayRandomSong();
            SongListView.SelectedIndex = _currentSongIndex;            
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSongIndex > 0)
            {
                _currentSongIndex--;
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songs[_currentSongIndex]);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_isShuffling && _songs.Count > 1)
            {
                PlayRandomSong();
                SongListView.SelectedIndex = _currentSongIndex;
            }
            else if (_isRepeating)
            {
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songs[_currentSongIndex]);
            }
            else if (_currentSongIndex < _songs.Count - 1)
            {
                _currentSongIndex++;
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songs[_currentSongIndex]);
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _isShuffling = ShuffleButton.IsChecked == true;
            if (_isRepeating)
            {
                _isRepeating = false;
                LoopButton.IsChecked = false;
                MessageBox.Show($"Loop is now {(_isRepeating ? "enabled" : "disabled")}");
            }
            ShuffleButton.Opacity = _isShuffling ? 1 : 0.5;
            MessageBox.Show($"Shuffle is now {(_isShuffling ? "enabled" : "disabled")}");
        }
        private void PlayRandomSong()
        {
            Random random = new Random();
            int randomIndex;

            // Ensure the new random song is different from the last one
            do
            {
                randomIndex = random.Next(0, _songs.Count);
            } while (randomIndex == _lastSongIndex);

            _currentSongIndex = randomIndex;
            _lastSongIndex = _currentSongIndex; // Update last played index
            PlaySelectedSong(_songs[_currentSongIndex]); // Play the selected song
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            _isRepeating = LoopButton.IsChecked == true;
            if (_isShuffling)
            {
                _isShuffling = false;
                ShuffleButton.IsChecked = false;
                MessageBox.Show($"Shuffle is now {(_isShuffling ? "enabled" : "disabled")}");
            }
            LoopButton.Opacity = _isRepeating ? 1 : 0.5;
            MessageBox.Show($"Loop is now {(_isRepeating ? "enabled" : "disabled")}");
        }

        private void PlaySelectedSong(Song song)
        {
            string filePath = song.FilePath;  // Use the dynamic file path from the Song object

            _mediaPlayer.Stop();
            _timer.Stop();

            var metadata = _musicService.GetMp3Metadata(filePath);
            if (metadata != null)
            {
                TrackTitleTextBlock.Text = metadata.Title;
                ArtistTitleTextBox.Text = metadata.Artist;
                string durationString = metadata.Duration;
                TimeSpan duration = TimeSpan.ParseExact(durationString, @"mm\:ss", null);
                DurationSlider.Maximum = duration.TotalSeconds;
                DurationTextBlock.Text = metadata.Duration;

                _mediaPlayer.Open(new Uri(filePath));
                _mediaPlayer.Play();
                _isPlaying = true;
                PlayButton.Content = "⏸️";

                _timer.Start();
            }
            else
            {
                MessageBox.Show("Could not retrieve MP3 metadata.");
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan && _mediaPlayer.Position < _mediaPlayer.NaturalDuration.TimeSpan)
            {
                DurationSlider.Value = _mediaPlayer.Position.TotalSeconds;
                CurrentPositionTextBlock.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
            }
            else
            {
                _timer.Stop();
                _isPlaying = false;
                Next_Click(null, null);
            }
        }
        private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging)  // Only seek if not dragging
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(DurationSlider.Value);
            }
        }

        private void DurationSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Get the clicked position within the slider
            var slider = sender as Slider;
            var mousePosition = e.GetPosition(slider);

            // Calculate the percentage of the slider's width that was clicked
            double clickedPercentage = mousePosition.X / slider.ActualWidth;

            // Calculate the new position in seconds based on the clicked percentage
            double newPositionInSeconds = clickedPercentage * slider.Maximum;

            // Set the slider value and update the MediaPlayer position
            slider.Value = newPositionInSeconds;
            _mediaPlayer.Position = TimeSpan.FromSeconds(newPositionInSeconds);
        }


        private void DurationSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = false;
            _mediaPlayer.Position = TimeSpan.FromSeconds(DurationSlider.Value);
        }


        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = e.NewValue;
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Volume = _mediaPlayer.Volume > 0 ? 0 : 0.5;
            VolumeSlider.Value = _mediaPlayer.Volume;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 Files | *.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {

                    var metadata = _musicService.GetMp3Metadata(filePath);
                    if (metadata != null)
                    {
                        _songs.Add(new Song
                        {
                            TrackNumber = _songIndex,
                            Title = metadata.Title,
                            Artist = metadata.Artist,
                            Duration = metadata.Duration,
                            FilePath = filePath,
                            AlbumCoverImage = metadata.AlbumCoverImage,
                        });
                        _songIndex++;
                    }
                    else
                    {
                        MessageBox.Show($"Could not retrieve MP3 metadata for {filePath}.");
                    }
                }
            }
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayerView videoPlayerView = new VideoPlayerView();
            videoPlayerView.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;

            // Get the song associated with this button
            var songToDelete = button?.DataContext as Song;

            if (songToDelete != null)
            {
                int currentIndex = _songs.IndexOf(songToDelete);

                // Remove the song from the _songs collection
                _songs.Remove(songToDelete);

                // If there are still songs in the collection, select the next one
                if (_songs.Count > 0)
                {
                    int nextIndex = currentIndex < _songs.Count ? currentIndex : _songs.Count - 1;
                    SongListView.SelectedIndex = nextIndex;
                    SongListView.ScrollIntoView(SongListView.SelectedItem);
                }
            }
        }


        private void SearchingButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
