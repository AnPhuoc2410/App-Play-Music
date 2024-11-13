using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class SongsService
    {
        private readonly FileService _fileService;
        private readonly MusicService _musicService;

        public ObservableCollection<Song> Songs { get; private set; } = new();
        private int _songIndex = 1;

        public SongsService(FileService fileService, MusicService musicService)
        {
            _fileService = fileService;
            _musicService = musicService;
            LoadSongsFromJson();
        }

        public void LoadSongsFromJson()
        {
            string path = "songPath.json";
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (File.Exists(fullPath))
            {
                var errorList = new List<int>();
                string totalSongs = _fileService.OutJsonValue(path, "TotalSong");

                if (totalSongs != null)
                {
                    Songs = _musicService.GetMp3List(path, int.Parse(totalSongs), out errorList);

                    if (errorList.Count > 0)
                    {
                        _musicService.DeleteAndChangeTotalSong(path, Songs);
                        totalSongs = _fileService.OutJsonValue(path, "TotalSong");
                    }

                    _songIndex = int.Parse(totalSongs) + 1;
                }
            }
        }

        public void ImportSongs()
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "MP3 Files | *.mp3"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    var metadata = _musicService.GetMp3Metadata(filePath);
                    if (metadata != null)
                    {
                        Songs.Add(new Song
                        {
                            TrackNumber = _songIndex,
                            Title = metadata.Title,
                            Artist = metadata.Artist,
                            Duration = metadata.Duration,
                            FilePath = filePath,
                        });
                        _songIndex++;
                    }
                }

                SaveSongs();
            }
        }

        private void SaveSongs()
        {
            var jsonListSong = JObject.Parse(_musicService.SongListToString(Songs));
            jsonListSong["TotalSong"] = _songIndex - 1;
            bool success = _fileService.InputJson("songPath.json", jsonListSong.ToString());

            if (!success)
            {
                throw new Exception("Failed to save songs to JSON.");
            }
        }

        public void DeleteSong(Song songToDelete)
        {
            if (songToDelete != null)
            {
                Songs.Remove(songToDelete);
            }
        }
        public ObservableCollection<Song> FindMusic(string inSearching)
        {
            ObservableCollection<Song> outListSong = new ObservableCollection<Song>();
            if (string.IsNullOrWhiteSpace(inSearching))
            {
                return Songs;
            }
            foreach (var song in Songs)
            {
                if (song.Title.ToLower().Contains(inSearching.ToLower()))
                {
                    outListSong.Add(song);
                }
            }

            return outListSong;
        }
    }
}
