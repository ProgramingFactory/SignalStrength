namespace SignalStrength.Core
{
    using System;


    /// <summary>
    /// This is interface ISignalStrengthProfileData
    /// </summary>
    public interface ISignalStrengthProfileData
    {
        /// <summary>Network profile name </summary>
        string ProfileName { get; set; }
        /// <summary>Interface state</summary>
        string InerfaceMode { get; set; }
        /// <summary>Inerface description</summary>
        string InerfaceDescription { get; set; }
    }
    
    
    /// <summary>
    /// This is class SignalStrengthProfileData
    /// </summary>
    public class SignalStrengthProfileData : ISignalStrengthProfileData
    {
        public SignalStrengthProfileData(string profileName, string inerfaceDescription, string inerfaceMode)
        {
            ProfileName = profileName ?? throw new ArgumentNullException(nameof(profileName));
            InerfaceDescription = inerfaceDescription ?? throw new ArgumentNullException(nameof(inerfaceDescription));
            InerfaceMode = inerfaceMode ?? throw new ArgumentNullException(nameof(inerfaceMode));
        }

        /// <summary>Network profile name </summary>
        public string ProfileName { get; set; }

        /// <summary>Interface state</summary>
        public string InerfaceMode { get; set; }
        
        /// <summary>Inerface description</summary>
        public string InerfaceDescription { get; set; }
    }
}
