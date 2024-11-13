using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using System.Collections.ObjectModel;
using System.IO;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicService
    {
        private FileRepository fileRepository = new();

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
                byte[] albumArt = null;
                if (file.Tag.Pictures.Length > 0)
                {
                    var picture = file.Tag.Pictures[0];
                    albumArt = picture.Data.Data;
                }
                return new Song
                {
                    TrackNumber = 1,
                    Title = title,
                    Duration = duration.ToString(@"mm\:ss"),
                    Artist = artist,
                    AlbumArt = albumArt
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
            FileService fileService = new FileService();

            var listSong = new ObservableCollection<Song>();
            try
            {
                int iTrackNumber = 1;
                for (int i = 1; i <= number; i++)
                {
                    string path = fileService.OutJsonValue(filePath, i.ToString());

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

        public void DeleteAllErrorSong(string file, List<int> errorList)
        {
            fileRepository = new();
            foreach (var error in errorList)
            {
                fileRepository.DeleteJsonValue(file, error.ToString());
            }
        }

        //Ver 1
        public void DeleteAndChangeTotalSong(string file, List<int> errorList, string key, string value)
        {

            foreach (var error in errorList)
            {
                // Lay file co index lon nhat             
                string sTotalSong = fileRepository.GetJsonFile(file, "TotalSong");
                string endListSong = fileRepository.GetJsonFile(file, sTotalSong);

                // Toi index bi loi, thay bang gia tri index lon nhat xong delete gia tri lon nhat do
                fileRepository.ChangeJsonKeyValue(file, error.ToString(), endListSong);
                fileRepository.DeleteJsonValue(file, endListSong);
                fileRepository.ChangeJsonKeyValue(file, key, value);

            }
            ///////////////////////////////////////



        }

        // Ver 2
        // Tinh lam lai y chang ver 1 nma them mot it condition. Nma chot nghi ra ver 3 make sense hon.
        public void DeleteAndChangeTotalSong(string file, List<int> errorList, ObservableCollection<Song> _songs)
        {
            string sTotalSong = _songs.Count.ToString();
            string endListSong = fileRepository.GetJsonFile(file, sTotalSong); // Lay bai hat cuoi cung
            for (int i = errorList.Count - 1; i >= 0; i++)
            {
                string error = errorList[i].ToString();

            }
        }

        // Ver 3
        public bool DeleteAndChangeTotalSong(string file, ObservableCollection<Song> songList)
        {
            string sSongList = SongListToString(songList);
            JObject jObjectList = JObject.Parse(sSongList);
            jObjectList["TotalSong"] = songList.Count;
            return fileRepository.InputJsonFile(file, jObjectList.ToString());
        }

        public bool AddSong(string file, string key, string value)
        {
            string sTotalSong = fileRepository.GetJsonFile(file, "TotalSong");
            fileRepository.ChangeJsonKeyValue(file, "TotalSong", (int.Parse(sTotalSong) + 1).ToString());
            return fileRepository.AddJsonValue(file, key, value);
        }

        public string SongListToString(ObservableCollection<Song> songList)
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
