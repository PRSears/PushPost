﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PushPost.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("refs.temp.xml")]
        public string TempReferenceFilename {
            get {
                return ((string)(this["TempReferenceFilename"]));
            }
            set {
                this["TempReferenceFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DEBUG {
            get {
                return ((bool)(this["DEBUG"]));
            }
            set {
                this["DEBUG"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CloseConfirmations {
            get {
                return ((bool)(this["CloseConfirmations"]));
            }
            set {
                this["CloseConfirmations"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoInsertMarkup {
            get {
                return ((bool)(this["AutoInsertMarkup"]));
            }
            set {
                this["AutoInsertMarkup"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public int MaxQueueSize {
            get {
                return ((int)(this["MaxQueueSize"]));
            }
            set {
                this["MaxQueueSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ConfirmBeforeRemove {
            get {
                return ((bool)(this["ConfirmBeforeRemove"]));
            }
            set {
                this["ConfirmBeforeRemove"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("posts_(alpha0.2)_10.mdf")]
        public string DBRelativeFilename {
            get {
                return ((string)(this["DBRelativeFilename"]));
            }
            set {
                this["DBRelativeFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("queue")]
        public string QueueFolderPath {
            get {
                return ((string)(this["QueueFolderPath"]));
            }
            set {
                this["QueueFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("previews")]
        public string PreviewFolderPath {
            get {
                return ((string)(this["PreviewFolderPath"]));
            }
            set {
                this["PreviewFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("autosaves")]
        public string AutosaveLocation {
            get {
                return ((string)(this["AutosaveLocation"]));
            }
            set {
                this["AutosaveLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("autosaved_{0}.xml")]
        public string AutosaveFilenameFormat {
            get {
                return ((string)(this["AutosaveFilenameFormat"]));
            }
            set {
                this["AutosaveFilenameFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SiteExportFolder {
            get {
                return ((string)(this["SiteExportFolder"]));
            }
            set {
                this["SiteExportFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int PostsPerPage {
            get {
                return ((int)(this["PostsPerPage"]));
            }
            set {
                this["PostsPerPage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("images")]
        public string ImagesSubfolder {
            get {
                return ((string)(this["ImagesSubfolder"]));
            }
            set {
                this["ImagesSubfolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>480</string>
  <string>600</string>
  <string>775</string>
  <string>1080</string>
  <string>1440</string>
  <string>1600</string>
  <string>0</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ImageSizes {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ImageSizes"]));
            }
            set {
                this["ImageSizes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("patricksears.ca")]
        public string WesbiteName {
            get {
                return ((string)(this["WesbiteName"]));
            }
            set {
                this["WesbiteName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("single")]
        public string SinglesSubfolder {
            get {
                return ((string)(this["SinglesSubfolder"]));
            }
            set {
                this["SinglesSubfolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IncludeBlogLinks {
            get {
                return ((bool)(this["IncludeBlogLinks"]));
            }
            set {
                this["IncludeBlogLinks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool TidyHTML {
            get {
                return ((bool)(this["TidyHTML"]));
            }
            set {
                this["TidyHTML"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("debug_log.txt")]
        public string DebugLogPath {
            get {
                return ((string)(this["DebugLogPath"]));
            }
            set {
                this["DebugLogPath"] = value;
            }
        }
    }
}
