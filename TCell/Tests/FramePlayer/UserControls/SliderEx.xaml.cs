using System.Windows;
using System.Windows.Controls;

namespace FramePlayer.UserControls
{
    public partial class SliderEx : UserControl
    {
        #region constructors
        public SliderEx()
        {
            InitializeComponent();
        }
        #endregion

        #region properties
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(SliderEx),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(MinimumPropertyChangedCallback)));
        private static void MinimumPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SliderEx ctrl = sender as SliderEx;
            if (ctrl != null)
                ctrl.SetMinimum();
        }
        public double Minimum
        {
            get { return (double)this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(SliderEx),
            new FrameworkPropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(MaximumPropertyChangedCallback)));
        private static void MaximumPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SliderEx ctrl = sender as SliderEx;
            if (ctrl != null)
                ctrl.SetMaximum();
        }
        public double Maximum
        {
            get { return (double)this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SliderEx),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(ValuePropertyChangedCallback)));
        private static void ValuePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SliderEx ctrl = sender as SliderEx;
            if (ctrl != null)
                ctrl.SetValue();
        }
        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(SliderEx),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(MaxLengthPropertyChangedCallback)));
        private static void MaxLengthPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SliderEx ctrl = sender as SliderEx;
            if (ctrl != null)
                ctrl.SetMaxLength();
        }
        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SliderEx),
            new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(TextPropertyChangedCallback)));
        private static void TextPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            SliderEx ctrl = sender as SliderEx;
            if (ctrl != null)
                ctrl.SetText();
        }
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }
        #endregion

        #region events
        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.D0:
                case System.Windows.Input.Key.D1:
                case System.Windows.Input.Key.D2:
                case System.Windows.Input.Key.D3:
                case System.Windows.Input.Key.D4:
                case System.Windows.Input.Key.D5:
                case System.Windows.Input.Key.D6:
                case System.Windows.Input.Key.D7:
                case System.Windows.Input.Key.D8:
                case System.Windows.Input.Key.D9:
                    break;
                case System.Windows.Input.Key.Enter:
                    slider.Focus();
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region private functions
        private void SetMinimum()
        {
            slider.Minimum = Minimum;
        }

        private void SetMaximum()
        {
            slider.Maximum = Maximum;
        }

        private void SetValue()
        {
            slider.Value = Value;
        }

        private void SetMaxLength()
        {
            textBox.MaxLength = MaxLength;
        }

        private void SetText()
        {
            textBlock.Text = Text;
        }
        #endregion
    }
}
