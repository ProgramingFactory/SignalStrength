﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SignalStrength.Wpf.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF007ACC")]
        public global::System.Windows.Media.Color ChosenAccent {
            get {
                return ((global::System.Windows.Media.Color)(this["ChosenAccent"]));
            }
            set {
                this["ChosenAccent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF0098FF")]
        public global::System.Windows.Media.Color ChosenSecondaryAccent {
            get {
                return ((global::System.Windows.Media.Color)(this["ChosenSecondaryAccent"]));
            }
            set {
                this["ChosenSecondaryAccent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/SignalStrength;component/Assets/byProgramingFactory.xaml")]
        public global::System.Uri ChosenTheme {
            get {
                return ((global::System.Uri)(this["ChosenTheme"]));
            }
            set {
                this["ChosenTheme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Large")]
        public global::FirstFloor.ModernUI.Presentation.FontSize ChosenFontSize {
            get {
                return ((global::FirstFloor.ModernUI.Presentation.FontSize)(this["ChosenFontSize"]));
            }
            set {
                this["ChosenFontSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int Global_Interval {
            get {
                return ((int)(this["Global_Interval"]));
            }
            set {
                this["Global_Interval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ChosenLanguage {
            get {
                return ((string)(this["ChosenLanguage"]));
            }
            set {
                this["ChosenLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int ChosenTimeColumns {
            get {
                return ((int)(this["ChosenTimeColumns"]));
            }
            set {
                this["ChosenTimeColumns"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public int ChoosenRange {
            get {
                return ((int)(this["ChoosenRange"]));
            }
            set {
                this["ChoosenRange"] = value;
            }
        }
    }
}
