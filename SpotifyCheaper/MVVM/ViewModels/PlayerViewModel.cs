using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        public Track CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsShuffle { get; set; }

        public void Play() { /* Logic to start playing */ }
        public void Pause() { /* Logic to pause */ }
        public void ToggleShuffle() { IsShuffle = !IsShuffle; }
    }

}
