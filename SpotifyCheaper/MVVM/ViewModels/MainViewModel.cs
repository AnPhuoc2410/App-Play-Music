using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SpotifyCheaper.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _playlist;
        public ObservableCollection<string> Playlist
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
            Playlist = new ObservableCollection<string> { "Us-UK", "Nhạc Suy", "IF You miss" };
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }

}
