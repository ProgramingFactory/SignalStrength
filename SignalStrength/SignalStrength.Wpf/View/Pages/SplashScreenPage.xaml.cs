namespace SignalStrength.Wpf.View.Pages
{
    using System.Windows.Controls;



    /// <summary>
    /// Interaction logic for SplashScreenPage.xaml
    /// </summary>
    public partial class SplashScreenPage : UserControl
    {
        public SplashScreenPage()
        {
            InitializeComponent();
            var frame = Extensions.VisualHelper.FindChild<FirstFloor.ModernUI.Windows.Controls.ModernFrame>(App.Current.MainWindow, "ContentFrame");
            if (frame != null)
            {
                frame.Navigating += (s, e) =>{
                    Core.Logging.Logger.Log($"Navigating {e.Source}", Core.Logging.Logger.LogLevel.INFO);
                };
            }
        }
    }
}
