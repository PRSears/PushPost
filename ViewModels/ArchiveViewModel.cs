using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.IO;
using System.Text;
using Extender;
using Extender.WPF;
using Extender.Debugging;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.Database;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PushPost.ViewModels
{
    internal class ArchiveViewModel : ViewModel
    {
        protected Models.Database.ArchiveQueue ArchiveQueue;
        protected ObservableCollection<CheckablePost> _CheckablePostCollection;
        public ObservableCollection<CheckablePost> CheckablePostCollection
        {
            get
            {
                return _CheckablePostCollection;
            }
            set
            {
                _CheckablePostCollection = value;
                OnPropertyChanged("QueuedPosts");
            }
        }

        public ICommand SelectAllCommand        { get; private set; }
        public ICommand SelectNoneCommand       { get; private set; }

        public ICommand SubmitSelectedCommand   { get; private set; }
        public ICommand RemoveSelectedCommand   { get; private set; }
        public ICommand ExportSelectedCommand   { get; private set; }
        public ICommand ImportFromXMLCommand    { get; private set; }

        public ICommand RemoveFromDBCommand     { get; private set; }
        public ICommand EditFromDBCommand       { get; private set; }

        public ICommand GeneratePagesCommand    { get; private set; }
        public ICommand PreviewQueueCommand     { get; private set; }
        public ICommand UploadPagesCommand      { get; private set; }

        public bool QueueHasSelected
        {
            get
            {
                foreach(CheckablePost entry in CheckablePostCollection)
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
                return (_CheckablePostCollection.Count > 0) &&
                        QueueHasSelected;
            }
        }
        public bool QueueCanAdd
        {
            get
            {
                return CheckablePostCollection.Count() < Properties.Settings.Default.MaxQueueSize;
            }
        }

        public ArchiveViewModel(Models.Database.ArchiveQueue archiveQueue)
        {

            CheckablePostCollection = new ObservableCollection<CheckablePost>();
            ArchiveQueue            = archiveQueue;

            SelectAllCommand        = new RelayCommand(() => this.SelectAll());
            SelectNoneCommand       = new RelayCommand(() => this.SelectNone());

            SubmitSelectedCommand   = new RelayCommand(
                () => this.SubmitSelected(),
                () => this.QueueCanExecute);
            RemoveSelectedCommand   = new RelayCommand(
                () => this.RemoveSelected(),
                () => this.QueueCanExecute);
            ExportSelectedCommand   = new RelayCommand(
                () => this.ExportSelected(),
                () => this.QueueCanExecute);
            ImportFromXMLCommand    = new RelayCommand(
                () => this.ImportFromXML(),
                () => this.QueueCanAdd);

            RemoveFromDBCommand     = new RelayCommand(() => this.RemoveFromDB());
            EditFromDBCommand       = new RelayCommand(() => this.EditFromDB());

            GeneratePagesCommand    = new RelayCommand(() => this.GeneratePages());
            PreviewQueueCommand     = new RelayCommand(() => this.PreviewQueue());
            UploadPagesCommand      = new RelayCommand(() => this.UploadPages());

            ArchiveQueue.QueueChanged += ArchiveQueue_QueueChanged;
            ArchiveQueue_QueueChanged(this);

            #region Debug: add dummy posts
            //ArchiveQueue.Enqueue(TextPost.Dummy());
            //ArchiveQueue.Enqueue(TextPost.Dummy());
            //ArchiveQueue.Enqueue(TextPost.Dummy());
            #endregion
        }

        protected void ArchiveQueue_QueueChanged(object sender)
        {
            RefreshCollection(ArchiveQueue.GetQueue());
        }

        protected void RefreshCollection(Queue<Post> posts)
        {
            _CheckablePostCollection = new ObservableCollection<CheckablePost>();
            foreach (Post p in posts)
                CheckablePostCollection.Add(new CheckablePost(p));

            OnPropertyChanged("CheckablePostCollection");
        }

        public void SelectNone()
        {
            foreach(CheckablePost entry in CheckablePostCollection)
            {
                entry.IsChecked = false;
            }
        }

        public void SelectAll()
        {
            foreach(CheckablePost entry in CheckablePostCollection)
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

        public void EditFromDB()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented.");
        }

        public void RemoveFromDB()
        {
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
                foreach(CheckablePost entry in CheckablePostCollection) if(entry.IsChecked)
                {
                        using(StreamWriter stream = File.CreateText(
                            GetUniqueFilename(dialog.FileName, entry.Post.Title)))
                        {
                            entry.Post.Serialize(stream);
                        }
                }
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

            foreach (CheckablePost entry in CheckablePostCollection.ToArray())
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
                    foreach (CheckablePost post in CheckablePostCollection)
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
