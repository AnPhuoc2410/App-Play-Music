using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.ViewModels;

namespace SpotifyCheaper.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        public Song CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsShuffle { get; set; }

        public void Play() { /* Logic to start playing */ }
        public void Pause() { /* Logic to pause */ }
        public void ToggleShuffle() { IsShuffle = !IsShuffle; }
    }

}
