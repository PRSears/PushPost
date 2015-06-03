using Extender.Debugging;
using Extender.WPF;
using PushPost.Models.HtmlGeneration;
using System;
using System.IO;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class PostViewModel : Extender.WPF.ViewModel
    {
        private short   _LastAutosaveIndex;
        private Post    _Post;

        protected WindowManager WindowManager;
        protected System.Windows.Threading.DispatcherTimer AutosaveTimer =
              new System.Windows.Threading.DispatcherTimer
              {
                  Interval  = new TimeSpan(0, 0, 2),
                  IsEnabled = false
              };

        public Post Post
        {
            get
            {
                return _Post;
            }
            set
            {
                this._Post = value;
                this._Post.PropertyChanged += Post_PropertyChanged;
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
        public ICommand QueuePostCommand            { get; private set; }
        public ICommand AddIResourceCommand         { get; private set; }
        public ICommand AddPhotoCommand             { get; private set; }
        public ICommand AddFootnoteCommand          { get; private set; }
        public ICommand ManageTagsCommand           { get; private set; }

        public ICommand DiscardPostCommand          { get; private set; }
        public ICommand QuickSwitchCodeCommand      { get; private set; }
        public ICommand QuickSwitchPhotoCommand     { get; private set; }

        public ICommand ImportFromFileCommand       { get; private set; }
        public ICommand ExportToFileCommand         { get; private set; }
        public ICommand PreviewInBrowserCommand     { get; private set; }
        public ICommand OpenArchiveManagerCommand   { get; private set; }
        public ICommand ViewReferencesCommand       { get; private set; }
        public ICommand CreateSiteCommand           { get; private set; }

        public ICommand EditSettingsCommand         { get; private set; }
        public ICommand DisplayAboutCommand         { get; private set; }
        public ICommand OpenHelpDocsCommand         { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel()
        {
            base.Initialize();

            this.Post               = TextPost.TemplatePost();
            this.ArchiveQueue       = new Models.Database.ArchiveQueue();
            this.WindowManager      = new WindowManager();
            this._LastAutosaveIndex = 0;

            // Post buttons' commands
            this.QueuePostCommand   = new RelayCommand
            (
                () =>
                {
                    this.ArchiveQueue.Enqueue(this.Post);
                    this.Post.ResetToTemplate();
                },
                () => (!this.PostIsDefault) && !(this.Post is PhotoPost)
            );
            this.SubmitPostCommand  = new RelayCommand
            (
                () => this.SubmitNow(),
                () => !this.PostIsDefault
            );

            // Menu toolbar commands
            this.ImportFromFileCommand      = new RelayCommand(() => this.ImportFromFile());
            this.ExportToFileCommand        = new RelayCommand(() => this.ExportToFile());
            this.PreviewInBrowserCommand    = new RelayCommand(() => Site.Preview(this.Post));
            this.CreateSiteCommand          = new RelayCommand(() => Site.Create());

            // Post toolbar menu commands 
            this.DiscardPostCommand = new RelayCommand
            (
                () => 
                {
                    if(Extender.WPF.ConfirmationDialog.Show("Discard post", "Are you sure you want to discard the entire post?"))
                        this.Post.ResetToTemplate();
                }
            );

            this.QuickSwitchCodeCommand = new RelayCommand
            (
                () => this.Post.Category = NavCategory.Code
            );

            this.QuickSwitchPhotoCommand = new RelayCommand
            (
                () => this.Post.Category = NavCategory.Photography
            );

            // New window commands
            this.EditSettingsCommand = new RelayCommand
            (
                () => WindowManager.OpenWindow(new View.SettingsEditor())
            );

            this.OpenArchiveManagerCommand = new RelayCommand
            (
                () => WindowManager.OpenWindow(new View.ArchiveManager(this.ArchiveQueue))
            );

            this.ViewReferencesCommand = new RelayCommand
            (
                () => WindowManager.OpenWindow(new View.ViewRefs(this.Post))
            );

            this.AddIResourceCommand = new RelayFunction
            (
                (parameter) => this.AddReference(parameter)
            );

            this.AddPhotoCommand = new RelayCommand
            (
                () =>
                {
                    if (Properties.Settings.Default.DefaultToBatchPhotoAdd)
                        WindowManager.OpenWindow(new View.BatchPhotoAdder(this.Post));
                    else
                        this.AddReference(4);
                },
                () => this.Post.Category == NavCategory.Photography
            );

            this.AddFootnoteCommand     = new RelayCommand(
                () => this.AddReference(3)); 

            this.DisplayAboutCommand    = new RelayCommand(
                () => WindowManager.OpenWindow(new View.About()));

            this.OpenHelpDocsCommand    = new RelayCommand(
                () => System.Windows.Forms.MessageBox.Show("Not implemented."),
                () => false);
            this.ManageTagsCommand = new RelayCommand(
                () => System.Windows.Forms.MessageBox.Show("Not implemented."),
                () => false);

            //this.Post.PropertyChanged       += Post_PropertyChanged; // subscribing in Post's setter instead
            this.AutosaveTimer.Tick         += AutosaveTimer_Tick;
            this.WindowManager.WindowClosed += WindowManager_WindowClosed;
            this.WindowManager.WindowOpened += WindowManager_WindowOpened;
        }

        #region Constructor overloads
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
                Post = TextPost.TemplatePost();
            else if (type == typeof(PhotoPost))
                Post = PhotoPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }

        private void InitializeByCategory(NavCategory category)
        {
            if (category == NavCategory.Blog || category == NavCategory.Code || category == NavCategory.Contact)
                Post = TextPost.TemplatePost();
            else if (category == NavCategory.Photography)
                Post = PhotoPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }
        #endregion

        public bool HasChildrenOpen
        {
            get
            {
                return this.WindowManager.ChildOpen();
            }
        }

        public bool IsPhoto
        {
            get
            {
                return this.Post.Category.Equals(NavCategory.Photography);
            }
        }

        public System.Windows.Visibility AddPhotoButtonVisible
        {
            get
            {
                return IsPhoto ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }

        public void CloseChildren()
        {
            this.WindowManager.CloseAll();
        }

        private void WindowManager_WindowOpened(object sender, System.Windows.Window window)
        {
            if      (window is View.ArchiveManager)
                this.ArchiveManagerOpen = true;

            else if (window is View.ViewRefs)
                this.ViewRefsOpen = true;
        }

        private void WindowManager_WindowClosed(object sender, Type windowType)
        {
            if      (windowType == typeof(View.ArchiveManager))
                this.ArchiveManagerOpen = false;

            else if (windowType == typeof(View.ViewRefs))
                this.ViewRefsOpen = false;
        }

        protected bool _ArchiveManagerOpen;
        public bool ArchiveManagerOpen
        {
            get
            {
                return _ArchiveManagerOpen;
            }
            set
            {
                _ArchiveManagerOpen = value;
                OnPropertyChanged("ArchiveManagerOpen");
            }
        }

        protected bool _ViewRefsOpen;
        public bool ViewRefsOpen
        {
            get
            {
                return _ViewRefsOpen;
            }
            set
            {
                _ViewRefsOpen = value;
                OnPropertyChanged("ViewRefsOpen");
            }
        }

        public bool PostIsDefault
        {
            get
            {
                // THOUGHT Since the default post changes depending on post type,
                //         this should be handled differently.
                Post def = TextPost.TemplatePost();
                return this.Post.Title == def.Title &&
                       this.Post.Author == def.Author &&
                       this.Post.MainText == def.MainText;
            }
        }

        public bool AddReference(object parameter)
        {

            int startIndex = -1;

            if (parameter is int)
                startIndex = (int)parameter;
            else if (parameter is string)
                int.TryParse((string)parameter, out startIndex);

            if (startIndex < 0)
                startIndex = 0;

            View.AddRefsDialog newDialog = new View.AddRefsDialog(this.Post, startIndex);

            // If this dialog is for a photo, we have to set the SwitchToBatchModeCommand 
            // from here so this.WindowManager can be used, and the PostView is the parent
            // of all the correct windows.
            if(newDialog.DataContext is ViewModels.CreateRefViewModel) // I'm sorry.
            {
                if((newDialog.DataContext as ViewModels.CreateRefViewModel).CurrentView is ViewModels.CreateRefViewModels.CreatePhotoViewModel) // ... really, I am.
                {
                    CreateRefViewModels.CreatePhotoViewModel vm = (CreateRefViewModels.CreatePhotoViewModel)(newDialog.DataContext as CreateRefViewModel).CurrentView; // Please don't hate me.
                    vm.SwitchToBatchModeCommand = new RelayCommand(() => this.OpenBatchPhotoAdd(newDialog));
                }
            }

            this.WindowManager.OpenWindow(newDialog);
            
            return true;
        }

        protected void OpenBatchPhotoAdd(View.AddRefsDialog addRefsDialog)
        {
            WindowManager.CloseChild(addRefsDialog);
            WindowManager.OpenWindow(new View.BatchPhotoAdder(this.Post));
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

                this.Post.ResetToTemplate(); 
            }
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
                Extender.WPF.CompletedMessagebox.Show(Export(stream, true));
        }

        public bool Export(StreamWriter stream, bool parse)
        {
            bool success;
            try
            {

                this.Post.Serialize(stream, parse);
                success = true;
            }
            catch (Exception e)
            {
                success = false;
                ExceptionTools.WriteExceptionText(e, true);
            }

            return success;
        }

        private void AutosaveTimer_Tick(object sender, EventArgs e)
        {
            this.Autosave();
            this.AutosaveTimer.IsEnabled = false;
        }

        private void Post_PropertyChanged(object sender,
                                          System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MainText")
            {
                this.AutosaveTimer.IsEnabled = true;
                this.AutosaveTimer.Stop();
                this.AutosaveTimer.Start();
            }
            else if (e.PropertyName == "Category")
            {
                OnPropertyChanged("AddPhotoButtonVisible");
            }
        }

        protected void Autosave()
        {            
            using(StreamWriter stream = File.CreateText(GenerateAutosaveName()))
            {
                Export(stream, false);
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


        #region Settings.settings aliases

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

        #endregion
    }
}
