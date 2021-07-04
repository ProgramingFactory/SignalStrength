namespace SignalStrength.Wpf.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Windows.Media;

    using SignalStrength.Wpf.Extensions;

    /// <summary>
    /// This is class LanguageSettingsViewModel
    /// </summary>
    public class LanguageSettingsViewModel : INotifyPropertyChanged
    {
        public System.Globalization.CultureInfo CurrentCulture => System.Globalization.CultureInfo.CurrentCulture;
        public static List<TranslatedLanguages> LanguageList { get; set; } = LanguageList = new List<TranslatedLanguages>()
            {
                new TranslatedLanguages("English", "en", Properties.Resources.en.ConvertBitmapToImageSource())
                ,
                new TranslatedLanguages("Croatian", "hr", Properties.Resources.hr_HR.ConvertBitmapToImageSource())
            };

        public LanguageSettingsViewModel()
        {
            if (!string.IsNullOrEmpty(SavedLanguage))
                ChangeCulture(SavedLanguage);

            SelectedLanguage = LanguageList.Where(l => l.Tag.ToLowerInvariant().Contains(CurrentCulture.TwoLetterISOLanguageName)).Select(x => x).FirstOrDefault();
            if(SelectedLanguage is null)
            {
                SelectedLanguage = new TranslatedLanguages("English", "en", Properties.Resources.en.ConvertBitmapToImageSource());
            }

        }

        private void ChangeCulture(string twoLetterISOLanguageName)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                System.Globalization.CultureInfo newCulture = new System.Globalization.CultureInfo(twoLetterISOLanguageName);
               
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = newCulture;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = newCulture;

                System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
                SavedLanguage = twoLetterISOLanguageName;
                //update culture
                Properties.Resources.Culture = newCulture;
                var lang = Properties.Resources.Language;

            }));
        }

        public bool RestartApp
        {
            get => restartApp;
            set { UpdateField(ref restartApp, value, _ => RestartAppMth()); }
        }
        private bool restartApp;

        private void RestartAppMth()
        {
            if (RestartApp) {
                Process.Start(Assembly.GetEntryAssembly().Location);
                App.Current.Shutdown();
            }
        }



        /// <summary>
        /// Propertie for <see L="AppSettings.UserSettings"/>
        /// </summary>
        public static string SavedLanguage { get; set; } = Properties.Settings.Default.ChosenLanguage;

        public TranslatedLanguages SelectedLanguage
        {
            get => selectedLang;
            set { UpdateField(ref selectedLang, value, _ => ChangeCulture(value.Tag)); }
        }
        private TranslatedLanguages selectedLang;



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        internal void UpdateField<T>(ref T field, T newValue, Action<T> onChangedCallback = null,
                                 [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            T oldValue = field;

            field = newValue;
            onChangedCallback?.Invoke(oldValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }




    /// <summary>
    /// This is interface ITranslatedLanguages
    /// </summary>
    public interface ITranslatedLanguages
    {
        string Culture { get; set; }
        string Tag { get; set; }
        ImageSource ImagePath { get; set; }
    }

    public class TranslatedLanguages : ITranslatedLanguages
    {
        public TranslatedLanguages(string cultureName, string tag, ImageSource imagePath)
        {
            Culture = cultureName ?? throw new ArgumentNullException(nameof(cultureName));
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            ImagePath = imagePath ?? throw new ArgumentNullException(nameof(imagePath));
        }

        public string Culture { get; set; }
        public string Tag { get; set; }
        public ImageSource ImagePath { get; set; }
    }
}
