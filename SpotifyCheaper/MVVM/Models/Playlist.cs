using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.Models
{
    using SpotifyCheaper.MVVM.Models;
    using System.Collections.ObjectModel;

    public class Playlist
    {
        public string? Name { get; set; }
        public ObservableCollection<Song> Tracks { get; set; } = new ObservableCollection<Song>();
    }

}
