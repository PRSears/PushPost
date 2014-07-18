using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using Extender.WPF;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PushPost.Models.HtmlGeneration;

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
        }

        public void GeneratePages()
        {
            Models.Database.ArchiveQueue.TestHarness();
        }

        public void EditFromDB()
        {
        }

        public void RemoveFromDB()
        {
        }

        public void ImportFromXML()
        {
        }

        public void ExportSelected()
        {
        }

        public void RemoveSelected()
        {

            if (ConfirmBeforeRemoval)
            if (!ConfirmationDialog.Show("Confirm removal", "Remove all selected posts from the queue?"))
                return;

            foreach(CheckablePost entry in CheckablePostCollection.ToArray())
            {
                if (entry.IsChecked)
                    ArchiveQueue.Remove(entry.Post);
            }
        }

        public void SubmitSelected()
        {
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
