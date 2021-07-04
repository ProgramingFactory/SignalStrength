namespace SignalStrength.Wpf.View
{

    using FirstFloor.ModernUI.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
 
        public MainWindow()
        {
            InitializeComponent();
            App.Current.MainWindow = this;
        }
    }
}
