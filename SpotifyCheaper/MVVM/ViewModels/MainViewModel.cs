using SpotifyCheaper.MVVM.ViewModels;

namespace SpotifyCheaper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public PlaylistViewModel PlaylistVM { get; set; } = new PlaylistViewModel();
        public PlayerViewModel PlayerVM { get; set; } = new PlayerViewModel();

    }

}
