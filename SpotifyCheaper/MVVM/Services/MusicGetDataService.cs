using SpotifyCheaper.Models;
using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // Get the title and duration from the MP3 file
                string title = Path.GetFileName(filePath);
                TimeSpan duration = file.Properties.Duration;
                string artist = file.Tag.FirstPerformer ?? "Unknown Artist";

                return new Song
                {
                    TrackNumber = 1,
                    Title = title,
                    Duration = duration,
                    Artist = artist
                };
            }
            catch (Exception ex)
            {
                // Handle or log the error as needed
                Console.WriteLine("Error retrieving MP3 metadata: " + ex.Message);
                return null;
            }
        }
    }
}
