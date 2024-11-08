using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Repositories
{
    public class SpotifyRepository
    {
        private readonly IConfiguration _configuration;
        private string clientId;
        private string clientSecret;

        public SpotifyRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            clientId = _configuration["SpotifyID:ClientID"];
            clientSecret = _configuration["SpotifyID:ClientSecret"];
        }
        public static async Task<string> GetSpotifyAccessToken(string clientId, string clientSecret)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(json);
                    return tokenResponse?.AccessToken;
                }
                else
                {
                    Console.WriteLine("Error retrieving access token");
                    return null;
                }
            }
        }

    }
    public class TokenResponse
    {
        public string AccessToken { get; set; }
    }
}
