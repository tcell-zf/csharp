using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FramePlayer
{
    internal enum PlayingStateType
    { Idle, Playing, Paused }

    public partial class MainWindow : Window
    {
        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private PlayingStateType state = PlayingStateType.Idle;
        private int currentFrameIndex = 0;
        private DispatcherTimer frameTimer = null;
        #endregion

        #region events
        private void Player_Closed(object sender, EventArgs e)
        {
            if (frameTimer != null)
            {
                frameTimer.Stop();
                frameTimer = null;
            }
        }

        private void Player_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                Close();
        }

        private void SlidingPanel_ButtonClicked(object sender, UserControls.ButtonClickedEventArgs e)
        {
            switch (e.ButtonType)
            {
                case UserControls.ButtonTypeEnum.Action:
                    Play();
                    break;
                case UserControls.ButtonTypeEnum.View:
                    switch (WindowState)
                    {
                        case WindowState.Normal:
                            WindowState = WindowState.Maximized;
                            WindowStyle = WindowStyle.None;
                            break;
                        case WindowState.Maximized:
                            WindowState = WindowState.Normal;
                            WindowStyle = WindowStyle.ThreeDBorderWindow;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region private functions
        private void Play()
        {
            switch (state)
            {
                case PlayingStateType.Idle:
                    StartPlayingFrames();
                    break;
                case PlayingStateType.Playing:
                    Pause();
                    break;
                case PlayingStateType.Paused:
                    Resume();
                    break;
                default:
                    break;
            }
        }

        private void StartPlayingFrames()
        {
            if (state != PlayingStateType.Idle)
                return;

            panel.ActionButonText = "Pause";
            frameTimer = ActionInATimeInterval(panel.Interval, new EventHandler(NextFrame));

            state = PlayingStateType.Playing;
        }

        private void Pause()
        {
            if (state != PlayingStateType.Playing)
                return;

            if (frameTimer != null)
            {
                panel.ActionButonText = "Play";
                frameTimer.Stop();

                state = PlayingStateType.Paused;
            }
        }

        private void Resume()
        {
            if (state != PlayingStateType.Paused)
                return;

            if (frameTimer != null)
            {
                panel.ActionButonText = "Pause";
                frameTimer.Start();

                state = PlayingStateType.Playing;
            }
        }

        private void Stop()
        {
            if (frameTimer != null)
            {
                frameTimer.Stop();
                frameTimer = null;
            }

            img.Source = null;
            panel.ActionButonText = "Play";
            state = PlayingStateType.Idle;
        }

        private static DispatcherTimer ActionInATimeInterval(double delayTime, EventHandler handler)
        {
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(delayTime),
                IsEnabled = false
            };
            timer.Tick += new EventHandler(handler);
            timer.Start();

            return timer;
        }

        private void NextFrame(Object sender, EventArgs e)
        {
            if (panel.FrameImages == null || panel.FrameImages.Count == 0)
            {
                Stop();
                return;
            }

            if (state != PlayingStateType.Playing)
                return;

            img.Source = BitmapFrame.Create(new Uri(panel.FrameImages[currentFrameIndex]));

            currentFrameIndex += panel.Step;
            if (currentFrameIndex > (panel.FrameImages.Count - 1))
                currentFrameIndex = 0;

            if (frameTimer != null)
                frameTimer.Interval = TimeSpan.FromMilliseconds(panel.Interval);
        }
        #endregion
    }
}
