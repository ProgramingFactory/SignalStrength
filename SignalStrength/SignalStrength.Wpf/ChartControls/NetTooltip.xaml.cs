namespace SignalStrength.Wpf.ChartControls
{
    using System.ComponentModel;
    using System.Windows.Controls;

    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Interaction logic for NetTooltip.xaml
    /// </summary>
    public partial class NetTooltip : UserControl,IChartTooltip
    {
        private TooltipData _data;
        public NetTooltip()
        {
            //System.Xaml.XamlParseException
            SelectionMode = TooltipSelectionMode.OnlySender;
            InitializeComponent();
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
