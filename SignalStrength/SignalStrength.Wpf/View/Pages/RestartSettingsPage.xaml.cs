namespace SignalStrength.Wpf.View.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using FirstFloor.ModernUI.Presentation;

    using SignalStrength.Wpf.Properties;

    using Resources = SignalStrength.Wpf.Properties.Resources;

    /// <summary>
    /// Interaction logic for RestartSettingsPage.xaml
    /// </summary>
    public partial class RestartSettingsPage : UserControl
    {
        //TODO: Move to resources
        private static string DialogTitle = Properties.Resources.ResetDialogTitle;
        private static string DialogContent = Properties.Resources.ResetDialogContent;
        private static string SettingsOkMsg = Properties.Resources.ResetMessageOK;
        private static string SettingsCancelMsg = $"{Properties.Resources.ResetMessageCancel1}{Environment.NewLine}{Properties.Resources.ResetMessageCancel2}";

        public RestartSettingsPage()
        {
            InitializeComponent();
            var frame = Extensions.VisualHelper.FindChild<FirstFloor.ModernUI.Windows.Controls.ModernFrame>(App.Current.MainWindow, "ContentFrame");
            if (frame != null)
                frame.Navigating += (s, e) =>
                {
                    this.resultTBL.Text = "";
                    this.resetSettingTBT.IsChecked = false;
                };
        }

        private void ModernToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == ShowDialog())
            {
                //default values
                Settings.Default.ChosenTheme = AppearanceManager.DarkThemeSource;
                Settings.Default.ChosenAccent = (Color)ColorConverter.ConvertFromString("#FF007ACC");
                Settings.Default.ChosenSecondaryAccent = (Color)ColorConverter.ConvertFromString("#FF0098FF");
                Settings.Default.ChosenFontSize = FirstFloor.ModernUI.Presentation.FontSize.Large;
                View.SettingsScan.Current.Interval = Settings.Default.Global_Interval = 10;
                View.SettingsScan.Current.TimeColumns = Settings.Default.ChosenTimeColumns = 20;
                View.SettingsScan.Current.Range = Settings.Default.ChoosenRange = 15;
                Settings.Default.Save();

                this.resultTBL.Text = SettingsOkMsg;
            }
            else
                this.resultTBL.Text = SettingsCancelMsg;
        }

        private MessageBoxResult ShowDialog()
        {
            var dlg = new FirstFloor.ModernUI.Windows.Controls.ModernDialog
            {
                Title = DialogTitle,
                Content = DialogContent
            };
            dlg.Buttons = new Button[] { dlg.OkButton, dlg.CancelButton };
            dlg.ShowDialog();

            return dlg.MessageBoxResult;
        }

 
    }
}
