using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpotifyCheaper.MVVM.Services
{
    public class VideoService
    {
        private FileService _fileService = new();

        public ObservableCollection<Video> Videos { get; private set; } = new();
        private int _videoIndex = 1;


        public VideoService(FileService fileService)
        {
            _fileService = fileService;
        }

        public Video GetMp4Metadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                // Get the title and duration from the MP3 file
                string title = Path.GetFileName(filePath);
                TimeSpan duration = file.Properties.Duration;

                return new Video
                {
                    VideoNumber = 1,
                    Title = title,
                    Duration = duration.ToString(@"mm\:ss"),
                    // Khong biet sao Phuoc ko import filePath ma van chay dc?
                };
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP4 metadata: " + ex.Message);
                return null;
            }
        }

        public void ImportVideos()
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Video Files|*.mp4;*.avi;*.mkv;*.wmv|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    var metadata = GetMp4Metadata(filePath);
                    if (metadata != null)
                    {
                        Videos.Add(new Video
                        {
                            VideoNumber = _videoIndex,
                            Title = metadata.Title,
                            Duration = metadata.Duration,
                            FilePath = filePath,
                        });
                        _videoIndex++;
                    }
                }

                SaveSongs();
            }
        }
        private void SaveSongs()
        {
            var jsonListVideos = JObject.Parse(SongListToString(Videos));
            jsonListVideos["TotalVideo"] = _videoIndex - 1;
            bool success = _fileService.InputJson("videoList.json", jsonListVideos.ToString());

            if (!success)
            {
                throw new Exception("Failed to save videos to JSON.");
            }
        }

        public string SongListToString(ObservableCollection<Video> videoList)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            int count = 1;

            foreach (Video video in videoList)
            {
                dict.Add(count++.ToString(), video.FilePath);
            }
            string sListVideo = JsonConvert.SerializeObject(dict, Formatting.Indented);
            return sListVideo;
        }
    }
}
