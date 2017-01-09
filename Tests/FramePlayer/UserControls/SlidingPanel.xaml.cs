using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FramePlayer.UserControls
{
    public enum ButtonTypeEnum
    { Action, View }
    public class ButtonClickedEventArgs : RoutedEventArgs
    {
        public ButtonTypeEnum ButtonType { get; set; }
    }
    public delegate void ButtonClickedEventHandler(object sender, ButtonClickedEventArgs e);

    public partial class SlidingPanel : UserControl
    {
        #region constructors
        public SlidingPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        private bool isFullScreen = false;

        public string ActionButonText
        {
            set { buttonAction.Content = value; }
        }

        public List<string> FrameImages
        {
            get { return folderSelector.FrameImages; }
        }

        public int Interval
        {
            get { return (int)sliderInterval.Value; }
        }

        public int Step
        {
            get { return (int)sliderStep.Value; }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SlidingPanel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsExpandedPropertyChangedCallback)));
        private static void IsExpandedPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SlidingPanel ctrl = sender as SlidingPanel;
            if (ctrl != null)
                ctrl.PlaySlidingEffect();
        }
        public bool IsExpanded
        {
            get { return (bool)this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        public static readonly RoutedEvent ButtonClickedEvent =
            EventManager.RegisterRoutedEvent("ButtonClicked", RoutingStrategy.Bubble, typeof(ButtonClickedEventHandler), typeof(SlidingPanel));
        public event ButtonClickedEventHandler ButtonClicked
        {
            add { AddHandler(ButtonClickedEvent, value); }
            remove { RemoveHandler(ButtonClickedEvent, value); }
        }
        #endregion

        #region events
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetControls();
        }

        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
        }

        private void folderSelector_FrameImagesSelected(object sender, FrameImagesSelectedEventArgs e)
        {
            SetControls();
        }

        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            ButtonClickedEventArgs args = new ButtonClickedEventArgs()
            {
                RoutedEvent = ButtonClickedEvent,
                ButtonType = ButtonTypeEnum.Action
            };
            RaiseEvent(args);
        }

        private void buttonView_Click(object sender, RoutedEventArgs e)
        {
            ButtonClickedEventArgs args = new ButtonClickedEventArgs()
            {
                RoutedEvent = ButtonClickedEvent,
                ButtonType = ButtonTypeEnum.View
            };
            RaiseEvent(args);

            isFullScreen = !isFullScreen;
            buttonView.Content = isFullScreen ? "Normal Screen" : "Full Screen";
        }
        #endregion

        #region private functions
        private void PlaySlidingEffect()
        {
            if (IsExpanded)
            {
                (FindResource("expanding") as Storyboard).Begin();
            }
            else
            {
                (FindResource("collapsing") as Storyboard).Begin();
            }
        }

        private void SetControls()
        {
            if (folderSelector.FrameImages != null && folderSelector.FrameImages.Count > 0)
            {
                parameterPanel.IsEnabled = true;
            }
            else
            {
                parameterPanel.IsEnabled = false;
            }
        }
        #endregion
    }
}
