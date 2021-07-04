namespace SignalStrength.Core.Events
{
    using System;

    /// <summary>
    /// This is interface INetworkCardConnected
    /// </summary>
    public interface INetworkCardConnected
    {
        event EventHandler<NetworkCardConnectedEventArgs> NetworkCardConnectedDisconnected;
    }

    /// <summary>
    /// This is class NetworkCardConnectedEvenArgs
    /// </summary>
    public class NetworkCardConnectedEventArgs : EventArgs
    {
        public NetworkCardConnectedEventArgs(bool isNetworkCardUp)
        {
            IsNetworkCardUp = isNetworkCardUp;
        }

        public bool IsNetworkCardUp { get; set; }
    } 
}

