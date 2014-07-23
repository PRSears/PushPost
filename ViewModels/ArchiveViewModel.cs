using Extender.Debugging;
using Extender.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Models.Database;
using PushPost.Models.HtmlGeneration;
using PushPost.ViewModels.ArchivesViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class ArchiveViewModel : ViewModel
    {
        public IArchiveViewModel[] Tabs 
        {
            get
            {
                return _Tabs;
            }
            set
            {
                _Tabs = value;
            }
        }
        public IArchiveViewModel Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
                OnPropertyChanged("Current");
            }
        }
        public int SelectedTabIndex
        {
            get
            {
                return Array.IndexOf(Tabs, Current);
            }
            set
            {
                Current = Tabs[value];
                OnPropertyChanged("SelectedTabIndex");
            }
        }
        //public ObservableCollection<CheckablePost> CheckablePostCollection
        //{
        //    get
        //    {
        //        return _CheckablePostCollection;
        //    }
        //    set
        //    {
        //        _CheckablePostCollection = value;
        //        OnPropertyChanged("QueuedPosts");
        //    }
        //}

        // Selection
        public ICommand SelectAllCommand        { get; private set; }
        public ICommand SelectNoneCommand       { get; private set; }
        // Queue
        public ICommand SubmitSelectedCommand   { get; private set; }
        public ICommand RemoveSelectedCommand   { get; private set; }
        public ICommand ExportSelectedCommand   { get; private set; }
        public ICommand ImportFromXMLCommand    { get; private set; }
        // Database
        public ICommand RemoveFromDBCommand     { get; private set; }
        public ICommand ExportFromDBCommand     { get; private set; }
        public ICommand SearchDBCommand         { get; private set; }
        public ICommand ReSearchDBCommand       { get; private set; }
        // Generation
        public ICommand GeneratePagesCommand    { get; private set; }
        public ICommand PreviewQueueCommand     { get; private set; }
        public ICommand UploadPagesCommand      { get; private set; }

        //protected ObservableCollection<CheckablePost>   _CheckablePostCollection;
        protected Models.Database.ArchiveQueue          ArchiveQueue;
        protected IArchiveViewModel[]                   _Tabs;
        protected IArchiveViewModel                     _Current;

        public bool QueueHasSelected
        {
            get
            {
                foreach(CheckablePost entry in Current.DisplayedPosts)
                {
                    if (entry.IsChecked)
                        return true;
                }

                return false;
            }
        }
        public bool QueueCanExecute
        {
            get
            {
                return (Current.DisplayedPosts.Count > 0) &&
                        QueueHasSelected &&
                       (Current is QueueViewModel);
            }
        }
        public bool QueueCanAdd
        {
            get
            {
                return Current.DisplayedPosts.Count() < Properties.Settings.Default.MaxQueueSize &&
                       (Current is QueueViewModel);
            }
        }

        public ArchiveViewModel(ArchiveQueue archiveQueue) : this(archiveQueue, false) { }

        public ArchiveViewModel(ArchiveQueue archiveQueue, bool startInDBView)
        {
            //if (startInDBView)  Current = new DatabaseViewModel(this);
            //else                Current = new QueueViewModel(this);
            Tabs = new IArchiveViewModel[]
            {
                new QueueViewModel(this),
                new DatabaseViewModel(this)
            };
            Current                 = Tabs[0];
            ArchiveQueue            = archiveQueue;

            // Selection
            SelectAllCommand        = new RelayCommand(() => this.SelectAll());
            SelectNoneCommand       = new RelayCommand(() => this.SelectNone());
            // Queue
            SubmitSelectedCommand   = new RelayCommand(() => this.SubmitSelected(),
                                                       () => this.QueueCanExecute);
            RemoveSelectedCommand   = new RelayCommand(() => this.RemoveSelected(),
                                                       () => this.QueueCanExecute);
            ExportSelectedCommand   = new RelayCommand(() => this.ExportSelected(),
                                                       () => this.QueueCanExecute);
            ImportFromXMLCommand    = new RelayCommand(() => this.ImportFromXML(),
                                                       () => this.QueueCanAdd);
            // Database
            RemoveFromDBCommand     = new RelayCommand(() => this.RemoveFromDB(),
                                                       () => this.Current is DatabaseViewModel 
                                                          || this.QueueHasSelected);
            ExportFromDBCommand     = new RelayCommand(() => this.ExportFromDB(),
                                                       () => this.Current is DatabaseViewModel 
                                                          && this.QueueHasSelected);
            SearchDBCommand         = new RelayCommand(() => this.SearchDB(),
                                                       () => this.Current is DatabaseViewModel);
            ReSearchDBCommand       = new RelayCommand(() => this.NextSearch(),
                                                       () => this.Current is DatabaseViewModel);
            // Generation
            GeneratePagesCommand    = new RelayCommand(() => this.GeneratePages());
            PreviewQueueCommand     = new RelayCommand(() => this.PreviewQueue(),
                                                       () => this.QueueCanExecute);
            UploadPagesCommand      = new RelayCommand(() => this.UploadPages());

            ArchiveQueue.QueueChanged += ArchiveQueue_QueueChanged;
            ArchiveQueue_QueueChanged(this);
        }

        protected void ArchiveQueue_QueueChanged(object sender)
        {
            var queueVM = Tabs.First(viewModel => 
                               viewModel.GetType().Equals(typeof(QueueViewModel)));

            if (queueVM == null) return;

            queueVM.RefreshCollection(ArchiveQueue.GetQueue());
        }

        public void RefreshCollection(Queue<Post> posts)
        {
            this.Current.RefreshCollection(posts);
        }

        public void SelectNone()
        {
            foreach(CheckablePost entry in Current.DisplayedPosts)
            {
                entry.IsChecked = false;
            }
        }

        public void SelectAll()
        {
            foreach (CheckablePost entry in Current.DisplayedPosts)
            {
                entry.IsChecked = true;
            }
        }

        public void UploadPages()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented.");
        }

        public void GeneratePages()
        {
            // prompt user to pick output folder
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select a folder to save the site in";

            CommonFileDialogResult r = dialog.ShowDialog();
            if (r != CommonFileDialogResult.Ok) return;

            // retrieve all posts from the database
            Post[] allPosts;
            using(Archive database = new Archive())
            {                
                allPosts = database.Dump();
            }            
                        
            // generate 
            PageBuilder site = new PageBuilder(allPosts);
            site.CreatePages();
            site.SavePages(dialog.FileName);            
        }

        public void PreviewQueue()
        {
            PageBuilder previewer = new PageBuilder(ArchiveQueue.GetQueue().ToArray());

            previewer.CreatePages();
            previewer.SavePages(Properties.Settings.Default.PreviewFolderPath);

            string firstFilePath = System.IO.Path.Combine(
                Properties.Settings.Default.PreviewFolderPath,
                previewer.Pages[0].FileName);

            System.Diagnostics.Process browserProc = new System.Diagnostics.Process();

            browserProc.StartInfo.FileName = firstFilePath;
            browserProc.StartInfo.UseShellExecute = true;
            browserProc.Start();
        }

        public void ExportFromDB()
        {
            this.ExportSelected();
        }

        public void RemoveFromDB()
        {
            if (!ConfirmationDialog.Show("Confirm post deletion",
                @"Are you sure you want to remove all selected posts from the DATABASE?
It will be a pain in the ass to get them back afterward."))
                return;

            using(Archive db = new Archive())
            {
                db.DeletePosts(
                    Current.DisplayedPosts
                    .Where(cp => cp.IsChecked)
                    .Select(cp => cp.Post)
                    .ToArray());

                db.SubmitChanges();
            }

            #region one at a time...
            //using(Archive db = new Archive())
            //{
            //    var selected = Current.DisplayedPosts.Where(p => p.IsChecked).ToArray();
            //    foreach (var post in selected)
            //    {
            //        db.DeletePost(post.Post);
            //        Current.DisplayedPosts.Remove(post);
            //    }

            //    db.SubmitChanges();
            //}
            #endregion
        }

        public void SearchDB()
        {
            if (!(Current is DatabaseViewModel)) return;

            (Current as DatabaseViewModel).ExecuteSearch();
        }

        public void NextSearch()
        {
            if (!(Current is DatabaseViewModel)) return;

            (Current as DatabaseViewModel).ExecuteNextSearch();
        }

        public void ImportFromXML()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt = ".xml";
            dialog.Filter = @"XML documents (*.txt, *.xml)
                |*.txt;*.xml|All files (*.*)|*.*";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                Post imported  = Post.Deserialize(dialog.FileName);
                ArchiveQueue.Enqueue(imported);
                
                if (DEBUG) Debug.WriteMessage("Imported: " + imported.ToString(), "info");
            }
            else return;
        }

        public void ExportSelected()
        {
            if (!QueueHasSelected)
                return;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select export folder";

            CommonFileDialogResult r = dialog.ShowDialog();

            if(r == CommonFileDialogResult.Ok)
            {
                bool success;
                try
                {
                    foreach (CheckablePost entry in Current.DisplayedPosts) if (entry.IsChecked)
                    {
                        using (StreamWriter stream = File.CreateText(
                            GetUniqueFilename(dialog.FileName, entry.Post.Title)))
                        {
                            entry.Post.Serialize(stream);
                        }
                    }
                    success = true;
                }
                catch (Exception e)
                {
                    ExceptionTools.WriteExceptionText(e, false);
                    success = false;
                }

                CompletedMessagebox.Show(success);
            }
        }

        private string GetUniqueFilename(string dir, string name)
        {
            int n = 0;

            string fullname;

            do
            {
                fullname = System.IO.Path.Combine(dir, string.Format(
                    @"{0}_{1}.xml", 
                    name, 
                    (n++).ToString("D3")));
            } while (System.IO.File.Exists(fullname));

            return fullname;
        }

        public void RemoveSelected()
        {
            RemoveSelected(Properties.Settings.Default.ConfirmBeforeRemove);
        }

        protected void RemoveSelected(bool confirm)
        {
            if (confirm)
            if (!ConfirmationDialog.Show("Confirm removal", "Remove all selected posts from the queue?"))
                    return;

            foreach (CheckablePost entry in Current.DisplayedPosts.ToArray())
            {
                if (entry.IsChecked)
                    ArchiveQueue.Remove(entry.Post);
            }
        }

        public void SubmitSelected()
        {
            try
            {
                using (Archive database = new Archive())
                {
                    foreach (CheckablePost post in Current.DisplayedPosts)
                    {
                        if (post.IsChecked)
                            database.CommitPost(post.Post);
                    }

                    database.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Extender.Debugging.ExceptionTools.WriteExceptionText(e, true,
                    "Failed to submit posts to the databse.");
                return;
            }

            //successful
            this.RemoveSelected(false);
        }

        private bool DEBUG { get { return Properties.Settings.Default.DEBUG; } }
        private bool ConfirmBeforeRemoval { get { return Properties.Settings.Default.ConfirmBeforeRemove; } }
    }

    internal class CheckablePost : INotifyPropertyChanged
    {
        protected Post _Post;
        protected bool _IsChecked;

        public Post Post
        {
            get
            {
                return _Post;
            }
            set
            {
                _Post = value;
                OnPropertyChanged("Post");
            }
        }
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public CheckablePost(Post post)
        {
            this.Post       = post;
            this.IsChecked  = false;
        }

        public CheckablePost(Post post, bool isChecked)
        {
            this.Post       = post;
            this.IsChecked  = isChecked;
        }

        public CheckablePost(PostTableLayer post)
        {
            this.Post = post.TryCreatePost();
        }

        public CheckablePost(PostTableLayer post, bool isChecked)
        {

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion
    }
}
