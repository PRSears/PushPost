using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Extender;
using Extender.WPF;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels
{
    internal class ViewRefsViewModel : ViewModel
    {
        public Post Post
        {
            get;
            protected set;
        }

        private ObservableCollection<CheckableResource> _ResourceCollection;
        public ObservableCollection<CheckableResource> ResourceCollection
        {
            get
            {
                return _ResourceCollection;
            }
            set
            {
                _ResourceCollection = value;
                OnPropertyChanged("ResourceCollection");
            }
        }

        public ICommand SelectAllCommand        { get; private set; }
        public ICommand SelectNoneCommand       { get; private set; }
        public ICommand RemoveSelectedCommand   { get; private set; }
        public ICommand RefreshViewCommand      { get; private set; }

        public ViewRefsViewModel(Post post)
        {
            this.Post = post;
            this.RefreshCollection();

            this.SelectAllCommand       = new RelayCommand(() => this.SelectAll());
            this.SelectNoneCommand      = new RelayCommand(() => this.SelectNone());
            this.RemoveSelectedCommand  = new RelayCommand(() => this.RemoveSelected());
            this.RefreshViewCommand     = new RelayCommand(() => this.RefreshCollection());
        }

        public void RefreshCollection()
        {
            _ResourceCollection = new ObservableCollection<CheckableResource>();

            foreach(IResource res in this.Post.Resources)
            {
                ResourceCollection.Add(new CheckableResource(res));
            }

            OnPropertyChanged("ResourceCollection");
        }

        public void RemoveSelected()
        {
            var selected = ResourceCollection.Where(r => r.IsChecked);
            foreach(var res in selected)
            {
                this.Post.Resources.Remove(res.Resource);
            }

            ResourceCollection.RemoveAll(r => r.IsChecked);
        }

        public void SelectAll()
        {
            foreach(CheckableResource res in ResourceCollection)
            {
                res.IsChecked = true;
            }
        }

        public void SelectNone()
        {
            foreach(CheckableResource res in ResourceCollection)
            {
                res.IsChecked = false;
            }
        }
    }

    internal class CheckableResource : INotifyPropertyChanged
    {
        protected IResource _Resource;
        protected bool _IsChecked;

        public IResource Resource 
        {
            get
            {
                return _Resource;
            }
            set
            {
                _Resource = value;
                OnPropertyChanged("Resource");
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

        public string Display
        {
            get
            {
                return string.Format(@"+@({0}) => {1}",
                    this.Resource.Name,
                    this.Resource.Value);
            }
        }

        public CheckableResource(IResource resource)
        {
            this.Resource   = resource;
            this.IsChecked  = false;
        }

        public CheckableResource(IResource resource, bool isChecked)
        {
            this.Resource   = resource;
            this.IsChecked  = isChecked;
        }

        public CheckableResource()
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
