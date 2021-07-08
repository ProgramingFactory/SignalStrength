namespace SignalStrength.Wpf
{
    using System;
using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    using SignalStrength.Wpf.AppSettings;
    using SignalStrength.Wpf.Properties;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LoggerOnStartup();
            App.Current.Dispatcher.Invoke(() => SetLanguage());
            
            this.Activated += (sender, args) => { UserSettings.AppLoading(); };
            this.Deactivated += (sender, args) => { UserSettings.AppClosing(); };

            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            App.Current.NavigationFailed += (s, e) => { Core.Logging.Logger.Log(e.Exception.Message, Core.Logging.Logger.LogLevel.WARN_EXCEPTION); };
        }
      
        private void ChangeCulture(string twoLetterISOLanguageName)
        {
            Core.Logging.Logger.Log($"App start-ChangedCulture ={twoLetterISOLanguageName}", Core.Logging.Logger.LogLevel.DEBUG_INFO);
            System.Globalization.CultureInfo newCulture = new System.Globalization.CultureInfo(twoLetterISOLanguageName);
            //update culture
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = newCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = newCulture;

            SignalStrength.Wpf.Properties.Resources.Culture = newCulture;

            Settings.Default.ChosenLanguage = View.LanguageSettingsViewModel.SavedLanguage = newCulture.TwoLetterISOLanguageName;
            Settings.Default.Save();
        }

        private void SetLanguage()
        {
            if (!string.IsNullOrEmpty(View.LanguageSettingsViewModel.SavedLanguage))
                ChangeCulture(SignalStrength.Wpf.Properties.Settings.Default.ChosenLanguage);
            else
            {
                var lang = System.Windows.Input.InputLanguageManager.Current.CurrentInputLanguage.TwoLetterISOLanguageName;
                if (View.LanguageSettingsViewModel.LanguageList.Where(x => x.Tag == lang).Any())
                   ChangeCulture(lang);
                else
                   ChangeCulture("en");
            }
        }

        /// <summary>
        /// Log and shut down
        /// </summary>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Core.Logging.Logger.Log(e.Exception.StackTrace, Core.Logging.Logger.LogLevel.FATAL_EXCEPTION);
            Core.Logging.Logger.Log(e.Exception.Message, Core.Logging.Logger.LogLevel.FATAL_EXCEPTION);

            if (e.Exception.InnerException != null)
                Core.Logging.Logger.Log($"InnerException = {e.Exception.InnerException.Message}", Core.Logging.Logger.LogLevel.FATAL_EXCEPTION);
            if (!SignalStrength.Core.SignalStrengthScan.RadioConnected())
                Core.Logging.Logger.Log("IsRadioOn: false", Core.Logging.Logger.LogLevel.DEBUG_INFO);

            Core.Logging.Logger.Log("Aplication is exit with FATAL EXCEPTION !!", Core.Logging.Logger.LogLevel.APPLICATION_STOPPED);
            Core.Logging.Logger.UnderlineToday();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Log App Start
        /// </summary>
        /// <param name="e"></param>
        private void LoggerOnStartup()
        {
#if !DEBUG
            SignalStrength.Core.Logging.Logger.DelateLogsOlderOFXXXDays(30);
#endif
            SignalStrength.Core.Logging.Logger.UnderlineToday();
            SignalStrength.Core.Logging.Logger.Log("Application is start ...", SignalStrength.Core.Logging.Logger.LogLevel.APPLICATION_STARTED);

        }

        /// <summary>
        /// Log App Exit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            SignalStrength.Core.Logging.Logger.Log("Aplication is exit ...", SignalStrength.Core.Logging.Logger.LogLevel.APPLICATION_STOPPED);
            SignalStrength.Core.Logging.Logger.UnderlineToday();
        }
    }
}


