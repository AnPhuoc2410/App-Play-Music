using SpotifyCheaper.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public PlaylistViewModel PlaylistVM { get; set; } = new PlaylistViewModel();
        public PlayerViewModel PlayerVM { get; set; } = new PlayerViewModel();
    }

}
