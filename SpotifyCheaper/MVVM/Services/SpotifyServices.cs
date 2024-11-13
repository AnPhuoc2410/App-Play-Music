using SpotifyCheaper.MVVM.Repositories;

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