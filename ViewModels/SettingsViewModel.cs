using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Extender;
using Extender.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PushPost.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        public bool Debug
        {
            get
            {
                return Properties.Settings.Default.DEBUG;
            }
            set
            {
                Properties.Settings.Default.DEBUG = value;
                OnPropertyChanged("Debug");
            }
        }

        public bool CloseConfirmations
        {
            get
            {
                return Properties.Settings.Default.CloseConfirmations;
            }
            set
            {
                Properties.Settings.Default.CloseConfirmations = value;
                OnPropertyChanged("CloseConfirmations");
            }
        }

        public bool AutoInsertMarkup
        {
            get
            {
                return Properties.Settings.Default.AutoInsertMarkup;
            }
            set
            {
                Properties.Settings.Default.AutoInsertMarkup = value;
                OnPropertyChanged("AutoInsertMarkup");
            }
        }

        public bool ConfirmBeforeRemove
        {
            get
            {
                return Properties.Settings.Default.ConfirmBeforeRemove;
            }
            set
            {
                Properties.Settings.Default.ConfirmBeforeRemove = value;
                OnPropertyChanged("ConfirmBeforeRemove");
            }
        }

        public bool IncludeBlogLinks
        {
            get
            {
                return Properties.Settings.Default.IncludeBlogLinks;
            }
            set
            {
                Properties.Settings.Default.IncludeBlogLinks = value;
                OnPropertyChanged("IncludeBlogLinks");
            }
        }

        public string DatabaseFilename
        {
            get
            {
                return Properties.Settings.Default.DBRelativeFilename;
            }
            set
            {
                Properties.Settings.Default.DBRelativeFilename = value;
                OnPropertyChanged("DatabaseFilename");
            }
        }

        public string QueueFolderPath
        {
            get
            {
                return Properties.Settings.Default.QueueFolderPath;
            }
            set
            {
                Properties.Settings.Default.QueueFolderPath = value;
                OnPropertyChanged("QueueFolderPath");
            }
        }

        public string PreviewFolderPath
        {
            get
            {
                return Properties.Settings.Default.PreviewFolderPath;
            }
            set
            {
                Properties.Settings.Default.PreviewFolderPath = value;
                OnPropertyChanged("PreviewFolderPath");
            }
        }

        public string AutosaveLocation
        {
            get
            {
                return Properties.Settings.Default.AutosaveLocation;
            }
            set
            {
                Properties.Settings.Default.AutosaveLocation = value;
                OnPropertyChanged("AutosaveLocation");
            }
        }

        public string AutosaveFileFormat
        {
            get
            {
                return Properties.Settings.Default.AutosaveFilenameFormat;
            }
            set
            {
                Properties.Settings.Default.AutosaveFilenameFormat = value;
                OnPropertyChanged("AutosaveFileFormat");
            }
        }

        public string SiteExportFolder
        {
            get
            {
                return Properties.Settings.Default.SiteExportFolder;
            }
            set
            {
                Properties.Settings.Default.SiteExportFolder = value;
                OnPropertyChanged("SiteExportFolder");
            }
        }

        public string ImagesSubfolder
        {
            get
            {
                return Properties.Settings.Default.ImagesSubfolder;
            }
            set
            {
                Properties.Settings.Default.ImagesSubfolder = value;
                OnPropertyChanged("ImagesSubfolder");
            }
        }
        public string SinglesSubfolder
        {
            get
            {
                return Properties.Settings.Default.SinglesSubfolder;
            }
            set
            {
                Properties.Settings.Default.SinglesSubfolder = value;
                OnPropertyChanged("ImagesSubfolder");
            }
        }

        public int MaxQueueSize
        {
            get
            {
                return Properties.Settings.Default.MaxQueueSize;
            }
            set
            {
                Properties.Settings.Default.MaxQueueSize = value;
                OnPropertyChanged("MaxQueueSize");
            }
        }

        public int PostsPerPage
        {
            get
            {
                return Properties.Settings.Default.PostsPerPage;
            }
            set
            {
                Properties.Settings.Default.PostsPerPage = value;
                OnPropertyChanged("PostsPerPage");
            }
        }

        public string WebsiteName
        {
            get
            {
                return Properties.Settings.Default.WesbiteName;
            }
            set
            {
                Properties.Settings.Default.WesbiteName = value;
                OnPropertyChanged("WebsiteName");
            }
        }

        public ICommand BrowseSiteFolderCommand         { get; private set; }
        public ICommand BrowseQueueFolderCommand        { get; private set; }
        public ICommand BrowsePreviewsFolderCommand     { get; private set; }
        public ICommand BrowseAutosaveFolderCommand     { get; private set; }
        public ICommand BrowseImagesFolderCommand       { get; private set; }

        public SettingsViewModel()
        {
            BrowseSiteFolderCommand     = new RelayCommand(() => this.ChangeSiteFolder());
            BrowseQueueFolderCommand    = new RelayCommand(() => this.ChangeQueueFolder());
            BrowsePreviewsFolderCommand = new RelayCommand(() => this.ChangePreviewsFolder());
            BrowseAutosaveFolderCommand = new RelayCommand(() => this.ChangeAutosaveFolder());
            BrowseImagesFolderCommand   = new RelayCommand(() => this.ChangeImagesFolder());
        }

        public void ChangeImagesFolder()
        {
            string result = ShowFolderSelectDialog("inline images");

            if (string.IsNullOrWhiteSpace(result)) 
                return;
            else 
                this.ImagesSubfolder = result;            
        }

        public void ChangeSiteFolder()
        {
            string result = ShowFolderSelectDialog("the site");

            if (string.IsNullOrWhiteSpace(result)) 
                return;
            else 
                this.SiteExportFolder = result;
        }

        public void ChangeQueueFolder()
        {
            string result = ShowFolderSelectDialog("temporary queue");

            if (string.IsNullOrWhiteSpace(result))
                return;
            else
                this.QueueFolderPath = result;
        }

        public void ChangePreviewsFolder()
        {
            string result = ShowFolderSelectDialog("previews");

            if (string.IsNullOrWhiteSpace(result))
                return;
            else
                this.PreviewFolderPath = result;
        }

        public void ChangeAutosaveFolder()
        {
            string result = ShowFolderSelectDialog("backups");

            if (string.IsNullOrWhiteSpace(result))
                return;
            else
                this.AutosaveLocation = result;
        }

        private string ShowFolderSelectDialog(string purpose)
        {
            var dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;
            dialog.Title = string.Format("Select a folder to save {0} in", purpose);

            CommonFileDialogResult r = dialog.ShowDialog();
            if (r != CommonFileDialogResult.Ok)
                return string.Empty;
            else
                return dialog.FileName;
        }            
        
        //var dialog = new CommonOpenFileDialog();
        //dialog.IsFolderPicker = true;
        //dialog.Title = "Select a folder to save the site in";

        //CommonFileDialogResult r = dialog.ShowDialog();
        //if (r != CommonFileDialogResult.Ok) return;
    }
}
