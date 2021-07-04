namespace SignalStrength.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using ManagedNativeWifi;

    using SignalStrength.Core.Events;
    using SignalStrength.Core.Extensions;

    /// <summary>
    /// This is interface ISignalStrengthScan
    /// </summary>
    public interface ISignalStrengthScan : INetworkCardConnected,IScanNetworkRefreshed
    {
        bool IsRadioOn { get; }
        bool TurnRadioOn { get; set; }
        bool RefreshData();
        int RefreshDataInterval { get; set; }
        Extensions.MultiThreadObservableCollection<ISignalStrengthData> SignalStrengthDataColl { get; set; }
        ISignalStrengthProfileData SignalStrengthProfileData { get; set; }
        bool IsScanning { get; set; }
    }


    /// <summary>
    /// This is class SignalStrengthScan
    /// </summary>
    public class SignalStrengthScan : ISignalStrengthScan
    {                  
        public bool IsRadioOn => RadioConnected(); 
        //TODO: delate
        public bool TurnRadioOn { get; set; }
        public int RefreshDataInterval { get; set; } = 5;
        public bool IsScanning { get; set; }

        public MultiThreadObservableCollection<ISignalStrengthData> SignalStrengthDataColl { get; set; }
        public ISignalStrengthProfileData SignalStrengthProfileData { get; set; }


        public event EventHandler<NetworkCardConnectedEventArgs> NetworkCardConnectedDisconnected;
        public event EventHandler<ScanRefreshedEventArgs> ScanNetworkCollectionRefreshed;

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private ManagedNativeWifi.NativeWifiPlayer NativeWifiPlayer;
        private const string HIDDEN_NET = "<?>";


        /// <summary>
        /// Call only one time on start
        /// </summary>
        /// <returns>true if can be refreshed</returns>
        public bool RefreshData()
        {
            SignalStrengthDataColl = new MultiThreadObservableCollection<ISignalStrengthData>();
            Refresh();
            NativeWifiPlayer = new NativeWifiPlayer();
            NativeWifiPlayer.AvailabilityChanged += (s, e) =>
            {
                OnNetworkCardConnectedDisconnected(RadioConnected());
            };
            OnNetworkCardConnectedDisconnected(IsRadioOn);//check is state on start
            return IsRadioOn;
        }

        private void Refresh()
        {
            var _ = RefreshAsync(); 
        }
        private async Task RefreshAsync()
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                IsScanning = false;
                var interval = RefreshDataInterval;
                await Task.Delay(interval * 1000/*ms*/);

                if (CancellationTokenSource.IsCancellationRequested)
                        SignalStrengthDataColl.Clear();

                await RefreshAsync(CancellationTokenSource.Token, interval);

                await PopulateSignalData();

                await Task.Delay(0);
            }
        }


        private async Task PopulateSignalData()
        {
            SignalStrengthDataColl.Clear();
           
            foreach (var net in ManagedNativeWifi.NativeWifi.EnumerateAvailableNetworkGroups().OrderByDescending(s => s.SignalQuality))
            {
                IsScanning = true;
                await Task.Delay(0);

                foreach (var bss in net.BssNetworks)
                {
                    if (string.IsNullOrEmpty(SignalStrengthProfileData?.ProfileName))
                        SignalStrengthProfileData = new SignalStrengthProfileData(net.ProfileName, net.Interface.Description, bss.Bssid.ToString());

                    string name = default(string);
                    name = net.Ssid.ToString();
                    if (name is "") {
                        name = Encoding.ASCII.GetString(bss.Ssid.ToBytes(), 0, bss.Ssid.ToBytes().Length);
                        if (string.IsNullOrEmpty(name))
                            name = HIDDEN_NET;
                    }
                  
                    var data = new SignalStrengthData(name, net.SignalQuality, bss.Frequency, bss.Band, bss.LinkQuality, bss.SignalStrength, bss.Channel, bss.Bssid.ToString());
                    //remove double entries
                    if (!SignalStrengthDataColl.Where(x => x.Name == bss.Ssid.ToString()).Any())
                            SignalStrengthDataColl.Add(data);

                    
                }
            }
            RenameDoubleHiddenEntry(SignalStrengthDataColl);
            OnScanRefreshed(SignalStrengthDataColl);
        }



        private void OnNetworkCardConnectedDisconnected(bool radioConnected)
        {
            if (!radioConnected) {
                IsScanning = false;
            }
           NetworkCardConnectedDisconnected?.Invoke(this, new NetworkCardConnectedEventArgs(radioConnected));
        }

        private void OnScanRefreshed(MultiThreadObservableCollection<ISignalStrengthData> dataCollection)
        {
            ScanNetworkCollectionRefreshed?.Invoke(this, new ScanRefreshedEventArgs(dataCollection));
        }

        /// <summary>
        /// See is radio hardware or software ON or OFF
        /// </summary>
        /// <returns>true if On</returns>
        public static bool RadioConnected()
        {
            foreach (var interfaceInfo in NativeWifi.EnumerateInterfaceConnections())
            {

                var interfaceRadio = NativeWifi.GetInterfaceRadio(interfaceInfo.Id);
                if (interfaceRadio is null)
                    continue;

                return interfaceInfo.IsRadioOn;
            }
            return false;
        }


        /// <summary>
        /// Refreshes available wireless LANs.
        /// </summary>
        private Task RefreshAsync(int seconds)
        {
            return ManagedNativeWifi.NativeWifi.ScanNetworksAsync(timeout: TimeSpan.FromSeconds(seconds), CancellationToken.None);
        }


        /// <summary>
        /// Refreshes available wireless LANs.
        /// </summary>
        private Task RefreshAsync(CancellationToken cancellation, int seconds)
        {
            return ManagedNativeWifi.NativeWifi.ScanNetworksAsync(timeout: TimeSpan.FromSeconds(seconds), cancellation);
        }


        //Rename <?> multi hidden networks
        private void RenameDoubleHiddenEntry(MultiThreadObservableCollection<ISignalStrengthData> data)
        {
            var duplicate = data.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x);
            foreach (var item in duplicate)
            {
                var count = item.Count();

                if(item.Key is HIDDEN_NET)
                for (int i = item.Count(); i > 1; i--)
                {
                    var ssd = data.Where(x => x == item.ElementAt(i - 1)).LastOrDefault();
                    ssd.Name = $"{ssd.Name}_{i - 1}";
                }
            }
        }
    }
}
