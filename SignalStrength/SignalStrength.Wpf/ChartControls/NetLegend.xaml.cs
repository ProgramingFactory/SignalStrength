namespace SignalStrength.Wpf.ChartControls
{
    using System;
    using System.Collections.Generic;
using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    using LiveCharts.Wpf;

    /// <summary>
    /// Interaction logic for NetLegend.xaml
    /// </summary>
    public partial class NetLegend : UserControl, IChartLegend
    {
        private List<SeriesViewModel> _series;
        public NetLegend()
        {
            InitializeComponent();
            DataContext = this;
        }


        public List<SeriesViewModel> Series
        {
            get { return _series; }
            set
            {
                _series = value;
                OnPropertyChanged("Series");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
