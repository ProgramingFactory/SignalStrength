namespace SignalStrength.Wpf.View.Pages
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ChannalGraphPage.xaml
    /// </summary>
    public partial class ChannalGraphPage : UserControl/*,FirstFloor.ModernUI.Windows.IContent */
    {

        public ChannalGraphPage()
        {
            InitializeComponent();
            this.sceen.ResetCameraGesture = new MouseGesture(MouseAction.LeftDoubleClick);
            this.sceen.PanGesture = new MouseGesture(MouseAction.LeftClick);
        }
    }
}
