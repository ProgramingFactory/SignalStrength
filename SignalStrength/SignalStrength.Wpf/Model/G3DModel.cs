namespace SignalStrength.Wpf.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SignalStrength.Core;
    using SignalStrength.Wpf.Properties;

    public class G3DModel : INotifyPropertyChanged
    {
        public G3DModel()
        {
            SignalScan.Current.Refreshed += Scan_ScanNetworkCollectionRefreshed;
            TimeColumns = Settings.Default.ChosenTimeColumns;
            UpdateChannals(SignalScan.Current.NetDataCollection);
        }
        private void Scan_ScanNetworkCollectionRefreshed(object sender, Core.Events.ScanRefreshedEventArgs e)
        {
            UpdateChannals(e.SignalStrengthDataColl);
        }

        private void UpdateChannals(SignalStrength.Core.Extensions.MultiThreadObservableCollection<ISignalStrengthData> data)
        {
            TimeColumns = Settings.Default.ChosenTimeColumns;
            ScanValues = data.Select(x => new SignalStrength.Graphic3D.ScanValues() { Channal = x.Channel, Name = x.Name, Time = DateTime.Now.ToString("HH:mm:ss") })
                             .OrderBy(x => x.Channal)
                             .ToArray();
        }

        #region Channal graph
        public SignalStrength.Graphic3D.ScanValues[] ScanValues
        {
            get => scanValues;
            set { _UpdateField(ref scanValues, value); }
        }
        private SignalStrength.Graphic3D.ScanValues[] scanValues;


        public int TimeColumns
        {
            get => _timeColums;
            set { _UpdateField(ref _timeColums, value); }
        }
        private int _timeColums;
        #endregion

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
}
