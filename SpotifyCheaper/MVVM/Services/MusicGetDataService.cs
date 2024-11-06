using SpotifyCheaper.Models;
using System;
using System.Collections.Generic;
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
        public Track GetMp3Metadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                // Get the title and duration from the MP3 file
                string title = file.Tag.Title ?? "Unknown Title";
                TimeSpan duration = file.Properties.Duration;
                string artist = file.Tag.FirstPerformer ?? "Unknown Artist";

                return new Track
                {
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
