using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
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
        private FileRepository fileRepository = new();

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

                SaveVideos();
            }
        }
        private void SaveVideos()
        {
            var jsonListVideos = JObject.Parse(VideoListToString(Videos));
            jsonListVideos["TotalVideo"] = _videoIndex - 1;
            bool success = _fileService.InputJson("videoList.json", jsonListVideos.ToString());

            if (!success)
            {
                throw new Exception("Failed to save videos to JSON.");
            }
        }

        public string VideoListToString(ObservableCollection<Video> videoList)
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
        public ObservableCollection<Video> GetMp4List(string filePath, int number, out List<int> ErrorSongIndex)
        {
            ErrorSongIndex = new();
            FileService fileService = new FileService();

            var listVideo = new ObservableCollection<Video>();
            try
            {
                int iVideoNumber = 1;
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
                        Video sVideo = new Video
                        {
                            VideoNumber = iVideoNumber++,
                            Title = title,
                            Duration = duration.ToString(@"mm\:ss"),
                            FilePath = path
                        };
                        listVideo.Add(sVideo);
                    }
                    else
                    {
                        ErrorSongIndex.Add(i);
                    }
                }
                return listVideo;
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP4 metadata: " + ex.Message);
                return listVideo;
            }
        }
        public bool DeleteAndChangeTotalSong(string file, ObservableCollection<Video> videoList)
        {
            string sVideoList = VideoListToString(videoList);
            JObject jObjectList = JObject.Parse(sVideoList);
            jObjectList["TotalVideo"] = videoList.Count;
            return fileRepository.InputJsonFile(file, jObjectList.ToString());
        }
        public void LoadVideosFromJson()
        {
            string path = "videoList.json";
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

            if (File.Exists(fullPath))
            {
                var errorList = new List<int>();
                string totalVideos = _fileService.OutJsonValue(path, "TotalVideo");

                if (totalVideos != null)
                {
                    Videos = GetMp4List(path, int.Parse(totalVideos), out errorList);

                    if (errorList.Count > 0)
                    {
                        DeleteAndChangeTotalSong(path, Videos);
                        totalVideos = _fileService.OutJsonValue(path, "TotalVideo");
                    }

                    _videoIndex = int.Parse(totalVideos) + 1;
                }
            }
        }
    }
}
