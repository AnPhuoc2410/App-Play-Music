using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SpotifyCheaper.MVVM.Services;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : Window
    {
        private MusicGetDataService _musicService;
        private MediaPlayer _mediaPlayer;
        private bool _isPlaying = false;
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

        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
