namespace SignalStrength.Wpf.Model
{
    using System;

    using SignalStrength.Core;
    using SignalStrength.Core.Events;
    using SignalStrength.Core.Extensions;

    /// <summary>
    /// SignalScan class to all view have same data
    /// </summary>
    public class SignalScan 
    {
        public  ISignalStrengthScan Scan;
        public ISignalStrengthProfileData SignalStrengthProfileData { get; set; }
        public  MultiThreadObservableCollection<Core.ISignalStrengthData> NetDataCollection { get; set; }

        public event EventHandler<ScanRefreshedEventArgs> Refreshed;
        public event EventHandler<NetworkCardConnectedEventArgs> InterfaceConnectedDisconnected;

        private static readonly SignalScan _current = new SignalScan();
        public static SignalScan Current { get => _current; }

        static SignalScan()
        {
        }
        private SignalScan()
        {
            Scan = new SignalStrengthScan();
            Scan.RefreshData();
            

            NetDataCollection = Scan.SignalStrengthDataColl;
            SignalStrengthProfileData = Scan.SignalStrengthProfileData;

            Scan.NetworkCardConnectedDisconnected += Scan_NetworkCardConnectedDisconnected;
            Scan.ScanNetworkCollectionRefreshed += Scan_ScanNetworkCollectionRefreshed;
        }



        private void Scan_ScanNetworkCollectionRefreshed(object sender, Core.Events.ScanRefreshedEventArgs e)
        {
            NetDataCollection = e.SignalStrengthDataColl;
          
            Scan.RefreshDataInterval = View.SettingsScan.Current.Interval;
            Refreshed?.Invoke(sender, e);
        }

        private void Scan_NetworkCardConnectedDisconnected(object sender, Core.Events.NetworkCardConnectedEventArgs e)
        {
            InterfaceConnectedDisconnected?.Invoke(sender, e);
        }

    }
}
