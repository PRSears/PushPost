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

namespace PushPost.ViewModels.ArchivesViewModels
{
    internal class QueueViewModel : ViewModel, IArchiveViewModel
    {
        protected ArchiveViewModel _Parent;
        protected ObservableCollection<CheckablePost> _DisplayedPosts;

        public QueueViewModel(ArchiveViewModel parent)
        {
            this._Parent            = parent;
            this._DisplayedPosts    = new ObservableCollection<CheckablePost>();
        }

        public ObservableCollection<CheckablePost> DisplayedPosts
        {
            get
            {
                return _DisplayedPosts;
            }
            set
            {
                _DisplayedPosts = value;
                OnPropertyChanged("DisplayedPosts");
            }
        }

        public void RefreshCollection(Queue<Post> posts)
        {
            _DisplayedPosts = new ObservableCollection<CheckablePost>();

            foreach(Post post in posts)
            {
                DisplayedPosts.Add(new CheckablePost(post));
            }

            OnPropertyChanged("DisplayedPosts");
        }
    }
}
