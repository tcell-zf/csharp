using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace FramePlayer.UserControls
{
    public partial class SlidingPanel : UserControl
    {
        #region constructors
        public SlidingPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
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
        #endregion

        #region events
        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
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
        #endregion
    }
}
