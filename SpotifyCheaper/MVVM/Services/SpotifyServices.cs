using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class SpotifyServices
    {
        private SpotifyRepository spotifyRepository;

        public string GetToken()
        {
            spotifyRepository = new SpotifyRepository();
            return spotifyRepository.StringToken();
        }
    }
}