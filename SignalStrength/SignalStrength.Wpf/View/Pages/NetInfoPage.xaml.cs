namespace SignalStrength.Wpf.View.Pages
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for NetInfoPage.xaml
    /// </summary>
    public partial class NetInfoPage : UserControl
    {
        public NetInfoPage()
        {
            InitializeComponent();
            lvNetInfo.SelectionChanged += (sender, args) => { dgNetInfo.SelectedItem = lvNetInfo.SelectedItem; };
            dgNetInfo.SelectionChanged += (sender, args) => { lvNetInfo.SelectedItem = dgNetInfo.SelectedItem; };
        }
    }
}
