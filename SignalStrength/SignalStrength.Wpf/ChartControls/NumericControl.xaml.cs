namespace SignalStrength.Wpf.ChartControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for NumericControl.xaml
    /// </summary>
    [TemplatePart(Name = NumericControl.UP_BUTTON, Type = typeof(NumericControl))]
    [TemplatePart(Name = NumericControl.DOWN_BUTTON, Type = typeof(NumericControl))]
    public partial class NumericControl : UserControl
    {
        public readonly static DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(int), typeof(NumericControl), new UIPropertyMetadata(1));
        public readonly static DependencyProperty MaxProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericControl), new UIPropertyMetadata(int.MaxValue));
        public readonly static DependencyProperty MinProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericControl), new UIPropertyMetadata(int.MinValue));
        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                    "Value", typeof(int), typeof(NumericControl), new UIPropertyMetadata(0, (o, e) =>
                                                                    {
                                                                        NumericControl tb = (NumericControl)o;
                                                                        tb.RaiseValueChangedEvent(e);
                                                                    }));

        #region Fields
        public const string UP_BUTTON = "PART_UpButton";
        public const string DOWN_BUTTON = "PART_DownButton";

        public event EventHandler<DependencyPropertyChangedEventArgs> ValueChanged;
        Button _UpButton;
        Button _DownButton;
        #endregion
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _UpButton = Template.FindName(UP_BUTTON, this) as Button;
            _DownButton = Template.FindName(DOWN_BUTTON, this) as Button;
            _UpButton.Click += this.PART_UpButton_Click;
            _DownButton.Click += this.PART_DownButton_Click;
        }

        public NumericControl()
        {
            InitializeComponent();
            this.DefaultStyleKey = typeof(NumericControl);
        }
        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }



        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetCurrentValue(ValueProperty, value); }
        }

        private void RaiseValueChangedEvent(DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }


        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }



        private void PART_UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Max)
            {
                Value += Step;
                if (Value > Max)
                    Value = Max;
            }
        }

        private void PART_DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Min)
            {
                Value -= Step;
                if (Value < Min)
                    Value = Min;
            }
        }
    }
}
