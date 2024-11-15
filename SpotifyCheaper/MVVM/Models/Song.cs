﻿namespace SpotifyCheaper.MVVM.Models
{
    public class Song
    {
        public int TrackNumber { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; }
        public string FilePath { get; set; }
         public byte[] AlbumArt { get; set; }
        public string ToString()
        {
            return Title + "|" + FilePath;
        }
    }

}
