using Extender.Debugging;
using Extender.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Commands;
using PushPost.Models.HtmlGeneration;
using System;
using System.Linq;
using System.IO;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class PostViewModel : Extender.WPF.ViewModel
    {
        public View.ArchiveManager  ArchiveManager_Window;
        public View.AddRefsDialog   AddRef_Window;
        public View.SettingsEditor  SettingsEditor_Window;

        private short   _LastAutosaveIndex;
        private Post    _Post;
        public Post Post
        {
            get
            {
                return _Post;
            }
            private set
            {
                this._Post = value;
                OnPropertyChanged("Post");
            }
        }
        public NavCategory[] CategoriesList
        {
            get
            {
                return NavCategory.AllCategories;
            }
        }
        public Models.Database.ArchiveQueue ArchiveQueue
        {
            get;
            set;
        }

        #region ICommands
        public ICommand SubmitPostCommand           { get; private set; }
        public ICommand SubmitQueueCommand          { get; private set; }
        public ICommand QueuePostCommand            { get; private set; }
        public ICommand RemovePostCommand           { get; private set; }
        public ICommand DiscardCommand              { get; private set; }
        public ICommand AddIResourceCommand         { get; private set; }
        public ICommand AddFootnoteCommand          { get; private set; }

        public ICommand ImportFromFileCommand       { get; private set; }
        public ICommand ExportToFileCommand         { get; private set; }
        public ICommand PreviewInBrowserCommand     { get; private set; }
        public ICommand OpenArchiveManagerCommand   { get; private set; }
        public ICommand OpenPageGeneratorCommand    { get; private set; }
        public ICommand ViewReferencesCommand       { get; private set; }
        public ICommand ViewFootnotesCommand        { get; private set; }
        public ICommand CreateSiteCommand           { get; private set; }

        public ICommand EditSettingsCommand         { get; private set; }
        public ICommand DisplayAboutCommand         { get; private set; }
        public ICommand OpenHelpDocsCommand         { get; private set; }

        protected System.Windows.Threading.DispatcherTimer AutosaveTimer =
              new System.Windows.Threading.DispatcherTimer
              {
                  Interval  = new TimeSpan(0, 0, 2),
                  IsEnabled = false
              };
        #endregion

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel()
        {
            base.Initialize();

            this._Post              = TextPost.TemplatePost();
            this.ArchiveQueue       = new Models.Database.ArchiveQueue();
            this._LastAutosaveIndex = 0;

            // Post buttons' commands
            this.QueuePostCommand       = new QueuePostCommand(this);
            this.SubmitPostCommand      = new SubmitPostCommand(this);
            this.RemovePostCommand      = new RemovePostCommand(this);
            this.SubmitQueueCommand     = new SubmitQueueCommand(this);
            this.DiscardCommand         = new DiscardNewPostCommand(this);
            this.AddIResourceCommand    = new AddIResourceCommand(this);
            this.AddFootnoteCommand     = new AddFootnoteCommand(this);

            // Menu toolbar commands
            this.ImportFromFileCommand      = new ImportFromFileCommand(this);
            this.ExportToFileCommand        = new ExportToFileCommand(this);
            this.PreviewInBrowserCommand    = new PreviewInBrowserCommand(this);
            this.OpenArchiveManagerCommand  = new OpenArchiveManagerCommand(this);
            this.OpenPageGeneratorCommand   = new OpenPageGeneratorCommand(this);
            this.ViewReferencesCommand      = new ViewReferencesCommand(this);
            this.ViewFootnotesCommand       = new ViewFootnotesCommand(this);
            this.CreateSiteCommand          = new RelayCommand(() => this.CreateSite());

            this.EditSettingsCommand    = new RelayCommand(() => this.EditSettings());
            this.DisplayAboutCommand    = new RelayCommand(() => this.DisplayAbout());
            this.OpenHelpDocsCommand    = new RelayCommand(() => this.OpenHelpDocs());

            Post.PropertyChanged            += PostViewModel_PropertyChanged;
            this.AutosaveTimer.Tick         += AutosaveTimer_Tick;
        }


        /// <summary>
        /// Initializes a new isntance of the PostViewModel class.
        /// </summary>
        /// <param name="category">Category of Post the view is to be bound with.</param>
        public PostViewModel(NavCategory category):this()
        {
            InitializeByCategory(category);
        }

        /// <summary>
        /// Initializes a new instance of the PostViewModel class.
        /// </summary>
        /// <param name="type">Type of Post the view is to be bound with.</param>
        public PostViewModel(Type type):this()
        {
            InitializeByType(type);
        }

        private void InitializeByType(Type type)
        {
            if      (type == typeof(TextPost))
                _Post = TextPost.TemplatePost();
            else if (type == typeof(AlbumPost))
                _Post = AlbumPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }

        private void InitializeByCategory(NavCategory category)
        {
            if (category == NavCategory.Blog || category == NavCategory.Code || category == NavCategory.Contact)
                _Post = TextPost.TemplatePost();
            else if (category == NavCategory.Photography)
                _Post = AlbumPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }
        public void OpenHelpDocs()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented.");
        }

        public void DisplayAbout()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented.");
        }

        public void EditSettings()
        {
            if(SettingsEditor_Window != null)
            {
                SettingsEditor_Window.Focus();
                return;
            }

            SettingsEditor_Window           = new View.SettingsEditor();
            SettingsEditor_Window.Closed    += SettingsEditor_Closed;

            SettingsEditor_Window.Show();
        }

        protected void SettingsEditor_Closed(object sender, EventArgs e)
        {
            SettingsEditor_Window = null;
        }

        private void AutosaveTimer_Tick(object sender, EventArgs e)
        {
            this.Autosave();
            this.AutosaveTimer.IsEnabled = false;
        }

        private void PostViewModel_PropertyChanged(
            object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "MainText")
            {
                this.AutosaveTimer.IsEnabled = true;
                this.AutosaveTimer.Stop();
                this.AutosaveTimer.Start();
            }
        }

        public void OpenArchiveManager()
        {
            if (ArchiveManager_Window != null)
            { 
                ArchiveManager_Window.Focus();
                return;
            }

            ArchiveManagerOpen              = true;
            ArchiveManager_Window           = new View.ArchiveManager(ArchiveQueue);
            ArchiveManager_Window.Closed   += ArchiveManager_Closed;

            ArchiveManager_Window.Show();
        }

        protected void ArchiveManager_Closed(object sender, EventArgs e)
        {
            ArchiveManagerOpen      = false;
            ArchiveManager_Window   = null;
        }

        private bool _ArchiveManagerOpen;
        public bool ArchiveManagerOpen
        {
            get
            {
                return _ArchiveManagerOpen; ;
            }
            protected set
            {
                _ArchiveManagerOpen = value;
                OnPropertyChanged("ArchiveManagerOpen");
            }
        }

        public bool HasChildrenOpen
        {
            get
            {
                return (ArchiveManager_Window   != null) ||
                       (AddRef_Window           != null) ||
                       (SettingsEditor_Window   != null);
            }
        }

        public void CloseChildren()
        {
            if (AddRef_Window != null)
                AddRef_Window.Close();

            if (ArchiveManager_Window != null)
                ArchiveManager_Window.Close();

            if (SettingsEditor_Window != null)
                SettingsEditor_Window.Close();
        }

        public bool CanSubmitPost 
        { 
            get
            {
                Post def = TextPost.TemplatePost();
                return !(
                    this.Post.Title == def.Title &&
                    this.Post.Author == def.Author && 
                    this.Post.MainText == def.MainText);
            }
        }

        public bool CanRemovePost
        {
            get
            {
                return true;
            }
        }

        public bool CanAddResource
        {
            get
            {
                return this.Post != null;
            }
        }

        public bool HasReferences
        {
            get
            {
                return this.Post.Resources.Count() > 0; 
            }
        }

        public bool HasFootnotes
        {
            get
            {
                return this.Post.Footers.Count() > 0;
            }
        }

        public bool MenuToolbarCanExecute
        {
            get { return true; }
        }

        public void QueuePostForSubmit()
        {
            this.ArchiveQueue.Enqueue(this.Post);

            this.Post = TextPost.TemplatePost();
        }

        public void QueueForRemoval()
        {
            System.Windows.Forms.MessageBox.Show("Removing post.");
        }

        public void SubmitQueue()
        {
            System.Windows.Forms.MessageBox.Show("Submitted queue");
        }

        public void OpenAddRefsDialog(int startIndex)
        {
            if (AddRef_Window != null)
            {
                AddRef_Window.Focus();
                return;
            }

            AddRef_Window           = new View.AddRefsDialog(this.Post, startIndex);
            AddRef_Window.Closed   += AddRef_Window_Closed;

            AddRef_Window.Show();
        }

        protected void AddRef_Window_Closed(object sender, EventArgs e)
        {
            AddRef_Window = null;
        }

        public void OpenAddFootnoteDialog()
        {
            System.Windows.Forms.MessageBox.Show("Footnotes not implemented.");
        }

        public void SubmitNow()
        {
            if (Extender.WPF.ConfirmationDialog.Show("Confirm", "Add this post to the database as-is?"))
            {
                try
                {
                    using(Models.Database.Archive database = new Models.Database.Archive())
                    {
                        database.CommitPost(this.Post);
                        database.SubmitChanges();
                    }
                }
                catch (Exception e)
                {
                    Extender.Debugging.ExceptionTools.WriteExceptionText(e, true,
                        "Failed to submit post to the databse.");
                    return;
                }

                this.Post = TextPost.TemplatePost();
            }
        }

        public void Discard()
        {
            InitializeByType(_Post.GetType());
            System.Windows.Forms.MessageBox.Show("Discarded post.");
        }

        public void PreviewInBrowser()
        {
            Site.Preview(new Post[] { this.Post });
        }

        public void OpenPageGenerator()
        {
            throw new NotImplementedException();
        }

        public void ViewReferences()
        {
            throw new NotImplementedException();
        }

        public void ViewFootnotes()
        {
            throw new NotImplementedException();
        }

        public void ImportFromFile()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = @"XML documents (*.txt, *.xml)
                |*.txt;*.xml|All files (*.*)|*.*";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                Post deserialized = Post.Deserialize(dialog.FileName);

                if (deserialized != null)
                {
                    this.Post = deserialized;
                    if (DEBUG) Debug.WriteMessage("Imported: " + this._Post.ToString(), "info");
                }
            }
            else return;
        }

        public void ExportToFile()
        {
            string savePath;

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title        = "Save Post as...";
            dialog.DefaultExt   = ".xml";
            dialog.FileName     = string.Format("{0}.xml", this.Post.Title);

            Nullable<bool> result = dialog.ShowDialog();

            if  (result == true) savePath = dialog.FileName;
            else return;

            using(StreamWriter stream = File.CreateText(savePath))
                Extender.WPF.CompletedMessagebox.Show(Export(stream));
        }

        public bool Export(StreamWriter stream)
        {
            bool success;
            try
            {

                this.Post.Serialize(stream);
                success = true;
            }
            catch (Exception e)
            {
                success = false;
                ExceptionTools.WriteExceptionText(e, true);
            }

            return success;
        }

        protected void Autosave()
        {            
            using(StreamWriter stream = File.CreateText(GenerateAutosaveName()))
            {
                Export(stream);
            }
        }

        protected string GenerateAutosaveName()
        {
            if(!Directory.Exists(AUTOSAVE_LOCATION))
                Directory.CreateDirectory(AUTOSAVE_LOCATION);

            if (_LastAutosaveIndex >= 5)
                _LastAutosaveIndex = 0;

            return GenerateAutosaveName(_LastAutosaveIndex++);
        }

        protected string GenerateAutosaveName(int fileNum)
        {
            return Path.Combine(AUTOSAVE_LOCATION, string.Format(
                AUTOSAVE_FILENAME_FORMAT, fileNum.ToString("D2")));
        }

        public void CreateSite()
        {
            Site.Create();
        }

        public bool DEBUG 
        { 
            get
            {
                return Properties.Settings.Default.DEBUG;
            }
        }

        public string AUTOSAVE_LOCATION
        {
            get
            {
                return Properties.Settings.Default.AutosaveLocation;
            }
        }

        public string AUTOSAVE_FILENAME_FORMAT
        {
            get
            {
                return Properties.Settings.Default.AutosaveFilenameFormat;
            }
        }
    }
}
