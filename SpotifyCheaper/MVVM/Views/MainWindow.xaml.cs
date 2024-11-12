using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.MVVM.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
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
        private FileService fileService = new();

        private MediaPlayer _mediaPlayer = new();
        private bool _isPlaying = false;
        private ObservableCollection<Song> _songs = new();
        private DispatcherTimer _timer;
        private MusicService _musicService = new();
        private int _currentSongIndex = -1;
        private bool _isShuffling = false;
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

           
            List<int> lErrorList;

            //Get Path
            // Sau nay thay songPath = playList.Name;
            string path = "songPath.json";
            string fullPath = Directory.GetCurrentDirectory() + "\\" + path;
            // Check File co hay ko neu ko co thi tao.
            if (File.Exists(fullPath))
            {
                // Doc tat ca cac bai hat tren thu muc xong roi ghi ra cac bai bi loi ko import dc
                string GetTotalSongInFile = fileService.OutJsonValue("songPath.json", "TotalSong");
                if (GetTotalSongInFile != null)
                {
                    _songs = _musicService.GetMp3List(path, int.Parse(GetTotalSongInFile), out lErrorList);
                    //Delete cac file bi loi
                    if (lErrorList.Count != 0) _musicService.DeleteAndChangeTotalSong(path, _songs);

                    // Lay tong cac bai sau khi chinh tong so bai lai
                    GetTotalSongInFile = fileService.OutJsonValue("songPath.json", "TotalSong");
                    _songIndex = int.Parse(GetTotalSongInFile) + 1;
                }
            }
            
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
            if (_isShuffling && _songs.Count > 1)
            {
                PlayRandomSong();
                SongListView.SelectedIndex = _currentSongIndex;
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
                        var song =new Song
                        {
                            TrackNumber = _songIndex,
                            Title = metadata.Title,
                            Artist = metadata.Artist,
                            Duration = metadata.Duration,
                            FilePath = filePath  // Store the file path
                        };
                        _songs.Add(song);
                        _songIndex++;

                    }
              
           
                    else
                    {
                        MessageBox.Show($"Could not retrieve MP3 metadata for {filePath}.");
                    }
                }
            }
            string inS = _musicService.SongListToString(_songs);
            JObject jsonListSong = JObject.Parse(inS);
            jsonListSong["TotalSong"] = _songIndex-1;
            bool success = fileService.InputJson("songPath.json", jsonListSong.ToString());
            if (success)
            {
                MessageBox.Show("Add song success", "Ok", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else MessageBox.Show("Add song unsuccess", "Ok", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayerView videoPlayerView = new VideoPlayerView();
            this.Hide();
            videoPlayerView.ShowDialog();
            
        }

        private void SearchingButton_Click(object sender, RoutedEventArgs e)
        {
            string sSearchName = SearchingTextBox.Text;

        }

       
    }
}
