using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.Models
{
    using System.Collections.ObjectModel;

    public class Playlist
    {
        public string? Name { get; set; }
        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
    }

}
