using Extender.Debugging;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Commands;
using PushPost.Models.HtmlGeneration;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class PostViewModel : Extender.WPF.ViewModel
    {
        public View.ArchiveManager  ArchiveManager_Window;
        public View.AddRefsDialog   AddRef_Window;

        private Post _Post;
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
        #endregion

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel()
        {
            base.Initialize();

            this._Post          = TextPost.TemplatePost();
            this.ArchiveQueue   = new Models.Database.ArchiveQueue();

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
            this.CreateSiteCommand          = new Extender.WPF.RelayCommand(() => this.CreateSite());
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
                       (AddRef_Window           != null);
            }
        }

        public void CloseChildren()
        {
            if(AddRef_Window != null)
                AddRef_Window.Close();

            if(ArchiveManager_Window != null)
                ArchiveManager_Window.Close();
        }

        public bool CanSubmitPost 
        { 
            get
            {
                // TODO validate post
                return true;
            }
        }

        public bool CanSubmitQueue
        {
            get
            {
                // TODO (re)check all posts in queue
                return true;
            }
        }

        public bool CanRemovePost
        {
            get
            {
                // TODO Check to see if the post has valid Title/ID/etc
                // try Archive.remove
                return true;
            }
        }

        public bool CanAddResource
        {
            get
            {
                // TODO implement CanAddResource logic
                return true;
            }
        }

        public bool HasReferences
        {
            get
            {
                return true; // TODO add HasReferences logic... Check to see if the refs temp file is present
            }
        }

        public bool HasFootnotes
        {
            get
            {
                return true; // TODO add HasFootnotes logic... Check to see if the refs temp file is present
            }
        }

        public bool MenuToolbarCanExecute
        {
            get { return true; } // TODO add MenuToolbarCanExecute logic
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
            PageBuilder previewer = new PageBuilder(new Post[] { this.Post });

            previewer.CreatePages();
            previewer.SavePages(Properties.Settings.Default.PreviewFolderPath);

            string firstFilePath = System.IO.Path.Combine(
                Properties.Settings.Default.PreviewFolderPath,
                previewer.Pages[0].FileName);

            System.Diagnostics.Process browserProc  = new System.Diagnostics.Process();

            browserProc.StartInfo.FileName          = firstFilePath;
            browserProc.StartInfo.UseShellExecute   = true;
            browserProc.Start();
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

            using(System.IO.StreamWriter stream = System.IO.File.CreateText(savePath))
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

                Extender.WPF.CompletedMessagebox.Show(success);
            }

            // TODO Add Extender.WPF.CompletedMessagebox on success
        }

        public void CreateSite()
        {
            // prompt user to pick output folder
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select a folder to save the site in";

            CommonFileDialogResult r = dialog.ShowDialog();
            if (r != CommonFileDialogResult.Ok) return;

            // retrieve all posts from the database
            Post[] allPosts;
            using (PushPost.Models.Database.Archive database = new PushPost.Models.Database.Archive())
            {
                allPosts = database.Dump();
            }

            // generate 
            PageBuilder site = new PageBuilder(allPosts);
            site.CreatePages();
            site.SavePages(dialog.FileName);    
        }

        public bool DEBUG 
        { 
            get
            {
                return Properties.Settings.Default.DEBUG;
            }
        }
    }
}
