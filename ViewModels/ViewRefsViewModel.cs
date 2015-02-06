using Extender;
using Extender.WPF;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class ViewRefsViewModel : ViewModel
    {
        public Post Post
        {
            get;
            protected set;
        }

        public ObservableCollection<Checkable<IResource>> ResourceCollection
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
        public ICommand CopySelectedCommand     { get; private set; }
        public ICommand RefreshViewCommand      { get; private set; }

        private const int RefreshInterval = 1111;
        private System.Windows.Threading.DispatcherTimer _AutoRefreshTimer;
        private ObservableCollection<Checkable<IResource>> _ResourceCollection;

        public ViewRefsViewModel(Post post)
        {
            this.Post = post;
            this.RefreshCollection();

            this.SelectAllCommand       = new RelayCommand(() => this.SelectAll());
            this.SelectNoneCommand      = new RelayCommand(() => this.SelectNone());
            this.RemoveSelectedCommand  = new RelayCommand(() => this.RemoveSelected());
            this.CopySelectedCommand    = new RelayCommand(() => this.CopySelected());
            this.RefreshViewCommand     = new RelayCommand(() => this.RefreshCollection());

            this._AutoRefreshTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval  = new System.TimeSpan(0, 0, 0, 0, RefreshInterval),
                IsEnabled = true
            };
            this._AutoRefreshTimer.Tick += (s, e) => this.RefreshCollection();
        }

        public void RefreshCollection()
        {
            if(ResourceCollection == null)
                ResourceCollection = new ObservableCollection<Checkable<IResource>>();

            var resourceCollection = this.ResourceCollection.Select(checkable => checkable.Resource)
                                                            .ToList();

            // Add any new resources in this.Post.Resources that are not in the 
            // ResourceCollection listbox.
            foreach(IResource res in this.Post.Resources)
            {
                if(!resourceCollection.Contains(res))
                {
                    ResourceCollection.Add(new Checkable<IResource>(res));
                }
            }

            // Remove any resources that are in this.ResourceCollection but no longer
            // appear in this.Post.Resources.
            foreach(IResource res in resourceCollection)
            {
                if(!this.Post.Resources.Contains(res))
                {
                    ResourceCollection.RemoveAll(cr => cr.Resource.Equals(res));
                }
            }

            OnPropertyChanged("ResourceCollection");
        }

        public void RemoveSelected() // TODO Add button to the GUI for this command
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
            foreach(Checkable<IResource> res in ResourceCollection)
            {
                res.IsChecked = true;
            }
        }

        public void SelectNone()
        {
            foreach (Checkable<IResource> res in ResourceCollection)
            {
                res.IsChecked = false;
            }
        }

        public void CopySelected()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (Checkable<IResource> res in ResourceCollection)
            {
                if(res.IsChecked)
                {
                    buffer.Append(string.Format(@"+@({0}) ", res.Resource.Name));
                }
            }

            System.Windows.Clipboard.SetText(buffer.ToString());
        }

        public void OnClosing()
        {
            this._AutoRefreshTimer.Stop();
        }
    }
}
