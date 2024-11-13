using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.ViewModels;
using System.Collections.ObjectModel;

namespace SpotifyCheaper.ViewModels
{


    public class PlaylistViewModel : ViewModelBase
    {
        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();

        public void AddPlaylist(string name)
        {
            Playlists.Add(new Playlist { Name = name });
        }

        public void RemovePlaylist(Playlist playlist)
        {
            Playlists.Remove(playlist);
        }
    }

}
