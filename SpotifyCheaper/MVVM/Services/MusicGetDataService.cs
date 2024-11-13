using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicGetDataService
    {
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

                // Get the title, duration, and artist from the MP3 file
                string title = Path.GetFileName(filePath);
                TimeSpan duration = file.Properties.Duration;
                string artist = file.Tag.FirstPerformer ?? "Unknown Artist";

                // Get the album cover image if available
                byte[] imageData = null;
                if (file.Tag.Pictures.Length > 0)
                {
                    var picture = file.Tag.Pictures[0];
                    imageData = picture.Data.Data;
                }

                return new Song
                {
                    TrackNumber = 1,
                    Title = title,
                    Duration = duration.ToString(@"mm\:ss"),
                    Artist = artist,
                    AlbumCoverImage = imageData
                };
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP3 metadata: " + ex.Message);
                return null;
            }
        }
        public BitmapImage ConvertToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

    }
}
