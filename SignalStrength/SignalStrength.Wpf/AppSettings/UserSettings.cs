namespace SignalStrength.Wpf.AppSettings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using FirstFloor.ModernUI.Presentation;
using SignalStrength.Wpf.Properties;

    public interface IUserSettings
    {
        void AppLoading();
        void AppClosing();
    }

    public class UserSettings
    {

        public static void AppLoading()
        {
            if (Settings.Default.ChosenTheme != null)
                AppearanceManager.Current.ThemeSource = Settings.Default.ChosenTheme;
            else
                AppearanceManager.Current.ThemeSource = View.SettingsAppearanceViewModel.DefaultTheme;
            //I've pre-set the values for these so null isn't an option
            AppearanceManager.Current.FontSize = Settings.Default.ChosenFontSize;
            AppearanceManager.Current.AccentColor = Settings.Default.ChosenAccent;
            AppearanceManager.Current.SecondaryAccentColor = Settings.Default.ChosenSecondaryAccent;
            Wpf.View.SettingsScan.Current.Interval = Settings.Default.Global_Interval;
            Wpf.View.LanguageSettingsViewModel.SavedLanguage = Settings.Default.ChosenLanguage;
            Wpf.View.SettingsScan.Current.TimeColumns = Settings.Default.ChosenTimeColumns;
            Wpf.View.SettingsScan.Current.Range = Settings.Default.ChoosenRange;
        }

        public static void AppClosing()
        {
            Settings.Default.ChosenTheme = AppearanceManager.Current.ThemeSource;
            Settings.Default.ChosenFontSize = AppearanceManager.Current.FontSize;
            Settings.Default.ChosenAccent = AppearanceManager.Current.AccentColor;
            Settings.Default.ChosenSecondaryAccent = AppearanceManager.Current.SecondaryAccentColor;
            Settings.Default.Global_Interval = View.SettingsScan.Current.Interval;
            Settings.Default.ChosenLanguage = View.LanguageSettingsViewModel.SavedLanguage;
            Settings.Default.ChosenTimeColumns = View.SettingsScan.Current.TimeColumns;
            Settings.Default.ChoosenRange = View.SettingsScan.Current.Range;
            Settings.Default.Save();
        }


    }
}
