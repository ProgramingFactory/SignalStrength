namespace SignalStrength.Wpf.View.Pages
{
    using System;
    using System.Linq;
    using System.Windows.Controls;

    using SignalStrength.Core;


    /// <summary>
    /// Interaction logic for GaugePage.xaml
    /// </summary>
    public partial class GaugePage : UserControl
    {
        //Remember which network is choosen
        private ISignalStrengthData SelectedItem = default(ISignalStrengthData);
        public GaugePage()
        {
            InitializeComponent();
            Model.SignalScan.Current.Refreshed += (s, e) =>
            {

                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ISignalStrengthData selectedItemTemp = default(ISignalStrengthData);
                    if (SelectedItem != null)
                    {
                        selectedItemTemp = lvNames.Items.OfType<SignalStrengthData>().Where(i => i.Name == SelectedItem.Name).FirstOrDefault();
                        var containSelectedItem = lvNames.Items.OfType<SignalStrengthData>().Where(i => i.Name == SelectedItem.Name).Any();

                        if (containSelectedItem is false && selectedItemTemp == null)
                        {
                            GaugeLinkQuality.CurrentValue = 0D;
                            GaugeSigStrenght.CurrentValue = 0D;
                            GaugeRSSI.CurrentValue = -100;
                        }
                        else
                        {
                            GaugeLinkQuality.CurrentValue = selectedItemTemp.LinkQuality;
                            GaugeSigStrenght.CurrentValue = selectedItemTemp.SignalQuality;
                            GaugeRSSI.CurrentValue = selectedItemTemp.RSSI;

                            //set focus back
                            var index = lvNames.Items.IndexOf(selectedItemTemp);
                            lvNames.SelectedItem = lvNames.Items[index];
                        }
                    }
                   
                }));
            };
            lvNames.SelectionChanged += (s, e) =>
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {

                    if (lvNames.SelectedItem != null)
                    {                   
                        var selectedItemTmp = SelectedItem = (SignalStrength.Core.SignalStrengthData)lvNames.SelectedItem;
                        GaugeLinkQuality.CurrentValue = selectedItemTmp.LinkQuality;
                        GaugeSigStrenght.CurrentValue = selectedItemTmp.SignalQuality;
                        GaugeRSSI.CurrentValue = selectedItemTmp.RSSI;
                    }
                    var containSelectedItem = lvNames.Items.OfType<SignalStrengthData>().Where(i => i.Name == SelectedItem.Name).Any();
                    if (containSelectedItem is false && SelectedItem == null)
                    {
                        GaugeLinkQuality.CurrentValue = 0D;
                        GaugeSigStrenght.CurrentValue = 0D;
                        GaugeRSSI.CurrentValue = -100;
                    }
                }));
            };

        }
    }
}
