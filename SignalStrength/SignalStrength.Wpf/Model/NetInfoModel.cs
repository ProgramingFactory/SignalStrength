namespace SignalStrength.Wpf.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SignalStrength.Core;
    using SignalStrength.Core.Extensions;

    public class NetInfoModel : INotifyPropertyChanged
    {
        public ISignalStrengthProfileData SignalStrengthProfileData { get; set; }
        public static MultiThreadObservableCollection<Core.ISignalStrengthData> NetDataCollection { get; set; }

        private bool IsProfileSet = false;

        public NetInfoModel()
        {
            SignalScan.Current.Refreshed += Scan_ScanNetworkCollectionRefreshed;
            SignalScan.Current.InterfaceConnectedDisconnected += Scan_NetworkCardConnectedDisconnected ;
            NetDataCollection = SignalScan.Current.NetDataCollection;
            //set  IsInterfaceUp for splash screen 
            IsInterfaceUp = SignalScan.Current.Scan.IsRadioOn;
            IsScanning = NetDataCollection is null ? false : NetDataCollection.Count != 0;
        }
        private void Scan_ScanNetworkCollectionRefreshed(object sender, Core.Events.ScanRefreshedEventArgs e)
        {
            IsProfileSet = !string.IsNullOrEmpty(Profile);
            NetDataCollection = e.SignalStrengthDataColl;

            IsScanning = NetDataCollection is null ? false : NetDataCollection.Count != 0;

            if (SignalScan.Current.Scan.SignalStrengthProfileData != null && IsProfileSet is false)
            {
                Profile = null; Interface = null; InterfaceMac = null;
                Profile = SignalScan.Current.Scan.SignalStrengthProfileData.ProfileName;
                Interface = SignalScan.Current.Scan.SignalStrengthProfileData.InerfaceDescription;
                InterfaceMac = SignalScan.Current.Scan.SignalStrengthProfileData.InerfaceMode;
                IsProfileSet = true;
            }
        }

        private void Scan_NetworkCardConnectedDisconnected(object sender, Core.Events.NetworkCardConnectedEventArgs e)
        {
            if (!e.IsNetworkCardUp)
            {
                IsScanning = false;
            }
            IsInterfaceUp = e.IsNetworkCardUp;
        }

        #region SplashScreen
        public bool IsInterfaceUp
        {
            get => isInterfaceUp;
            set { _UpdateField(ref isInterfaceUp, value); }
        }
        private bool isInterfaceUp;
        #endregion


        #region NetInfo
        public bool IsScanning
        {
            get => isScanning;
            set { _UpdateField(ref isScanning, value); }
        }
        private bool isScanning;

        public string Profile
        {
            get => profile;
            set { _UpdateField(ref profile, value); }
        }
        private string profile;

        public string Interface
        {
            get => interfaceDescription;
            set { _UpdateField(ref interfaceDescription, value); }
        }
        private string interfaceDescription;
        public string InterfaceMac
        {
            get => interfaceState;
            set { _UpdateField(ref interfaceState, value); }
        }
        private string interfaceState;
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
