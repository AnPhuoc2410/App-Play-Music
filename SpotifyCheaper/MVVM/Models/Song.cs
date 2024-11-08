﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Models
{
    public class Song
    {
        public int TrackNumber { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; } // Example: "3:57"
    }

}