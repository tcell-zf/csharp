using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WINFORM = System.Windows.Forms;

namespace FramePlayer.UserControls
{
    public class FrameImagesSelectedEventArgs : RoutedEventArgs
    {
        public List<string> FrameImages { get; set; }
    }
    public delegate void FrameImagesSelectedEventHandler(object sender, FrameImagesSelectedEventArgs e);

    public partial class FolderSelector : UserControl
    {
        #region constructors
        public FolderSelector()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        public string SelectedPath
        {
            get
            {
                string selectedPath = string.Empty;
                if (!string.IsNullOrEmpty(textBoxFolder.Text))
                {
                    if (Directory.Exists(textBoxFolder.Text))
                        selectedPath = textBoxFolder.Text;
                }
                return selectedPath;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(SelectedPath));

                if (!Directory.Exists(value))
                    throw new FileNotFoundException();

                textBoxFolder.Text = value;
            }
        }

        public List<string> FrameImages { get; set; }

        public static readonly RoutedEvent FrameImagesSelectedEvent =
            EventManager.RegisterRoutedEvent("FrameImagesSelected", RoutingStrategy.Bubble, typeof(FrameImagesSelectedEventHandler), typeof(FolderSelector));
        public event FrameImagesSelectedEventHandler FrameImagesSelected
        {
            add { AddHandler(FrameImagesSelectedEvent, value); }
            remove { RemoveHandler(FrameImagesSelectedEvent, value); }
        }
        #endregion

        #region events
        private void buttonSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new WINFORM.FolderBrowserDialog()
            {
                Description = "Please select the folder which contains the frame images.",
                ShowNewFolderButton = false,
                SelectedPath = this.SelectedPath
            };
            if (dlg.ShowDialog() == WINFORM.DialogResult.OK)
            {
                FindAllImagesInTheFolder(dlg.SelectedPath);

                if (FrameImages != null && FrameImages.Count > 0)
                {
                    this.SelectedPath = dlg.SelectedPath;

                    FrameImagesSelectedEventArgs args = new FrameImagesSelectedEventArgs()
                    {
                        RoutedEvent = FrameImagesSelectedEvent,
                        FrameImages = this.FrameImages
                    };
                    RaiseEvent(args);
                }
                else
                {
                    textBoxFolder.Text = string.Empty;
                }
            }
        }
        #endregion

        #region private functions
        private void FindAllImagesInTheFolder(string path)
        {
            FrameImages = null;

            if (!Directory.Exists(path))
                return;

            string[] files = Directory.GetFiles(path);
            if (files == null || files.Length == 0)
                return;

            foreach (string file in files)
            {
                if (TCell.IO.File.GetFileCategory(file) != TCell.IO.FileCategory.Image)
                    continue;

                if (FrameImages == null)
                    FrameImages = new List<string>();
                FrameImages.Add(file);
            }
        }
        #endregion
    }
}
