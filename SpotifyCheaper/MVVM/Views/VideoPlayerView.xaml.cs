using Microsoft.Win32;
using SpotifyCheaper.MVVM.Services;
using System.Windows;
using System.Windows.Media;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : Window
    {
        private MusicService _musicService;
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


        private void DurationBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging)  // Only seek if not dragging
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(DurationBar.Value);
            }
        }
    }
}
