using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyCheaper.Models;
using System.Collections.ObjectModel;
using SpotifyCheaper.MVVM.ViewModels;

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
