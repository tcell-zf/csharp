﻿using System;
using System.Windows;
using System.Windows.Controls;

using TCell.IO;
using TCell.Text;
using TCell.Abstraction;

namespace TCell.MediaPlayerPlugins.VideoPlayer
{
    public class Player : MediaElement, IPlayable
    {
        #region properties
        public string Id
        {
            get { return "VideoPlayer"; }
        }

        public string SourcePath { get; set; }
        #endregion

        #region public functions
        public bool StartPlayer()
        {
            LoadedBehavior = MediaState.Manual;
            return true;
        }

        public bool StopPlayer()
        {
            return true;
        }

        public bool ExecuteCommand(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return true;

            TextCommand cmd = TextCommand.Parse(commandText);
            if (cmd == null)
                return true;

            bool execResult = true;
            switch (cmd.Name)
            {
                case TextCommand.CommandName.MediaPlay:
                    string path = cmd.GetParameterValue(TextCommand.ParameterName.Path);
                    execResult = PlayMedia(path);
                    break;
                case TextCommand.CommandName.MediaStop:
                    execResult = PlayMedia(string.Empty);
                    break;
                case TextCommand.CommandName.MediaMute:
                    execResult = Mute(true);
                    break;
                case TextCommand.CommandName.MediaUnmute:
                    execResult = Mute(false);
                    break;
                default:
                    break;
            }

            return execResult;
        }
        #endregion

        #region private functions
        private bool PlayMedia(string sourcePath)
        {
            if (!System.IO.File.Exists(sourcePath))
                return false;
            FileCategory category = File.GetFileCategory(sourcePath);
            if (category != FileCategory.Audio && category != FileCategory.Video)
                return false;

            SourcePath = sourcePath;
            if (string.IsNullOrEmpty(sourcePath))
            {
                this.Source = null;
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Source = new Uri(sourcePath);
                this.Visibility = Visibility.Visible;
                this.Play();
            }
            return true;
        }

        private bool Mute(bool isMute)
        {
            this.IsMuted = isMute;
            return (this.IsMuted == isMute);
        }
        #endregion
    }
}