﻿using Newtonsoft.Json;
using SpotifyCheaper.MVVM.Services;
using System.Net.Http;
using System.Text;

namespace SpotifyCheaper.MVVM.Repositories
{

    public class SpotifyRepository
    {

        private FileService jSonService;
        private readonly HttpClient client;
        //public string StringToken ()
        //{
        //    jSonService = new JSonService ();
        //    string keyFile = "appsettings.json";
        //    string sGetID = "SpotifySettings:ClientID";
        //    string sGetSecretKey = "SpotifySettings:SercetKeyID";

        //    string client_id = jSonService.OutJson(keyFile, sGetID);
        //    string sercet_key_id = jSonService.OutJson (keyFile, sGetSecretKey);

        //    // Convert String to UTF-8 bytes
        //    byte[] utf8Bytes = Encoding.UTF8.GetBytes(client_id + ":" + sercet_key_id);
        //    string enscryptedID = Convert.ToBase64String (utf8Bytes);

        //}
        public async Task<string> StringToken()
        {
            jSonService = new FileService();
            string sKeyFile = "appsettings.json";
            string sGetID = "SpotifySettings:ClientID";
            string sGetSecretKey = "SpotifySettings:SercetKeyID";

            string sClientID = jSonService.OutJsonValue(sKeyFile, sGetID);
            string sSecretKeyID = jSonService.OutJsonValue(sKeyFile, sGetSecretKey);

            // Convert key to Base64
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(sClientID + ":" + sSecretKeyID);
            string encodedCredentials = Convert.ToBase64String(utf8Bytes);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedCredentials);
            var requestContent = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "client_credentials")
    });

            HttpResponseMessage response = await client.PostAsync("https://accounts.spotify.com/api/token", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                return jsonResponse.access_token;
            }
            else
            {
                throw new Exception("Failed to retrieve Spotify token: " + response.ReasonPhrase);
            }
        }


        public HttpClient GetHeader(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
            return client;
        }

        public async Task<string> SearchTrackAsync(string query, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                string url = $"https://api.spotify.com/v1/search?q={query}&type=track&limit=10&include_external=audio";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent; // You can parse this JSON as needed
                }
                else
                {
                    throw new Exception("Failed to search for track: " + response.ReasonPhrase);
                }
            }
        }

    }
}
