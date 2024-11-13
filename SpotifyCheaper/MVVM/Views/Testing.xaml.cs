using SpotifyCheaper.MVVM.Models;
using SpotifyCheaper.MVVM.Repositories;
using SpotifyCheaper.MVVM.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpotifyCheaper.MVVM.Views
{
    /// <summary>
    /// Interaction logic for Testing.xaml
    /// </summary>
    public partial class Testing : Window
    {
        private FileService jsonService;

        public ObservableCollection<Song> _songs { get; set; }
        public string testString { get; set; }
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
