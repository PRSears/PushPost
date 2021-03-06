﻿using Extender.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Input;

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
                OnPropertyChanged("DebugVisibility");
            }
        }

        public string DebugFilePath
        {
            get
            {
                return Properties.Settings.Default.DebugLogPath;
            }
            set
            {
                Properties.Settings.Default.DebugLogPath = value;
                OnPropertyChanged("DebugFilePath");
            }
        }

        public System.Windows.Visibility DebugVisibility
        {
            get
            {
                return Debug ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

        public bool DefaultToBatchPhotoAdd
        {
            get
            {
                return Properties.Settings.Default.DefaultToBatchPhotoAdd;
            }
            set
            {
                Properties.Settings.Default.DefaultToBatchPhotoAdd = value;
                OnPropertyChanged("DefaultToBatchPhotoAdd");
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

        public bool TidyHTML
        {
            get
            {
                return Properties.Settings.Default.TidyHTML;
            }
            set
            {
                Properties.Settings.Default.TidyHTML = value;
                OnPropertyChanged("TidyHTML");
            }
        }

        public bool AutoReplaceImages
        {
            get
            {
                return Properties.Settings.Default.AutoReplaceImages;
            }
            set
            {
                Properties.Settings.Default.AutoReplaceImages = value;
                OnPropertyChanged("AutoReplaceImages");
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

        public string PhotosSubfolder
        {
            get
            {
                return Properties.Settings.Default.PhotosSubfolder;
            }
            set
            {
                Properties.Settings.Default.PhotosSubfolder = value;
                OnPropertyChanged("PhotosSubfolder");
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

        public int PhotoPostsPerPage
        {
            get
            {
                return Properties.Settings.Default.PhotoPostsPerPage;
            }
            set
            {
                Properties.Settings.Default.PhotoPostsPerPage = value;
                OnPropertyChanged("PhotoPostsPerPage");
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
        public ICommand BrowsePhotosFolderCommand       { get; private set; }

        public SettingsViewModel()
        {
            BrowseSiteFolderCommand     = new RelayCommand(() => this.ChangeSiteFolder());
            BrowseQueueFolderCommand    = new RelayCommand(() => this.ChangeQueueFolder());
            BrowsePreviewsFolderCommand = new RelayCommand(() => this.ChangePreviewsFolder());
            BrowseAutosaveFolderCommand = new RelayCommand(() => this.ChangeAutosaveFolder());
            BrowseImagesFolderCommand   = new RelayCommand(() => this.ChangeImagesFolder());
            BrowsePhotosFolderCommand   = new RelayCommand(() => this.ChangePhotosFolder());
        }

        public void ChangeImagesFolder()
        {
            string result = ShowFolderSelectDialog("inline images");

            if (string.IsNullOrWhiteSpace(result)) 
                return;
            else 
                this.ImagesSubfolder = result;            
        }

        public void ChangePhotosFolder()
        {
            string result = ShowFolderSelectDialog("photo gallery");

            if (string.IsNullOrWhiteSpace(result))
                return;
            else
                this.PhotosSubfolder = result;
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
    }
}
