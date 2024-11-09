using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using SpotifyCheaper.MVVM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicService
    {
        private JsonRepository jsonRepository;

        // <summary>
        /// Gets the metadata (title and duration) of an MP3 file.
        /// </summary>
        /// <param name="filePath">The path to the MP3 file.</param>
        /// <returns>An Mp3Metadata object containing the title and duration of the MP3 file.</returns>
        public Song GetMp3Metadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                // Get the title and duration from the MP3 file
                string title = Path.GetFileName(filePath);
                TimeSpan duration = file.Properties.Duration;
                string artist = file.Tag.FirstPerformer ?? "Unknown Artist";

                return new Song
                {
                    TrackNumber = 1,
                    Title = title,
                    Duration = duration.ToString(@"mm\:ss"),
                    Artist = artist
                    // Khong biet sao Phuoc ko import filePath ma van chay dc?
                };
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP3 metadata: " + ex.Message);
                return null;
            }
        }

        public ObservableCollection<Song> GetMp3List(string filePath, int number, out List<int> ErrorSongIndex)
        {
            ErrorSongIndex = new();
            JSonService jSonService = new JSonService();

            var listSong = new ObservableCollection<Song>();
            try
            {
                int iTrackNumber = 1;
                for (int i = 1; i <= number; i++)
                {
                    string path = jSonService.OutJsonValue(filePath,i.ToString());
                    
                    // Check if the song exist
                    if (File.Exists(path))
                    {
                        
                        var file = TagLib.File.Create(path);
                        // Get the title and duration from the MP3 file
                        string title = Path.GetFileName(path);
                        TimeSpan duration = file.Properties.Duration;
                        string artist = file.Tag.FirstPerformer ?? "Unknown Artist";
                        Song sSong = new Song
                        {
                            TrackNumber = iTrackNumber++,
                            Title = title,
                            Duration = duration.ToString(@"mm\:ss"),
                            Artist = artist,
                            FilePath = path
                        };
                        listSong.Add(sSong);                        
                    }
                    else
                    {
                        ErrorSongIndex.Add(i);
                    }
                }
                return listSong;
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP3 metadata: " + ex.Message);
                return listSong;
            }
        }

        public string GetListSongString(Playlist inPlayList)
        {
            var _songs = inPlayList.Tracks;
            Dictionary<string, string> songPathDictionary = new();
            int count = 1;

            foreach (var song in _songs)
            {
                songPathDictionary[count++.ToString()] = song.FilePath;
            }
            // Add total song number
            songPathDictionary["TotalSong"] = _songs.Count.ToString();
            string s = JsonConvert.SerializeObject(songPathDictionary, Formatting.Indented);
            return s;
        }

        public void DeleteAllErrorSong (string file, List<int> errorList)
        {
            jsonRepository = new();
            foreach (var error in errorList)
            {
                jsonRepository.DeleteJsonValue(file, error.ToString());
            }
        }

        public void DeleteAndChangeTotalSong (string file, List<int> errorList, string key, string value)
        {
            jsonRepository = new();
            foreach (var error in errorList)
            {
                // Lay file co index lon nhat             
                string sTotalSong = jsonRepository.GetJsonFile(file, "TotalSong");
                string endListSong = jsonRepository.GetJsonFile(file, sTotalSong);

                // Toi index bi loi, thay bang gia tri index lon nhat xong delete gia tri lon nhat do
                jsonRepository.ChangeJsonKeyValue(file, error.ToString(), endListSong);
                jsonRepository.DeleteJsonValue(file, endListSong);
                jsonRepository.ChangeJsonKeyValue(file, key, value);

            }
          
        }

        public bool AddSong (string file, string key, string value)
        {
            jsonRepository =new();
            string sTotalSong = jsonRepository.GetJsonFile(file, "TotalSong");
            jsonRepository.ChangeJsonKeyValue(file, "TotalSong", (int.Parse(sTotalSong)+1).ToString());
            return jsonRepository.AddJsonValue(file, key, value);
        }

        public string RetrieveTotalSongFromSongList(ObservableCollection<Song> songList)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            int count = 1;

            foreach (Song song in songList)
            {
                dict.Add(count++.ToString(), song.FilePath);
            }
            string sListSong = JsonConvert.SerializeObject(dict, Formatting.Indented);
            return sListSong;
        }
    }
}
