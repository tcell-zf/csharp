using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using Leap;
using System.Collections.Generic;

namespace BallRoller
{
    public partial class MainWindow : Window
    {
        #region constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private Controller leapCtrl = null;
        private DateTime? prevFrameReceived = null;
        private float? prevDist = null;
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            leapCtrl = new Controller(Properties.Settings.Default.LeapControlConnectionKey);
            leapCtrl.EventContext = SynchronizationContext.Current;
            leapCtrl.Init += leapCtrl_Init;
            leapCtrl.Connect += leapCtrl_Connect;
            leapCtrl.Disconnect += leapCtrl_Disconnect;
            leapCtrl.DeviceFailure += leapCtrl_DeviceFailure;
            leapCtrl.DeviceLost += leapCtrl_DeviceLost;
            leapCtrl.FrameReady += leapCtrl_FrameReady;
            //ball.Roll();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                Close();
        }

        private void CanvasRed_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeBackground("RedBackground");
        }

        private void CanvasRed_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ChangeBackground("RedBackground");
        }

        private void CanvasBlue_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeBackground("BlueBackground");
        }

        private void CanvasBlue_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ChangeBackground("BlueBackground");
        }

        private void CanvasPink_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeBackground("PinkBackground");
        }

        private void CanvasPink_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ChangeBackground("PinkBackground");
        }

        private void buttonFullScreen_Click(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    switch (WindowStyle)
                    {
                        case WindowStyle.SingleBorderWindow:
                            WindowStyle = WindowStyle.None;
                            buttonFullScreen.Content = "N";
                            break;
                        case WindowStyle.None:
                            WindowStyle = WindowStyle.SingleBorderWindow;
                            WindowState = WindowState.Normal;
                            buttonFullScreen.Content = "F";
                            break;
                        default:
                            break;
                    }
                    break;
                case WindowState.Normal:
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    buttonFullScreen.Content = "N";
                    break;
                default:
                    break;
            }
        }

        private void leapCtrl_Init(object sender, LeapEventArgs e)
        {
            textBlockConnectionInfo.Text = "Leap Motion Controller: INITIALIZED";
        }

        private void leapCtrl_Connect(object sender, ConnectionEventArgs e)
        {
            textBlockConnectionInfo.Text = leapCtrl.IsConnected ? "Leap Motion Controller: CONNECTED" : "Leap Motion Controller: DISCONNECTED";
        }

        private void leapCtrl_Disconnect(object sender, ConnectionLostEventArgs e)
        {
            textBlockConnectionInfo.Text = leapCtrl.IsConnected ? "Leap Motion Controller: CONNECTED" : "Leap Motion Controller: DISCONNECTED";
        }

        private void leapCtrl_DeviceLost(object sender, DeviceEventArgs e)
        {
            textBlockConnectionInfo.Text = "Leap Motion Controller: LOST";
        }

        private void leapCtrl_DeviceFailure(object sender, DeviceFailureEventArgs e)
        {
            textBlockConnectionInfo.Text = "Leap Motion Controller: FAILURE";
        }

        private void leapCtrl_FrameReady(object sender, FrameEventArgs eventArgs)
        {
            if (prevFrameReceived == null)
            {
                prevFrameReceived = DateTime.Now;
                return;
            }
            if ((DateTime.Now - prevFrameReceived.Value).Milliseconds < 500)
                return;
            if (eventArgs.frame.Hands == null || eventArgs.frame.Hands.Count == 0)
                return;

            if (eventArgs.frame.Hands.Count == 2)
            {
                HandleZooming(eventArgs.frame.Hands[0], eventArgs.frame.Hands[1]);
            }
            else
            {
                prevDist = null;

                if (eventArgs.frame.Hands.Count == 1)
                {
                    if (eventArgs.frame.Hands[0].IsLeft)
                        HandleBackgroundChanging(eventArgs.frame.Hands[0]);
                }
            }

            //Hand h = eventArgs.frame.Hands[0];
            //if (h != null)
            //{
            //    if (h.IsRight)
            //    {
            //        if (h.PinchStrength > 0.4)
            //        {
            //            if (h.PinchDistance > 40)
            //                ball.ZoomFactor += 0.05;

            //            if (ball.ZoomFactor > 4)
            //                ball.ZoomFactor = 4;
            //        }
            //    }
            //    else
            //    {
            //        if (h.PinchStrength > 0.4)
            //        {
            //            if (h.PinchDistance > 40)
            //                ball.ZoomFactor -= 0.05;

            //            if (ball.ZoomFactor < 0)
            //                ball.ZoomFactor = 0;
            //        }
            //    }

            //    //Finger thumb = (from f in h.Fingers
            //    //                where f.Type == Finger.FingerType.TYPE_THUMB
            //    //                select f).SingleOrDefault();
            //    //Finger index = (from f in h.Fingers
            //    //                where f.Type == Finger.FingerType.TYPE_INDEX
            //    //                select f).SingleOrDefault();

            //    //if (thumb != null && index != null)
            //    //{

            //    //}
            //}
        }
        #endregion

        #region private function
        private void ChangeBackground(string bgName)
        {
            var res = this.TryFindResource(bgName) as RadialGradientBrush;
            if (res != null)
                mainGrid.Background = res;
        }

        private void HandleZooming(Hand h1, Hand h2)
        {
            float distance = h1.PalmPosition.DistanceTo(h2.PalmPosition);

            if (prevDist != null)
            {
                float diff = distance - prevDist.Value;

                double factor = ball.ZoomFactor;
                if (diff < -2.0)
                {
                    factor -= 0.01;
                    if (factor < 0)
                        factor = 0;
                }
                else if (diff > 2.0)
                {
                    factor += 0.01;
                    if (factor > 4.0)
                        factor = 4.0;
                }
                else { }

                ball.ZoomFactor = factor;
            }

            prevDist = distance;
        }

        private void HandleBackgroundChanging(Hand h)
        {
            List<Finger> extendedFingers = new List<Finger>();
            foreach (Finger f in h.Fingers)
            {
                if (f.IsExtended)
                {
                    if (extendedFingers == null)
                        extendedFingers = new List<Finger>();
                    extendedFingers.Add(f);
                }
            }

            switch (extendedFingers.Count)
            {
                case 0:
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                    buttonFullScreen.Content = "F";
                    break;
                case 1:
                    if (extendedFingers[0].Type == Finger.FingerType.TYPE_INDEX)
                        ChangeBackground("RedBackground");
                    break;
                case 2:
                    if (extendedFingers[0].Type == Finger.FingerType.TYPE_INDEX || extendedFingers[0].Type == Finger.FingerType.TYPE_MIDDLE
                        || extendedFingers[1].Type == Finger.FingerType.TYPE_INDEX || extendedFingers[1].Type == Finger.FingerType.TYPE_MIDDLE)
                        ChangeBackground("BlueBackground");
                    break;
                case 3:
                    if (extendedFingers[0].Type == Finger.FingerType.TYPE_MIDDLE || extendedFingers[0].Type == Finger.FingerType.TYPE_RING || extendedFingers[0].Type == Finger.FingerType.TYPE_PINKY
                        || extendedFingers[1].Type == Finger.FingerType.TYPE_MIDDLE || extendedFingers[1].Type == Finger.FingerType.TYPE_RING || extendedFingers[1].Type == Finger.FingerType.TYPE_PINKY
                        || extendedFingers[2].Type == Finger.FingerType.TYPE_MIDDLE || extendedFingers[2].Type == Finger.FingerType.TYPE_RING || extendedFingers[2].Type == Finger.FingerType.TYPE_PINKY)
                        ChangeBackground("PinkBackground");
                    break;
                case 5:
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    buttonFullScreen.Content = "N";
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
