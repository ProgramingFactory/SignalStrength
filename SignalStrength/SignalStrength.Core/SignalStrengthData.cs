namespace SignalStrength.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Controls;


/// <summary>
/// This is interface ISignalStrengthData
/// </summary>
    public interface ISignalStrengthData
    {
        /// <summary>Network name </summary>
        string Name { get; set; }
        /// <summary> Signal quality (0-100) </summary>
        int SignalQuality { get; set; }
        /// <summary>Link quality (0-100)</summary>
        int LinkQuality { get; set; }
        /// <summary>Signal strength (RSSI)</summary>
        int RSSI { get; set; }
        /// <summary>Channel</summary>
        int Channel { get; set; }
        /// <summary>Frequency band (GHz) </summary>
        float NetBand { get; set; }
        /// <summary>Network frequency (KHz) </summary>
        int Frequency { get; set; }
        /// <summary>Media Access Control (MAC) address </summary>
        string MacAddress { get; set; }
    }

    /// <summary>
    /// This is class SignalStrengthData
    /// </summary>
    public class SignalStrengthData : ISignalStrengthData, IEquatable<SignalStrengthData>
    {
        public SignalStrengthData(string name, int signalQuality, int frequency, 
                                  float netBand, int linkQuality, int rSSI, int channel,
                                  string macAddress)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SignalQuality = signalQuality;
            Frequency = frequency;
            NetBand = netBand;
            LinkQuality = linkQuality;
            RSSI = rSSI;
            Channel = channel;
            MacAddress = macAddress ?? throw new ArgumentNullException(nameof(macAddress));
        }
       
        /// <summary>Network name </summary>
        public string Name { get; set; }

        /// <summary> Signal quality (0-100) </summary>
        public int SignalQuality { get; set; }

        /// <summary>Link quality (0-100)</summary>
        public int LinkQuality { get; set; }

        /// <summary>Signal strength (RSSI)</summary>
        public int RSSI { get; set; }

        /// <summary>Channel</summary>
        public int Channel { get; set; }

        /// <summary>Frequency band (GHz) </summary>
        public float NetBand { get; set; }

        /// <summary>Network frequency (KHz) </summary>
        public int Frequency { get; set; }

        /// <summary>Media Access Control (MAC) address </summary>
        public string MacAddress { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SignalStrengthData);
        }

        public bool Equals(SignalStrengthData other)
        {
            return other != null &&
                   Name == other.Name &&
                   SignalQuality == other.SignalQuality &&
                   LinkQuality == other.LinkQuality &&
                   RSSI == other.RSSI &&
                   Channel == other.Channel &&
                   NetBand == other.NetBand &&
                   Frequency == other.Frequency &&
                   MacAddress == other.MacAddress;
        }

        public override int GetHashCode()
        {
            int hashCode = 822698484;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + SignalQuality.GetHashCode();
            hashCode = hashCode * -1521134295 + LinkQuality.GetHashCode();
            hashCode = hashCode * -1521134295 + RSSI.GetHashCode();
            hashCode = hashCode * -1521134295 + Channel.GetHashCode();
            hashCode = hashCode * -1521134295 + NetBand.GetHashCode();
            hashCode = hashCode * -1521134295 + Frequency.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MacAddress);
            return hashCode;
        }
    }
}
