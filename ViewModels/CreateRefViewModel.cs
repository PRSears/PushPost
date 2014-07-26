using Extender.Debugging;
using PushPost.Commands;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.ViewModels.CreateRefViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

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
                NotifyingResource.Types[3].Name
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
                return
                    (!string.IsNullOrEmpty(CurrentView.Resource.Name)) &&
                    (!string.IsNullOrEmpty(CurrentView.Resource.Value));
            }
        }

        public string Save()
        {
            if(CurrentView is CreateImageViewModel)
            {
                if(string.IsNullOrWhiteSpace(Properties.Settings.Default.SiteExportFolder))
                {
                    System.Windows.Forms.MessageBox.Show("You need to select a folder " +
                        "to create the site in before an image can be added.");

                    int count = 0;
                    do
                    {
                        if ((count++) > 2) return string.Empty; // cancel

                        Properties.Settings.Default.SiteExportFolder = SelectSiteExportFolder();
                    } while (string.IsNullOrWhiteSpace(Properties.Settings.Default.SiteExportFolder));
                }

                (CurrentView as CreateImageViewModel).Image.Proccess();
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
                    return string.Empty;
                }
            }
            else
            {
                this.Post.Resources.Add(CurrentView.Resource);
                if (AutoInsertMarkup) this.Post.MainText += CurrentView.Resource.Markup;
            }

            CloseCommand.Execute(null);
            return CurrentView.Resource.Markup;
        }

        private string SelectSiteExportFolder()
        {
            var dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;
            dialog.Title = string.Format("Select a folder to export the site into");

            CommonFileDialogResult r = dialog.ShowDialog();
            if (r != CommonFileDialogResult.Ok)
                return string.Empty;
            else
                return dialog.FileName;
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
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateFootViewModel));

            if (nextVM == null)
                nextVM = new CreateFootViewModel();

            CurrentView = nextVM;
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
