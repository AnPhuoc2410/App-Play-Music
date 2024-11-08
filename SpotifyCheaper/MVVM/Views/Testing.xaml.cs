﻿using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
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
        private SpotifyRepository spotifyRepository;
        public Testing()
        {
            InitializeComponent();
        }

        private void Testing_Loaded(object sender, RoutedEventArgs e)
        {
            spotifyRepository = new SpotifyRepository();
            TestingTextBox.Text = spotifyRepository.StringToken();
        }
    }
}
