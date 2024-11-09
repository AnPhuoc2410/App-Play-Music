using Microsoft.Win32;
using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicButtonService
    {
        private MusicGetDataService _musicService = new();
        private ObservableCollection<Song> _songs = new();
        private int _songIndex = 1;


        public void ImportData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 Files | *.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {

                    var metadata = _musicService.GetMp3Metadata(filePath);
                    if (metadata != null)
                    {
                        _songs.Add(new Song
                        {
                            TrackNumber = _songIndex,
                            Title = metadata.Title,
                            Artist = metadata.Artist,
                            Duration = metadata.Duration,
                            FilePath = filePath  // Store the file path
                        });
                        _songIndex++;
                    }
                    else
                    {
                        MessageBox.Show($"Could not retrieve MP3 metadata for {filePath}.");
                    }
                }
            }
        }
        public List<Song> GetSongList()
        {
            return _songs.ToList();
        }
    }
}
