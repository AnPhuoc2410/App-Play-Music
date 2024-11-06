using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Services;
using SpotifyCheaper.ViewModels;
using System.Collections.ObjectModel;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpotifyCheaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer _mediaPlayer = new();
        private bool _isPlaying = false;
        private ObservableCollection<Song> _song;
        private DispatcherTimer _timer;
        private MusicGetDataService _musicService;
        public MainWindow()
        {
            InitializeComponent();
            LoadSongs();
            SongListView.ItemsSource = _song;

            _mediaPlayer.Open(new Uri(@"E:\FPT\FA24_Term5\PRN212\SpotifyCheaper\SpotifyCheaper\MVVM\Resources\Music\thucgiac.mp3"));

            // Initialize services and timer
            _musicService = new();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }
        private void LoadSongs()
        {
            _song = new ObservableCollection<Song>();
            {
                new Song { Title = "Jabulani - PJ Powers", Duration = "4:59" };
                new Song { Title = "Yego - PAPA JONES", Duration = "4:00" };
                new Song { Title = "Kuliko Jana - Sauti Sol", Duration = "4:31" };
                new Song { Title = "So Alive - SAGE Ft. OCTO", Duration = "4:59" };
                new Song { Title = "La La La - Naughty Boy", Duration = "3:59" };
            }
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Play();
            LoadMp3(@"E:\FPT\FA24_Term5\PRN212\SpotifyCheaper\SpotifyCheaper\MVVM\Resources\Music\thucgiac.mp3");
            _isPlaying = true;
        }

        

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying) {
                _mediaPlayer.Pause();
                _isPlaying = false;
            }
            else
            {
                _mediaPlayer.Play();
                _isPlaying = true;
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = (double)e.NewValue;
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer.Volume > 0)
            {
                _mediaPlayer.Volume = 0;
                VolumeSlider.Value = _mediaPlayer.Volume;
            }
            else
            {
                _mediaPlayer.Volume = 0.5;
                VolumeSlider.Value = _mediaPlayer.Volume;
            }
        }
        private void LoadMp3(string filePath)
        {
            // Use Mp3MetadataService to get metadata
            var metadata = _musicService.GetMp3Metadata(filePath);
            if (metadata != null)
            {
                TrackTitleTextBlock.Text = metadata.Title;
                DurationSlider.Maximum = metadata.Duration.TotalSeconds;
                DurationTextBlock.Text = metadata.Duration.ToString(@"mm\:ss");

                // Play the MP3 file
                _mediaPlayer.Open(new Uri(filePath));
                _mediaPlayer.Play();

                // Start the timer
                _timer.Start();
            }
            else
            {
                MessageBox.Show("Could not retrieve MP3 metadata.");
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.Position < _mediaPlayer.NaturalDuration.TimeSpan)
            {
                DurationSlider.Value = _mediaPlayer.Position.TotalSeconds;
                CurrentPositionTextBlock.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
            }
            else
            {
                _timer.Stop();
            }
        }
    }
}