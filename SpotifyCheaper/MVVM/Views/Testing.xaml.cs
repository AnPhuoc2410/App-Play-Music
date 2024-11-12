using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using SpotifyCheaper.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for Testing.xaml
    /// </summary>
    public partial class Testing : Window
    {
        private FileService jsonService;

        public ObservableCollection<Song> _songs {  get; set; }
        public string testString {  get; set; }
        public Testing()
        {
            InitializeComponent();
        }

        private void Testing_Loaded(object sender, RoutedEventArgs e)
        {
            FileRepository jsonRepository = new();
            //string textBoxOut= testString;
            int i = 1;
            string textBoxOut = jsonRepository.GetJsonFile("songPath.json", i.ToString());
          
            jsonService = new();
            string path = "songPath.json";
            string GetTotalSongInFile = jsonService.OutJsonValue(path, "TotalSong");
            TestingTextBox.Text = GetTotalSongInFile;
        }
    }
}
