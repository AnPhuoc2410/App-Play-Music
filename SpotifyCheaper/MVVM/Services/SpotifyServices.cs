using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Repositories;

namespace SpotifyCheaper.MVVM.Services
{
    public class SpotifyServices
    {
        private SpotifyRepository spotifyRepository = new SpotifyRepository();
        private string currentToken ="";
        public async Task<string> GetToken()
        {
            return await spotifyRepository.StringToken();
        }

        public void SetToken(string token)
        {
            currentToken = token;
        }

        public async Task<string> GetImageAsync(string songName)
        { 
            currentToken = await GetToken();
            var response = await spotifyRepository.SearchTrackAsync(songName, currentToken);
            JObject jObject = JObject.Parse(response);

            try
            {
                string imageUrl = jObject["tracks"]["items"][0]["album"]["images"][0]["url"].ToString();
                return imageUrl;
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}