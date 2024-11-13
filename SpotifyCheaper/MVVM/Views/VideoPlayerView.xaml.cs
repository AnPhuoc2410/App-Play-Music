using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Google.Apis.YouTube.v3;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.MVVM.Models;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : Window
    {
        
        private DispatcherTimer _timer;
        private bool _isPlaying = false;
        private bool _isDragging = false;
        private DispatcherTimer _mouseActivityTimer;
        private FileService _fileService = new();
        private VideoService _videoService;
        private bool _isRepeating = false;
        private bool _isFullScreen = false;
        private WindowState _previousWindowState;
        private Rect _previousWindowBounds;
        private DispatcherTimer _hideTimer;
        private bool _controlsVisible = true;
        private int _currentVideoIndex = -1;


        public VideoPlayerView()
        {
            InitializeComponent();
            _videoService = new VideoService(_fileService);
            mediaElement.MediaOpened += MediaElement_MediaOpened; // Register the event handler
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            InitializePlayer();

            // Initialize the timer for hiding controls
            _hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Adjust this duration as needed
            };
            _hideTimer.Tick += HideTimer_Tick;

            // Start by showing controls
            ShowControls();

        }



        private void InitializePlayer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }


        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan) // Check if media has a valid duration
            {
                double newPosition = mediaElement.Position.TotalSeconds - 10; // Calculate new position
                mediaElement.Position = TimeSpan.FromSeconds(Math.Max(newPosition, 0)); // Ensure not less than zero
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                mediaElement.Pause();
                PlayButton.Content = "⏯️";
            }
            else
            {
                mediaElement.Play();
                PlayButton.Content = "⏸️";
            }
            _isPlaying = !_isPlaying;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            // Fast-forward the video by 10 seconds
            if (mediaElement.NaturalDuration.HasTimeSpan) // Check if media has a valid duration
            {
                double newPosition = mediaElement.Position.TotalSeconds + 10; // Calculate new position
                double maxPosition = mediaElement.NaturalDuration.TimeSpan.TotalSeconds; // Get maximum duration
                mediaElement.Position = TimeSpan.FromSeconds(Math.Min(newPosition, maxPosition)); // Ensure not beyond total duration
            }
        }

        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {
            _isRepeating = !_isRepeating;
            LoopButton.Content = _isRepeating ? "🔁 (On)" : "🔁";
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _videoService.ImportVideos();
                MessageBox.Show("Add video success", "Ok", MessageBoxButton.OK, MessageBoxImage.None);
            }
            catch (Exception)
            {
                MessageBox.Show("Add video unsuccess", "Ok", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                // Set the total duration in the DurationTextBox
                TimeSpan totalDuration = mediaElement.NaturalDuration.TimeSpan;
                DurationBar.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                DurationTextBox.Text = totalDuration.ToString(@"hh\:mm\:ss");
            }
        }
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeating)
            {
                mediaElement.Position = TimeSpan.Zero;
                DurationBar.Value = mediaElement.Position.TotalSeconds;
                mediaElement.Play();
            }
            else
            {
                // Stop playback, reset media state, and update the UI
                mediaElement.Stop();
                mediaElement.Source = null; // Unset the source if you want to "close" the video

                _isPlaying = false;
                PlayButton.Content = "⏯️";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan && mediaElement.Position < mediaElement.NaturalDuration.TimeSpan)
            {
                DurationBar.Value = mediaElement.Position.TotalSeconds;
                CurrentPositionTextBlock.Text = mediaElement.Position.ToString(@"mm\:ss");
            }
            else
            {
                _timer.Stop();
                _isPlaying = false;
            }
        }


        private void DurationBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging)  // Only seek if not dragging
            {
                mediaElement.Position = TimeSpan.FromSeconds(DurationBar.Value);
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
            mediaElement.Position = TimeSpan.FromSeconds(newPositionInSeconds);
        }

        private void BtnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }
        private void ToggleFullScreen()
        {
            if (_isFullScreen)
            {
                // Exit full-screen mode
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = _previousWindowState;
                this.Top = _previousWindowBounds.Top;
                this.Left = _previousWindowBounds.Left;
                this.Width = _previousWindowBounds.Width;
                this.Height = _previousWindowBounds.Height;
                this.Topmost = false; // No longer keep above other windows
                ShowControls(); // Show controls upon exiting full screen
            }
            else
            {
                // Enter full-screen mode
                _previousWindowState = this.WindowState;
                _previousWindowBounds = new Rect(this.Left, this.Top, this.Width, this.Height);

                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true; // Keep the window above other windows
            }

            _isFullScreen = !_isFullScreen;
            _hideTimer.Start(); // Restart the timer to hide controls in full-screen mode
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isFullScreen)
            {
                // Only show controls and restart the timer when in full-screen mode
                ShowControls();
                _hideTimer.Stop();
                _hideTimer.Start();
            }
        }

        private void ShowControls()
        {
            if (!_controlsVisible)
            {
                ControlsPanel.Visibility = Visibility.Visible;
                _controlsVisible = true;
            }
        }

        private void HideControls()
        {
            if (_controlsVisible)
            {
                ControlsPanel.Visibility = Visibility.Hidden;
                _controlsVisible = false;
            }
        }

        private void HideTimer_Tick(object sender, EventArgs e)
        {
            // Hide the controls after the timer interval if in full-screen mode
            if (_isFullScreen)
            {
                HideControls();
            }
        }

        private void VideoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoListView.SelectedItem is Video selectedVideo)
            {
                _currentVideoIndex = _videoService.Videos.IndexOf(selectedVideo);
                PlaySelectedSong(selectedVideo);
            }
        }
        private void PlaySelectedSong(Video video)
        {
            string filePath = video.FilePath;  // Use the dynamic file path from the Song object

            mediaElement.Stop();
            _timer.Stop();

            var metadata = _videoService.GetMp4Metadata(filePath);
            if (metadata != null)
            {
                string durationString = metadata.Duration;
                TimeSpan duration = TimeSpan.ParseExact(durationString, @"mm\:ss", null);
                DurationBar.Maximum = duration.TotalSeconds;
                DurationTextBox.Text = metadata.Duration;

                mediaElement.Source = new Uri(filePath);
                mediaElement.Play();
                _isPlaying = true;
                PlayButton.Content = "⏸️";

                _timer.Start();
            }
            else
            {
                MessageBox.Show("Could not retrieve MP4 metadata.");
            }
        }
    }
}
