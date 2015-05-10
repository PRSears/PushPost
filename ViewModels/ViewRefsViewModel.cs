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

        public ICommand SelectAllCommand            { get; private set; }
        public ICommand SelectNoneCommand           { get; private set; }
        public ICommand RemoveSelectedCommand       { get; private set; }
        public ICommand ExpandSelectedCommand       { get; private set; }
        public ICommand CopyMarkupSelectedCommand   { get; private set; }
        public ICommand CopyValueSelectedCommand    { get; private set; }
        public ICommand CopyHTMLSelectedCommand     { get; private set; }
        public ICommand EditSelectedCommand         { get; private set; }
        public ICommand RefreshViewCommand          { get; private set; }

        private const int RefreshInterval = 1111;
        private System.Windows.Threading.DispatcherTimer _AutoRefreshTimer;
        private ObservableCollection<Checkable<IResource>> _ResourceCollection;

        public ViewRefsViewModel(Post post)
        {
            this.Post = post;
            this.RefreshCollection();

            this.InitializeCommands();

            this._AutoRefreshTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval  = new System.TimeSpan(0, 0, 0, 0, RefreshInterval),
                IsEnabled = true
            };
            this._AutoRefreshTimer.Tick += (s, e) => this.RefreshCollection();
        }

        protected void InitializeCommands()
        {
            this.SelectAllCommand = new RelayCommand
            (
                () =>
                {
                    foreach (Checkable<IResource> res in ResourceCollection)
                        res.IsChecked = true;
                },
                () => ResourceCollection.Count > 0
            );

            this.SelectNoneCommand = new RelayCommand
            (
                () =>
                {
                    foreach (Checkable<IResource> res in ResourceCollection)
                        res.IsChecked = false;
                },
                () => HasSelected
            );

            this.RemoveSelectedCommand = new RelayCommand
            (
                () =>
                {
                    var selected = ResourceCollection.Where(r => r.IsChecked)
                                                     .ToArray();

                    foreach (var resource in selected)
                        this.Post.Resources.Remove(resource.Resource);

                    ResourceCollection.RemoveAll(cr => cr.IsChecked);
                },
                () => HasSelected
            );

            this.ExpandSelectedCommand = new RelayCommand
            (
                () =>
                {
                    Post.MainText = ResourceManager.ExpandReferences
                    (
                        Post.MainText,
                        ResourceCollection.Where(cr => cr.IsChecked)
                                          .Select(cr => cr.Resource)
                                          .ToList()
                    );
                },
                () => HasSelected
            );

            this.CopyMarkupSelectedCommand = new RelayCommand
            (
                () =>
                {
                    StringBuilder buffer = new StringBuilder();

                    foreach (var checkable in ResourceCollection.Where(cr => cr.IsChecked))
                        buffer.Append(string.Format(@"+@({0}) ", checkable.Resource.Name));

                    if (buffer.Length > 0)
                        System.Windows.Clipboard.SetText(buffer.ToString());
                },
                () => HasSelected
            );

            this.CopyValueSelectedCommand = new RelayCommand
            (
                () =>
                {
                    StringBuilder buffer = new StringBuilder();

                    foreach (var checkable in ResourceCollection.Where(cr => cr.IsChecked))
                        buffer.AppendLine(checkable.Resource.Value);

                    if (buffer.Length > 0)
                        System.Windows.Clipboard.SetText(buffer.ToString());
                },
                () => HasSelected
            );

            this.CopyHTMLSelectedCommand = new RelayCommand
            (
                () =>
                {
                    StringBuilder buffer = new StringBuilder();

                    foreach (var checkable in ResourceCollection.Where(cr => cr.IsChecked))
                        buffer.AppendLine(checkable.Resource.CreateHTML());

                    if (buffer.Length > 0)
                        System.Windows.Clipboard.SetText(buffer.ToString());
                },
                () => HasSelected
            );

            this.EditSelectedCommand = new RelayCommand
            (
                () =>
                {
                    throw new System.NotImplementedException();
                },
                () => HasSelected
            );

            this.RefreshViewCommand = new RelayCommand
            (
                () => RefreshCollection()
            );
        }

        public bool HasSelected
        {
            get
            {
                foreach(Checkable<IResource> res in ResourceCollection) 
                {
                    if (res.IsChecked) return true;
                }

                return false;
            }
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

        public void OnClosing()
        {
            this._AutoRefreshTimer.Stop();
        }
    }
}
