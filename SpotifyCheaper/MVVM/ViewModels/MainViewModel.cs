using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SpotifyCheaper.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Playlist> _playlist;
        public ObservableCollection<Playlist> Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        public MainViewModel()
        {
            // Sample data; replace with your actual data source
            MainWindow mainWindow = new MainWindow();
            var inPlaylist = mainWindow.currentPlaylist;
            Playlist = inPlaylist;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }

}
