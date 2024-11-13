using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.MVVM.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SpotifyCheaper
{
    public partial class MainWindow : Window
    {
        private FileService fileService = new();
        private MusicService _musicService = new();
        private MediaPlayer _mediaPlayer = new();
        private SongsService _songSerivce;
        private MusicButtonService _musicButtonService;

        private bool _isPlaying = false;
        private DispatcherTimer _timer;
        private int _currentSongIndex = -1;
        private bool _isShuffling = false;
        private int _songIndex = 1;
        private int _lastSongIndex = -1;
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            _songSerivce = new SongsService(fileService, _musicService);
            _musicButtonService = new MusicButtonService(_mediaPlayer, _songSerivce.Songs);
            InitializePlayer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _songSerivce.LoadSongsFromJson();
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
            SongListView.ItemsSource = _songSerivce.Songs;

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
                _currentSongIndex = _songSerivce.Songs.IndexOf(selectedSong);
                PlaySelectedSong(selectedSong);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlayRandomSong();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSongIndex > 0)
            {
                _currentSongIndex--;
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_isShuffling && _songSerivce.Songs.Count > 1)
            {
                PlayRandomSong();
                SongListView.SelectedIndex = _currentSongIndex;
            }
            else
            {
                if (_currentSongIndex < _songSerivce.Songs.Count - 1)
                {
                    _currentSongIndex++;
                }
                else
                {
                    // Reset to the first song if it's the last song
                    _currentSongIndex = 0;
                }
                SongListView.SelectedIndex = _currentSongIndex;
                PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]);
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _isShuffling = ShuffleButton.IsChecked == true;
            ShuffleButton.Opacity = _isShuffling ? 1 : 0.5;
            MessageBox.Show($"Shuffle is now {(_isShuffling ? "enabled" : "disabled")}");
        }
        private void PlayRandomSong()
        {
            Random random = new Random();
            int randomIndex;

            do
            {
                randomIndex = random.Next(0, _songSerivce.Songs.Count);
            } while (randomIndex == _lastSongIndex);

            _currentSongIndex = randomIndex;
            _lastSongIndex = _currentSongIndex; // Update last played index
            PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]); // Play the selected song
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            bool isRepeating = LoopButton.IsChecked == true;
            MessageBox.Show($"Repeat is now {(isRepeating ? "enabled" : "disabled")}");
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
                Next_Click(null,null);
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
            if (sender is Slider slider)
            {
                var mousePosition = e.GetPosition(slider);

                double clickedPercentage = mousePosition.X / slider.ActualWidth;

                double newPositionInSeconds = clickedPercentage * slider.Maximum;

                slider.Value = newPositionInSeconds;
                _mediaPlayer.Position = TimeSpan.FromSeconds(newPositionInSeconds);
            }
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
            try
            {
                _songSerivce.ImportSongs();
                MessageBox.Show("Add song success", "Ok", MessageBoxButton.OK, MessageBoxImage.None);
            }
            catch (Exception)
            {
                MessageBox.Show("Add song unsuccess", "Ok", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayerView videoPlayerView = new VideoPlayerView();
            this.Hide();
            videoPlayerView.ShowDialog();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Song songToDelete)
            {
                _songSerivce.DeleteSong(songToDelete);
                MessageBox.Show("Song deleted.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void SearchingButton_Click(object sender, RoutedEventArgs e)
        {
            string sSearchName = SearchingTextBox.Text.Trim();
            var displayListSong = _songSerivce.FindMusic(sSearchName);
            //SongListView.Items.Clear();
            SongListView.ItemsSource = displayListSong;

        }
    }
}
