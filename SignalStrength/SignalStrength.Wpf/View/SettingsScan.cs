namespace SignalStrength.Wpf.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SignalStrength.Wpf.Properties;

    public class SettingsScan : INotifyPropertyChanged
    {
        private static bool initial = false;
        private static SettingsScan _current = default(SettingsScan);
        public static SettingsScan Current => Init();
        private static SettingsScan Init()
        {
            if (!initial)
            {
                _current = new SettingsScan();
                initial = true;
                return _current;
            }
            else return _current;
        }

        private void UpdateValue(int value)
        {
            Current.Interval = value;
            Settings.Default.Global_Interval = value; 
            Settings.Default.Save();
        }

        private void Update3DValue(int value)
        {
            Current.TimeColumns = value;
            Settings.Default.ChosenTimeColumns = value;
            Settings.Default.Save();
        }
        private void Update2DValue(int value)
        {
            Current.Range = value;
            Settings.Default.ChoosenRange = value;
            Settings.Default.Save();
        }

        public int TimeColumns
        {
            get => _timeColums;
            set { UpdateChangedField(ref _timeColums, value, _ => Update3DValue(value)); }
        }
        private int _timeColums;


        public int Interval
        {
            get => _interval;
            set { UpdateChangedField(ref _interval, value, _ => UpdateValue(value)); }
        }
        private int _interval;


        public int Range
        {
            get => _range;
            set { UpdateChangedField(ref _range, value, _ => Update2DValue(value)); }
        }
        private int _range;



        public virtual event PropertyChangedEventHandler PropertyChanged;
        internal void UpdateChangedField<T>(ref T field, T newValue, Action<T> onChangedCallback = null,
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
    }

  
}
