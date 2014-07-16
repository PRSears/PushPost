using Extender.Debugging;
using PushPost.Commands;
using PushPost.Models.HtmlGeneration;
using System;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class PostViewModel : Extender.WPF.ViewModel
    {
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

        #region ICommands
        public ICommand SubmitPostCommand           { get; private set; }
        public ICommand SubmitQueueCommand          { get; private set; }
        public ICommand QueuePostCommand            { get; private set; }
        public ICommand RemovePostCommand           { get; private set; }
        public ICommand DiscardCommand              { get; private set; }
        public ICommand AddIResourceCommand         { get; private set; }
        public ICommand AddFootnoteCommand          { get; private set; }

        public ICommand ImportFromFileCommand { get; private set; }
        public ICommand ExportToFileCommand { get; private set; }
        public ICommand PreviewInBrowserCommand     { get; private set; }
        public ICommand OpenArchiveManagerCommand   { get; private set; }
        public ICommand OpenPageGeneratorCommand    { get; private set; }
        public ICommand ViewReferencesCommand       { get; private set; }
        public ICommand ViewFootnotesCommand        { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel()
        {
            base.Initialize();

            _Post = TextPost.TemplatePost();

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
            System.Windows.Forms.MessageBox.Show("Post queued.");
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
            AddRefsDialog dialog = new AddRefsDialog(this.Post, startIndex);
            dialog.Show();
        }

        public void OpenAddFootnoteDialog()
        {
            System.Windows.Forms.MessageBox.Show("Footnotes not implemented.");
        }

        public void SubmitNow()
        {
            // QueuePostForSubmit()
            // Queue.SubmitQueue()...

            System.Text.StringBuilder post = new System.Text.StringBuilder();
            post.AppendLine(this.Post.ToString());
            post.AppendLine("");
            
            foreach(PushPost.Models.HtmlGeneration.Embedded.IResource r in Post.Resources)
            {
                post.AppendLine(r.ToString());
            }

            System.Windows.Forms.MessageBox.Show(post.ToString());

            System.Windows.Forms.MessageBox.Show(Post.ParsedMainText);
        }

        public void Discard()
        {
            InitializeByType(_Post.GetType());
            System.Windows.Forms.MessageBox.Show("Discarded post.");
        }

        public void PreviewInBrowser()
        {
            throw new NotImplementedException();
        }

        public void OpenArchiveManager()
        {
            PushPost.View.ArchiveManager archive = new View.ArchiveManager();
            archive.Show();
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
                this.Post = Post.Deserialize(dialog.FileName);

                if (DEBUG) Console.WriteLine("Imported: " + this._Post.ToString());
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
                this.Post.Serialize(stream);
            }
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
