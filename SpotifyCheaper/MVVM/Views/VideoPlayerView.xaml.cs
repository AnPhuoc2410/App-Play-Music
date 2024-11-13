using Microsoft.Win32;
using SpotifyCheaper.MVVM.Services;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using SpotifyCheaper.MVVM.Services;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : Window
    {
        private MusicService _musicService;
        private DispatcherTimer _timer;
        private MediaPlayer _mediaPlayer;
        private bool _isPlaying = false;
        private bool _isDragging = false;
        public VideoPlayerView()
        {
            InitializeComponent();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();

            Window.GetWindow(this)?.Close();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {

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

        }

        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.wmv|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                mediaElement.Source = new Uri(openFileDialog.FileName);
                mediaElement.Play();
                _isPlaying = true;
                PlayButton.Content = "⏸️";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan && mediaElement.Position < mediaElement.NaturalDuration.TimeSpan)
            {
                DurationBar.Value = _mediaPlayer.Position.TotalSeconds;
                CurrentPositionTextBlock.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
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
    }
}
