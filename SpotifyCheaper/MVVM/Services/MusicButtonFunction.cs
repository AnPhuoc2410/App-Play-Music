using SpotifyCheaper.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Services
{
    public class MusicButtonFunction
    {

        public ObservableCollection<Song> FindMusic(string inSeaching, ObservableCollection<Song> songList)
        {
            ObservableCollection<Song> outListSong = new ObservableCollection<Song>();
            string regexCondition = string.Empty;
            for (int i = 0; i < songList.Count; i++)
            {
                Song song = songList[i];
                if (song.Title.ToLower().Contains(inSeaching)) outListSong.Add(song);
            }
            return outListSong;
        }

    }
}
