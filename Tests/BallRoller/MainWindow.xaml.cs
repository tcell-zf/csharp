using System.Windows;
using System.Windows.Media;

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

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ball.Roll();
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
        #endregion

        #region private function
        private void ChangeBackground(string bgName)
        {
            var res = this.TryFindResource(bgName) as RadialGradientBrush;
            if (res != null)
                mainGrid.Background = res;
        }
        #endregion
    }
}
