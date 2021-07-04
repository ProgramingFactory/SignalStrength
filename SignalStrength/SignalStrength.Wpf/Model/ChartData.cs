namespace SignalStrength.Wpf.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using LiveCharts;
    using LiveCharts.Wpf;

    using SignalStrength.Core.Extensions;

    public class ChartData : INotifyPropertyChanged
    {
        public Func<double, string> DateTimeFormatter { get; set; }
        public Func<ChartPoint, string> TooltipFormatter { get; set; }

        private TimeSpan RangeTime;
        public static int Range { get; set; } = Properties.Settings.Default.ChoosenRange;


        public ChartData()
        {
            SignalScan.Current.Refreshed += Scan_ScanNetworkCollectionRefreshed;

            GraphSeries = new SeriesCollection();
            var mapper = LiveCharts.Configurations.Mappers.Xy<ChartModel>()
           .X(model => model.Time.Ticks)     //use DateTime.Ticks as X
           .Y(model => model.NetSStrength);  //use the value property as Y
            //lets save the mapper globally.
            Charting.For<ChartModel>(mapper);

            DateTimeFormatter = value =>  new DateTime((long)value).ToString("HH:mm:ss");
          
            RangeTime = TimeSpan.FromMinutes(Range);
        }


        private void Scan_ScanNetworkCollectionRefreshed(object sender, Core.Events.ScanRefreshedEventArgs e)
        {
            UpdateChart(e.SignalStrengthDataColl);
        }

        public void UpdateChart(MultiThreadObservableCollection<Core.ISignalStrengthData> netDataColl)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                Range = Properties.Settings.Default.ChoosenRange;
                RangeTime = TimeSpan.FromMinutes(Range);

                var names = netDataColl.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => new { NetName = x.Key, CountTime = x.Count() });

                foreach (var item in netDataColl)
                {
                    var lineSeria = GraphSeries.Where(ls => ls.Title == item.Name).FirstOrDefault();
                    if (lineSeria != null)
                    {
                        lineSeria.Values.Add(new ChartModel()
                        {
                           // Name = item.Name,
                            Time = DateTime.Now,
                            NetSStrength = item.SignalQuality,
                            NetBand = item.NetBand.ToString(),
                            Channel = item.Channel.ToString(),
                            Mac = item.MacAddress
                        });
                        lineSeria.Values.InitializeStep(lineSeria);
                    }
                    else
                    {
                        LineSeries newSeria = new LineSeries();

                        newSeria.Title = item.Name;
                        newSeria.Values = new ChartValues<ChartModel>() { new ChartModel() { NetSStrength = item.SignalQuality,
                                                                                             Time = DateTime.Now , NetBand = item.NetBand.ToString(),
                                                                                             Channel=item.Channel.ToString(), Mac = item.MacAddress} };
                        newSeria.PointGeometrySize = 15;
                        newSeria.PointGeometry = null;
                        newSeria.LineSmoothness = (item.Name.Length * 0.05) > 1 ? 1 : item.Name.Length * 0.08;

                        //TODO: REMOVE (simple tooltip formating binding)
                        //TooltipFormatter = tip =>
                        //                $" Strength: {tip.Y}\n Time: {new DateTime((long)tip.X).ToString("HH:mm:ss")}";

                        //BindingOperations.SetBinding(newSeria, LineSeries.LabelPointProperty, new Binding("TooltipFormatter"));

                        if (!(GraphSeries.Where(x => x.Title == newSeria.Title).Any()))
                            GraphSeries.Add(newSeria);


                    }
                }
                GraphSeries.Where(g => g.ActualValues.Count > 1).ToList().ForEach(x => (x as LineSeries).PointGeometry = null);
                GraphSeries.Where(g => g.ActualValues.Count == 1).ToList().ForEach(x => (x as LineSeries).PointGeometry = DefaultGeometries.Diamond);
 
                var valueForRemove = GraphSeries.Where(s => s.Values.OfType<ChartModel>().Where(x => x.Time < (DateTime.Now - RangeTime)).Any()).Any();
                if (valueForRemove)
                {
                    foreach (var series in GraphSeries)
                    {
                        var tracker = series.Values.GetTracker(series);
                        var points = tracker.Referenced.Keys.OfType<ChartModel>().Where(x => x.Time < (DateTime.Now - RangeTime)).Select(x => x).ToList();
                        points.ForEach(p =>
                        {
                            if (series.Values.Count == 1)
                            {
                                (series as LineSeries).PointGeometry = null;
                                GraphSeries.Remove(series);
                            }
                            series.Values.Remove(p);

                        });
                    }
                }
 
            }));
        }

        public LiveCharts.SeriesCollection GraphSeries
        {
            get { return _graphSeries; }
            set { _UpdateField(ref _graphSeries, value); }
        }
        private LiveCharts.SeriesCollection _graphSeries;


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void _UpdateField<T>(ref T field, T newValue, Action<T> onChangedCallback = null,
                                  [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            T oldValue = field;

            field = newValue;
            onChangedCallback?.Invoke(oldValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    public class ChartModel
    {
        public int NetSStrength { get; set; } 
        public string NetBand { get; set; }
        public string Channel { get; set; }
        public string Mac { get; set; }
        public DateTime Time { get; set; }

        //public string Name { get; set; }
        //public int LinkQuality { get; set; }
        //public int RSSI { get; set; }
    }
}
