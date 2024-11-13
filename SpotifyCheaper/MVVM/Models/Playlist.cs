namespace SpotifyCheaper.Models
{
    using SpotifyCheaper.MVVM.Models;
    using System.Collections.ObjectModel;

    public class Playlist
    {
        public string? Name { get; set; }
        public ObservableCollection<Song> Tracks { get; set; } = new ObservableCollection<Song>();

        public ObservableCollection<Video> Videos { get; set; } = new ObservableCollection<Video>();

    }

}
