using Microsoft.Win32;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
        private Random _random = new();

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
                ((Button)sender).Content = "⏯️"; // Change button text to Play
            }
            else
            {
                _mediaPlayer.Play();
                ((Button)sender).Content = "⏸️"; // Change button text to Pause
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
            if (_currentSongIndex >= 0 && _currentSongIndex < _songs.Count)
            {
                PlaySelectedSong(_songs[_currentSongIndex]);
            }
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
            if (_currentSongIndex < _songs.Count - 1)
            {
                _currentSongIndex++;
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songs[_currentSongIndex]);
            }
        }
        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            _isShuffling = !_isShuffling;
            ShuffleButton.Opacity = _isShuffling ? 1 : 0.5;
            MessageBox.Show($"Shuffle is now {(_isShuffling ? "enabled" : "disabled")}");
        }

        private void Loop_Click(object sender, RoutedEventArgs e)
        {
            _isRepeating = !_isRepeating;
            LoopButton.Opacity = _isRepeating ? 1 : 0.5;
            MessageBox.Show($"Repeat is now {(_isRepeating ? "enabled" : "disabled")}");
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
                DurationSlider.Maximum = metadata.Duration.TotalSeconds;
                DurationTextBlock.Text = metadata.Duration.ToString(@"mm\:ss");

                _mediaPlayer.Open(new Uri(filePath));
                _mediaPlayer.Play();
                _isPlaying = true;

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
                int count = 1;
                foreach (var filePath in openFileDialog.FileNames)
                {
                    
                    var metadata = _musicService.GetMp3Metadata(filePath);
                    if (metadata != null)
                    {
                        _songs.Add(new Song
                        {
                            TrackNumber = count,
                            Title = metadata.Title,
                            Artist = metadata.Artist,
                            Duration = metadata.Duration,
                            FilePath = filePath  // Store the file path
                        });
                        count++;
                    }
                    else
                    {
                        MessageBox.Show($"Could not retrieve MP3 metadata for {filePath}.");
                    }
                }
            }
        }

    }
}
