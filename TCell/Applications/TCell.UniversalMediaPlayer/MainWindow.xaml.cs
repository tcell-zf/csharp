using System;
using System.IO;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TCell.UniversalMediaPlayer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfigurations();
        }

        private void LoadConfigurations()
        {
            string str = ConfigurationManager.AppSettings["title"];
            if (!string.IsNullOrEmpty(str))
                Title = str;

            str = ConfigurationManager.AppSettings["backgroundImageUri"];
            if (!string.IsNullOrEmpty(str) && File.Exists(str))
                backgroundImageBrush.ImageSource = BitmapFrame.Create(new Uri(str));
        }
    }
}
