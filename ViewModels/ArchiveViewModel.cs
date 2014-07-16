using System;
using System.Collections.Generic;
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
        protected ObservableCollection<CheckablePost> _QueuedPosts;
        
        public ObservableCollection<CheckablePost> QueuedPosts
        {
            get
            {
                return _QueuedPosts;
            }
            set
            {
                _QueuedPosts = value;
                OnPropertyChanged("QueuedPosts");
            }
        }

        public ArchiveViewModel()
        {
            QueuedPosts = new ObservableCollection<CheckablePost>();
            QueuedPosts.Add(new CheckablePost(TextPost.Dummy()));
            QueuedPosts.Add(new CheckablePost(TextPost.Dummy()));
            QueuedPosts.Add(new CheckablePost(TextPost.Dummy()));
        }
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
