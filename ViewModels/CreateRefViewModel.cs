using Extender.Debugging;
using Extender.WPF;
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

        private IRefViewModel _CurrentView;
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
        public bool AllowTypeSelection { get; set; }

        public ICommand ViewCreateLinkCommand   { get; private set; }
        public ICommand ViewCreateCodeCommand   { get; private set; }
        public ICommand ViewCreateFootCommand   { get; private set; }
        public ICommand ViewCreateImageCommand  { get; private set; }
        public ICommand SaveRefCommand          { get; private set; }
        public ICommand CancelRefCommand        { get; private set; }
        
        public CreateRefViewModel() : this(typeof(Link)) { }

        public CreateRefViewModel(Type initialType)
        {
            ResourceTypeList = new string[]
            { 
                NotifyingResource.Types[0].Name, 
                NotifyingResource.Types[1].Name,
                NotifyingResource.Types[2].Name,
                NotifyingResource.Types[3].Name
            };

            ViewHistory         = new List<IRefViewModel>();
            ConfirmClose        = Properties.Settings.Default.CloseConfirmations;
            AutoInsertMarkup    = Properties.Settings.Default.AutoInsertMarkup;
            AllowTypeSelection  = true;

            SwitchToView(initialType);
            this.PropertyChanged += SelectedResource_PropertyChanged;

            SaveRefCommand      = new RelayCommand
            (
                () => this.Save(),
                () => this.CanSave
            );
            CancelRefCommand    = new RelayCommand
            (
                () => this.Cancel()
            );

            ViewCreateLinkCommand   = new RelayCommand
            (
                () => this.SwitchToLinkView(),
                () => this.CanSwitchViews
            );
            ViewCreateCodeCommand   = new RelayCommand
            (
                () => this.SwitchToCodeView(),
                () => this.CanSwitchViews
            );
            ViewCreateImageCommand  = new RelayCommand
            (
                () => this.SwitchToImageView(),
                () => this.CanSwitchViews
            );
            ViewCreateFootCommand   = new RelayCommand
            (
                () => this.SwitchToFooterView(),
                () => this.CanSwitchViews
            );
        }


        public CreateRefViewModel(Post post, Type initialType)
            : this(initialType)
        {
            _Post = post;
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
            CloseCommand.Execute(null);
        }

        public bool CanSave
        {
            get
            {
                return  (!string.IsNullOrEmpty(CurrentView.Resource.Name)) &&
                        (!string.IsNullOrEmpty(CurrentView.Resource.Value));
            }
        }

        public string Save() // 'ADD' button
        {
            if(CurrentView is CreateImageViewModel)
            {
                if (!Site.CheckSiteExportFolder())
                    return string.Empty;

                ImgProcessor.OrganizeImage
                (
                    (CurrentView as CreateImageViewModel).Image,
                    Properties.Settings.Default.ImageSizes
                );
            }

            if (this.Post == null)
            {
                try
                {
                    CurrentView.Save(Properties.Settings.Default.TempReferenceFilename);
                }
                catch (Exception e)
                {
                    ExceptionTools.WriteExceptionText(e, true);
                    throw e;
                }
            }
            else
            {
                this.Post.Resources.Add(CurrentView.Resource);
                if (AutoInsertMarkup && !(CurrentView.Resource is Photo)) this.Post.MainText += CurrentView.Resource.Markup;
            }

            CloseCommand.Execute(null);
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

            else if (typeOfView == typeof(Photo))
                SwitchToPhotoView();

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

            AllowTypeSelection = true;
        }

        public void SwitchToCodeView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateCodeViewModel));

            if (nextVM == null)
                nextVM = new CreateCodeViewModel();

            CurrentView = nextVM;

            AllowTypeSelection = true;
        }

        public void SwitchToImageView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateImageViewModel));

            if (nextVM == null)
                nextVM = new CreateImageViewModel();

            CurrentView = nextVM;

            AllowTypeSelection = true;
        }

        public void SwitchToFooterView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateFootViewModel));

            if (nextVM == null)
                nextVM = new CreateFootViewModel();

            CurrentView = nextVM;

            AllowTypeSelection = true;
        }

        public void SwitchToPhotoView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreatePhotoViewModel));

            if (nextVM == null)
                nextVM = new CreatePhotoViewModel();

            CurrentView = nextVM;

            AllowTypeSelection = false;
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
