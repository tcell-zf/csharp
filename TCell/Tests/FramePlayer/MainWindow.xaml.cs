using System.Windows;

namespace FramePlayer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SlidingPanel_ButtonClicked(object sender, UserControls.ButtonClickedEventArgs e)
        {
            switch (e.ButtonType)
            {
                case UserControls.ButtonTypeEnum.Action:
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
    }
}
