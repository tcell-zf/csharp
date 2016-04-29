using System;
using System.IO;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

using TCell.Abstraction;

namespace TCell.UniversalMediaPlayer
{
    public partial class MainWindow : Window, IPlayerHostable
    {
        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private List<IPlayable> players = null;
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfigurations();
            LoadPlayers();
        }
        #endregion

        #region private functions
        private void LoadConfigurations()
        {
            string str = ConfigurationManager.AppSettings["title"];
            if (!string.IsNullOrEmpty(str))
                Title = str;

            str = ConfigurationManager.AppSettings["backgroundImageUri"];
            if (!string.IsNullOrEmpty(str) && File.Exists(str))
                backgroundImageBrush.ImageSource = BitmapFrame.Create(new Uri(str));
        }

        private bool LoadPlayers()
        {
            return true;
        }
        #endregion
    }
}
