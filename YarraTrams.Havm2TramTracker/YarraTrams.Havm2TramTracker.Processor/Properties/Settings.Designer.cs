﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YarraTrams.Havm2TramTracker.Processor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=localhost\\SQLEXPRESS;Persist Security Info=False;max pool size=500;In" +
            "tegrated Security=true;database=TramTracker;")]
        public string TramTrackerDB {
            get {
                return ((string)(this["TramTrackerDB"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Havm2TramTrackerAPI {
            get {
                return ((string)(this["Havm2TramTrackerAPI"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public int Havm2TramTrackerAPITimeoutSeconds {
            get {
                return ((int)(this["Havm2TramTrackerAPITimeoutSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\iop\\Downloads\\temp\\HAVM2TTLogs")]
        public string LogFilePath {
            get {
                return ((string)(this["LogFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LogT_Temp_TripRowsToFilePriorToInsert {
            get {
                return ((bool)(this["LogT_Temp_TripRowsToFilePriorToInsert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LogT_Temp_SchedulesRowsToFilePriorToInsert {
            get {
                return ((bool)(this["LogT_Temp_SchedulesRowsToFilePriorToInsert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LogT_Temp_SchedulesMasterRowsToFilePriorToInsert {
            get {
                return ((bool)(this["LogT_Temp_SchedulesMasterRowsToFilePriorToInsert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert {
            get {
                return ((bool)(this["LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("03:00:00")]
        public global::System.TimeSpan DueTime {
            get {
                return ((global::System.TimeSpan)(this["DueTime"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DbTableSuffix {
            get {
                return ((string)(this["DbTableSuffix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public int DBCommandTimeoutSeconds {
            get {
                return ((int)(this["DBCommandTimeoutSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExecuteCopyToLiveAsPartOfDailyProcess {
            get {
                return ((bool)(this["ExecuteCopyToLiveAsPartOfDailyProcess"]));
            }
        }
    }
}
