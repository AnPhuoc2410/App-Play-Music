using SpotifyCheaper.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Repositories
{

    public class SpotifyRepository
    {
        private JSonService jSonService;

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
        public string StringToken()
        {
            jSonService = new JSonService();
            string keyFile = "appsettings.json";
            string sGetID = "SpotifySettings:ClientID";
            string sGetSecretKey = "SpotifySettings:SercetKeyID";

            string client_id = jSonService.OutJson(keyFile, sGetID);
            string secret_key_id = jSonService.OutJson(keyFile, sGetSecretKey);

            // Convert client_id and secret_key to Base64
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(client_id + ":" + secret_key_id);
            string encodedCredentials = Convert.ToBase64String(utf8Bytes);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedCredentials);

                var requestContent = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

                HttpResponseMessage response = client.PostAsync("https://accounts.spotify.com/api/token", requestContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                    return jsonResponse.access_token;
                }
                else
                {
                    throw new Exception("Failed to retrieve Spotify token: " + response.ReasonPhrase);
                }
            }
        }

    }
}
