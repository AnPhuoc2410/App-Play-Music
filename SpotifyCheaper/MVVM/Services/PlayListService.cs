using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class PlayListService
    {
        public readonly string playlistFile = Directory.GetCurrentDirectory()+ "\\" + "playList.json";

        private FileRepository  _fileRepository = new();   
        private MusicService _musicService = new MusicService();

        public string GetPlaylistName(string key)
        {
            try
            { 
                return _fileRepository.GetJsonFile(playlistFile, key);    
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string GetCountPlaylist()
        {
            int id = 1;
            try
            {
                if (!File.Exists(playlistFile))
                {
                    JObject jsonDefaultPlayList = new JObject();
                    jsonDefaultPlayList[id.ToString()] = "DefaultPlaylist";
                    jsonDefaultPlayList["TotalPlaylist"] = id;
                    File.WriteAllText(playlistFile, jsonDefaultPlayList.ToString());
                    return id.ToString();
                }
                else
                {
                    return _fileRepository.GetJsonFile(playlistFile, "TotalPlaylist");
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        // Doc tu file cu
        // Minh se chuyen gia tri play list vao => biet duoc id play list roi, thu can la get name, list song.
        public bool CreatePlayList (string file, int id, string playListName)
        {
            if (id == 0)
            {
                id = 1;
            }
            try
            {
                if (File.Exists(playlistFile))
                {
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(playlistFile);
                    jObject[id] = playListName;
                    string getTotalPlaylist = jObject.GetValue("TotalPlaylist").ToString();
                    jObject["TotalPlaylist"] = int.Parse(getTotalPlaylist) + 1;
                    File.WriteAllText(playlistFile, jObject.ToString());
                    return true;
                }
                else
                {
                    JObject jsonDefaultPlayList = new JObject();
                    jsonDefaultPlayList[id] = "DefaultPlaylist";
                    jsonDefaultPlayList["TotalPlaylist"] = id;
                    File.WriteAllText(playlistFile, jsonDefaultPlayList.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Get total playlist from json file. 
        /// Get id, playlist name from playList.json.
        /// So when people go to each playlist, we will load the tracks from id playlist.
        /// </summary>
        /// <param name="totalPlayList"></param> Not hardcore so can easily to reusable
        /// <returns></returns>
        public ObservableCollection<Playlist> GetTotalPlaylist(int totalPlayList)
        {
            ObservableCollection<Playlist> list = new ObservableCollection<Playlist>();
            for (int i =1 ; i <= totalPlayList; i++)
            {
                string playlistName =GetPlaylistName(i.ToString());
                if (playlistName != "")
                {

                    Playlist playlist = new Playlist()
                    {
                        Id = i,
                        Name = playlistName
                    };
                    list.Add(playlist);
                }
            }
            return list;
        }
    }
}
