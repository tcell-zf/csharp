using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BallRoller.UserControls
{
    public partial class Ball : UserControl
    {
        #region constructors
        public Ball()
        {
            InitializeComponent();

            // setup trackball for moving the model around
            trackball = new TrackballTemplate();
            trackball.Attach(this);
            trackball.Slaves.Add(myViewport3D);
            trackball.Enabled = true;
            trackball.Zoom(0.72);
        }
        #endregion

        #region properties
        TrackballTemplate trackball;

        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(Ball),
            new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(ZoomFactorPropertyChangedCallback)));
        private static void ZoomFactorPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            Ball ctrl = sender as Ball;
            if (ctrl != null)
                ctrl.SetZoomFactor();
        }
        public double ZoomFactor
        {
            get { return (double)this.GetValue(ZoomFactorProperty); }
            set { this.SetValue(ZoomFactorProperty, value); }
        }

        public static readonly DependencyProperty XAngelProperty = DependencyProperty.Register("XAngel", typeof(double), typeof(Ball),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(XAngelPropertyChangedCallback)));
        private static void XAngelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            Ball ctrl = sender as Ball;
            if (ctrl != null)
                ctrl.SetXAngel();
        }
        public double XAngel
        {
            get { return (double)this.GetValue(XAngelProperty); }
            set { this.SetValue(XAngelProperty, value); }
        }

        public static readonly DependencyProperty YAngelProperty = DependencyProperty.Register("YAngel", typeof(double), typeof(Ball),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(YAngelPropertyChangedCallback)));
        private static void YAngelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            Ball ctrl = sender as Ball;
            if (ctrl != null)
                ctrl.SetYAngel();
        }
        public double YAngel
        {
            get { return (double)this.GetValue(YAngelProperty); }
            set { this.SetValue(YAngelProperty, value); }
        }

        public static readonly DependencyProperty ZAngelProperty = DependencyProperty.Register("ZAngel", typeof(double), typeof(Ball),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(ZAngelPropertyChangedCallback)));
        private static void ZAngelPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            Ball ctrl = sender as Ball;
            if (ctrl != null)
                ctrl.SetZAngel();
        }
        public double ZAngel
        {
            get { return (double)this.GetValue(ZAngelProperty); }
            set { this.SetValue(ZAngelProperty, value); }
        }
        #endregion

        #region public functions
        public void Zoom(double factor)
        {
            trackball.Zoom(factor);
        }

        public void Roll()
        {
            Storyboard s = (Storyboard)this.FindResource("RotateStoryboard");
            this.BeginStoryboard(s);
        }
        #endregion

        #region private functions
        private void SetZoomFactor()
        {
            Zoom(ZoomFactor);
        }

        private void SetXAngel()
        {
            myRotateX.Angle = XAngel;
        }

        private void SetYAngel()
        {
            myRotateY.Angle = YAngel;
        }

        private void SetZAngel()
        {
            myRotateZ.Angle = ZAngel;
        }
        #endregion
    }
}
