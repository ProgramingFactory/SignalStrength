namespace SignalStrength.Core.Events
{
    using System;

    using SignalStrength.Core.Extensions;


    /// <summary>
    /// This is interface IScanRefreshed
    /// </summary>
    public interface IScanNetworkRefreshed
    {
        event EventHandler<ScanRefreshedEventArgs> ScanNetworkCollectionRefreshed;
    }


    /// <summary>
    /// This is class ScanRefreshed
    /// </summary>
    public class ScanRefreshedEventArgs:EventArgs
    {
        public ScanRefreshedEventArgs(MultiThreadObservableCollection<ISignalStrengthData> signalStrengthDataColl)
        {
            SignalStrengthDataColl = signalStrengthDataColl ?? throw new ArgumentNullException(nameof(signalStrengthDataColl));
        }

        public Extensions.MultiThreadObservableCollection<ISignalStrengthData> SignalStrengthDataColl { get; set; }
    }
}

