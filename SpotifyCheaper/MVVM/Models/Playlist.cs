namespace SpotifyCheaper.Models
{
    using SpotifyCheaper.MVVM.Models;
    using System.Collections.ObjectModel;

    public class Playlist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ObservableCollection<Song> Tracks { get; set; } = new ObservableCollection<Song>();
    }

}
