using Newtonsoft.Json;
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

        public ObservableCollection<Song> GetMp3List(string filePath, int number)
        {
            var listSong = new ObservableCollection<Song>();
            try
            {
                int iTrackNumber = 1;
                for (int i = 1; i <= number; i++)
                {
                    JSonService jSonService = new JSonService();
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


    }
}
