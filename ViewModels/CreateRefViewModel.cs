using Extender.Debugging;
using PushPost.Commands;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.ViewModels.CreateRefViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    internal class CreateRefViewModel : Extender.WPF.ViewModel
    {
        private bool DEBUG
        {
            get
            {
                return Properties.Settings.Default.DEBUG;
            }
        }

        private Post _Post;
        public Post Post
        {
            get
            {
                return _Post;
            }
        }

        public IRefViewModel _CurrentView;
        public IRefViewModel CurrentView 
        {
            get
            {
                return _CurrentView;
            }
            set
            {
                _CurrentView = value;
                OnPropertyChanged("CurrentView");
            }
        }
        protected List<IRefViewModel> ViewHistory { get; set; }

        public string[] ResourceTypeList
        {
            get;
            private set;
        }
        private string _SelectedResource;
        public string SelectedResource
        {
            get { return _SelectedResource; }
            set { _SelectedResource = value; OnPropertyChanged("SelectedResource"); }
        }

        public bool AutoInsertMarkup { get; set; }

        public ICommand ViewCreateLinkCommand   { get; private set; }
        public ICommand ViewCreateCodeCommand   { get; private set; }
        public ICommand ViewCreateFootCommand   { get; private set; }
        public ICommand ViewCreateImageCommand  { get; private set; }

        public ICommand SaveRefCommand { get; private set; }
        public ICommand CancelRefCommand { get; private set; }
        
        public CreateRefViewModel() : this(typeof(Link)) { }

        public CreateRefViewModel(Type initialType)
        {
            ResourceTypeList = new string[]
            { 
                NotifyingResource.Types[0].Name, 
                NotifyingResource.Types[1].Name,
                NotifyingResource.Types[2].Name,
            };

            ViewHistory         = new List<IRefViewModel>();
            ConfirmClose        = Properties.Settings.Default.CloseConfirmations;
            AutoInsertMarkup    = Properties.Settings.Default.AutoInsertMarkup;

            SwitchToView(initialType);
            this.PropertyChanged += SelectedResource_PropertyChanged;

            SaveRefCommand      = new SaveRefCommand(this);
            CancelRefCommand    = new CancelRefCommand(this);

            ViewCreateLinkCommand   = new ViewCreateLinkCommand(this);
            ViewCreateCodeCommand   = new ViewCreateCodeCommand(this);
            ViewCreateFootCommand   = new ViewCreateFootCommand(this);
            ViewCreateImageCommand  = new ViewCreateImageCommand(this);
        }

        public CreateRefViewModel(Post post, Type initialType)
            : this(initialType)
        {
            _Post = post;
        }

        public void Initialize(string resourceType)
        {
            if      (resourceType.ToLower().Contains("hypertext"))
                _CurrentView = new CreateLinkViewModel();
            else if (resourceType.ToLower().Contains("code"))
                _CurrentView = new CreateCodeViewModel();
            else if (resourceType.ToLower().Contains("image"))
                _CurrentView = new CreateImageViewModel();
            else
                throw new ArgumentException("Provided resourceType is not supported.");

            CurrentView.Resource.ResourceType = resourceType;
        }

        public void InitializeByType(Type type)
        {
            if      (type == typeof(Link))
                _CurrentView = new CreateLinkViewModel();

            else if (type == typeof(Code))
                _CurrentView = new CreateCodeViewModel();

            else if (type == typeof(InlineImage))
                _CurrentView = new CreateImageViewModel();

            else
                throw new ArgumentException("Provided Type is not supported.");

            _SelectedResource = type.Name;
        }

        [Obsolete]
        protected void Subscribe()
        {
            CurrentView.Resource.PropertyChanged += Resource_PropertyChanged;
        }

        protected void Resource_PropertyChanged(
            object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ResourceType")
            {
                Debug.WriteMessage("Attempting to switch ViewModels.", DEBUG, "info");

                string newType = CurrentView.Resource.ResourceType;

                if (newType.ToLower().Contains("hypertext"))
                    SwitchToLinkView();
                else if (newType.ToLower().Contains("code"))
                    SwitchToCodeView();
                else if (newType.ToLower().Contains("image"))
                    SwitchToImageView();
                else
                    System.Windows.Forms.MessageBox.Show("Unkown ResourceType. Cannot switch views.");

                CurrentView.Resource.QuietSetResourceType(newType);
            }
        }

        protected void SelectedResource_PropertyChanged(
            object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Console.WriteLine(SelectedResource);
            if (e.PropertyName == "SelectedResource")
            {
                SwitchToView(NotifyingResource.GetType(SelectedResource));
            }
        }

        public void Discard()
        {
            CurrentView.Initialize();
        }

        public void Cancel()
        {
            CloseAction();
        }

        public bool CanSave
        {
            get
            {
                return
                    (!string.IsNullOrEmpty(CurrentView.Resource.Name)) &&
                    (!string.IsNullOrEmpty(CurrentView.Resource.Value));
            }
        }

        public string Save()
        {
            if (this.Post == null)
            {
                try
                {
                    CurrentView.Save(Properties.Settings.Default.TempReferenceFilename);
                }
                catch (Exception e)
                {
                    ExceptionTools.WriteExceptionText(e, true);
                    return string.Empty;
                }
            }
            else
            {
                this.Post.Resources.Add(CurrentView.Resource);
                if (AutoInsertMarkup) this.Post.MainText += CurrentView.Resource.Markup;
            }

            CloseAction();
            return CurrentView.Resource.Markup;
        }

        public bool CanSwitchViews
        {
            get
            {   
                return string.IsNullOrEmpty(this.SelectedResource);
            }
        }

        public void SwitchToView(Type typeOfView)
        {
            Debug.WriteMessage("SwitchToView: " + typeOfView.Name, DEBUG);

            if (typeOfView == typeof(Link))
                SwitchToLinkView();

            else if (typeOfView == typeof(Code))
                SwitchToCodeView();

            else if (typeOfView == typeof(InlineImage))
                SwitchToImageView();

            else if (typeOfView == typeof(Footer))
                SwitchToFooterView();

            else
                throw new ArgumentException("Provided Type is not supported.");

            _SelectedResource = typeOfView.Name;
        }

        public void SwitchToLinkView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateLinkViewModel));

            if (nextVM == null)
                nextVM = new CreateLinkViewModel();

            CurrentView = nextVM;
            //Subscribe();
        }

        public void SwitchToCodeView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateCodeViewModel));

            if (nextVM == null)
                nextVM = new CreateCodeViewModel();

            CurrentView = nextVM;
            //Subscribe();
        }

        public void SwitchToImageView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateImageViewModel));

            if (nextVM == null)
                nextVM = new CreateImageViewModel();

            CurrentView = nextVM;
            //Subscribe();
        }

        public void SwitchToFooterView()
        {
            throw new NotImplementedException();
        }

        protected IRefViewModel RetrieveFromHistory(Type type)
        {
            foreach(IRefViewModel vm in ViewHistory)
            {
                if (vm.GetType() == type)
                    return vm;
            }

            return null;
        }

        protected void AddToHistory(IRefViewModel current)
        {
            if (current == null) return;

            foreach(IRefViewModel vm in ViewHistory)
            {
                if(vm.GetType() == current.GetType())
                {
                    ViewHistory.Remove(vm);
                    ViewHistory.Add(current);
                    return;
                }
            }

            ViewHistory.Add(current);
        }
    }
}
