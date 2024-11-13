using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.MVVM.Views;
using SpotifyCheaper.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private bool _isShuffling = false;//Shuffle list
        private bool _isRepeating = false;//Repaet All list
        private bool _isLooping = false;// Repeat One (Loop current song)


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

        private void ArtistsButton_Click(object sender, RoutedEventArgs e)
        {
            PlayListBox.Visibility = PlayListBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            // Optionally, bind data here if not already bound in your ViewModel setup
            DataContext = new MainViewModel();
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
            if (SongListView.SelectedItem is null)
            {
                MessageBox.Show("Please select a song to play", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                // Set to pause icon using direct path
                PlayButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"..\Resources\Images\play.png", UriKind.Relative)),
                    Width = 24,
                    Height = 24
                };
            }
            else
            {
                _mediaPlayer.Play();
                PlayButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"..\Resources\Images\pause.png", UriKind.Relative)),
                    Width = 24,
                    Height = 24
                };
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
            if (_isLooping) 
            {
                PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]);
            }
            else if (_isShuffling && _songSerivce.Songs.Count > 1)
            {
                PlayRandomSong();
                SongListView.SelectedIndex = _currentSongIndex;
            }
            else
            {
                _currentSongIndex++;
                if (_currentSongIndex >= _songSerivce.Songs.Count)
                {
                    _currentSongIndex = _isRepeating ? 0 : -1;
                }

                if (_currentSongIndex >= 0)
                {
                    SongListView.SelectedIndex = _currentSongIndex;
                    PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]);
                }
                else
                {
                    _mediaPlayer.Stop(); // Stop if no repeat and the playlist ends
                }
            }
        }


        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _isShuffling = ShuffleButton.IsChecked == true;
            ShuffleIcon.Source = new BitmapImage(new Uri(_isShuffling
                    ? @"..\Resources\Images\shuffle.png"
                    : @"..\Resources\Images\no_shuffle.png", UriKind.Relative));
            ShuffleButton.ToolTip = new ToolTip { Content = $"Shuffle: {(_isShuffling ? "On" : "Off")}" };
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
            _lastSongIndex = _currentSongIndex; 
            PlaySelectedSong(_songSerivce.Songs[_currentSongIndex]);
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRepeating)
            {
                // Switch from Repeat All to None
                _isRepeating = false;
                _isLooping = false;
                LoopIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\no_repeat.png", UriKind.Relative));
                LoopButton.ToolTip = "Loop: Off";
            }
            else if (_isLooping)
            {
                // Switch from Repeat One to Repeat All
                _isLooping = false;
                _isRepeating = true;
                LoopIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\repeat.png", UriKind.Relative));
                LoopButton.ToolTip = "Loop: Repeat All";
            }
            else
            {
                // Switch from None to Repeat One
                _isLooping = true;
                _isRepeating = false;
                LoopIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\repeat_one.png", UriKind.Relative));
                LoopButton.ToolTip = "Loop: Repeat One";
            }
        }

        private void PlaySelectedSong(Song song)
        {
            string filePath = song.FilePath; 

            _mediaPlayer.Stop();
            _timer.Stop();

            var metadata = _musicService.GetMp3Metadata(filePath);
            if (metadata != null)
            {
                TrackTitleTextBlock.Text = metadata.Title;
                ArtistTitleTextBox.Text = metadata.Artist;

                TimeSpan duration = TimeSpan.ParseExact(metadata.Duration, @"mm\:ss", null);
                DurationSlider.Maximum = duration.TotalSeconds;
                DurationTextBlock.Text = metadata.Duration;

                _mediaPlayer.Open(new Uri(filePath));
                _mediaPlayer.Play();
                _isPlaying = true;

                PlayButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"..\Resources\Images\pause.png", UriKind.Relative)),
                    Width = 24,
                    Height = 24
                };

                _timer.Start();

                // Handle album art if available
                if (metadata.AlbumArt != null && metadata.AlbumArt.Length > 0)
                {
                    using (var ms = new MemoryStream(metadata.AlbumArt))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        SongImage.Source = image;
                    }
                }
                else
                {
                    // Set a default image if no album art is available
                    SongImage.Source = new BitmapImage(new Uri(@"..\Resources\Images\default_album.png", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show("Could not retrieve MP3!");
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
            _mediaPlayer.Volume = e.NewValue; // Set the volume to the slider's new value

            // Update the icon and tooltip based on the volume level
            if (_mediaPlayer.Volume == 0)
            {
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\mute_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Mute";
            }
            else if (_mediaPlayer.Volume > 0 && _mediaPlayer.Volume <= 0.5)
            {
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\low_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Low Volume";
            }
            else
            {
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\max_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Max Volume";
            }
        }


        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer.Volume == 0)
            {
                _mediaPlayer.Volume = 0.5; // Set to low volume
                VolumeSlider.Value = _mediaPlayer.Volume;
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\low_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Low Volume";
            }
            else if (_mediaPlayer.Volume == 0.5)
            {
                _mediaPlayer.Volume = 1.0; // Set to max volume
                VolumeSlider.Value = _mediaPlayer.Volume;
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\max_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Max Volume";
            }
            else
            {
                _mediaPlayer.Volume = 0;
                VolumeSlider.Value = _mediaPlayer.Volume;
                VolumeIcon.Source = new BitmapImage(new Uri(@"..\Resources\Images\mute_volume.png", UriKind.Relative));
                VolumeButton.ToolTip = "Audio: Muted";
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _songSerivce.ImportSongs();
            }
            catch (Exception)
            {
                MessageBox.Show("Add song unsuccess", "Ok", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayerView videoPlayerView = new VideoPlayerView();
            if(_isPlaying)
            {
                _mediaPlayer.Pause();
                _isPlaying = false;
            }
            this.Hide();
            videoPlayerView.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Song songToDelete)
            {
                if(_currentSongIndex >= 0 && _songSerivce.Songs.IndexOf(songToDelete) == _currentSongIndex)
                {
                    _mediaPlayer.Stop();
                    _timer.Stop();
                    _isPlaying = false;

                    TrackTitleTextBlock.Text = string.Empty;
                    ArtistTitleTextBox.Text = string.Empty;
                    DurationTextBlock.Text = "00:00"; 
                    DurationSlider.Value = 0.0;
                    CurrentPositionTextBlock.Text = "00:00";
                }
                _songSerivce.DeleteSong(songToDelete);
                PlayButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"..\Resources\Images\play.png", UriKind.Relative)),
                    Width = 24,
                    Height = 24
                };
                _musicService.DeleteAndChangeTotalSong("songPath.json", _songSerivce.Songs);
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
