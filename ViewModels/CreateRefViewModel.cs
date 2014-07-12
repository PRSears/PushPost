using System;
using PushPost.Commands;
using Extender.Debugging;
using System.Windows.Input;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.Models.HtmlGeneration.PostTypes;
using PushPost.ViewModels.CreateRefViewModels;

namespace PushPost.ViewModels
{
    internal class CreateRefViewModel
    {
        public IRefViewModel CurrentView { get; set; }
        protected List<IRefViewModel> ViewHistory { get; set; }

        public string[] ResourceTypeList
        {
            get;
            private set;
        }

        public ICommand ViewCreateLinkCommand   { get; private set; }
        public ICommand ViewCreateCodeCommand   { get; private set; }
        public ICommand ViewCreateFootCommand   { get; private set; }
        public ICommand ViewCreateImageCommand  { get; private set; }

        public ICommand SaveRefCommand { get; private set; }
        public ICommand CancelRefCommand { get; private set; }

        public CreateRefViewModel() : this(0) { }

        public CreateRefViewModel(int resourceType_selectedIndex)
        {
            ResourceTypeList = new string[]
            { 
                "Hypertext Reference", 
                "Code Snippet",
                "Image",
                "Footnote"
            };
            
            Initialize(ResourceTypeList[resourceType_selectedIndex]);
            CurrentView.Resource.PropertyChanged += Resource_PropertyChanged;

            SaveRefCommand      = new SaveRefCommand(this);
            CancelRefCommand    = new CancelRefCommand(this);

            ViewCreateLinkCommand   = new ViewCreateLinkCommand(this);
            ViewCreateCodeCommand   = new ViewCreateCodeCommand(this);
            ViewCreateFootCommand   = new ViewCreateFootCommand(this);
            ViewCreateImageCommand  = new ViewCreateImageCommand(this);
        }

        public void Initialize(string resourceType)
        {
            if      (resourceType.ToLower().Contains("hypertext"))
                CurrentView = new CreateLinkViewModel();
            else if (resourceType.ToLower().Contains("code"))
                CurrentView = new CreateCodeViewModel();
            else
                throw new ArgumentException("Provided resourceType is not supported.");

            CurrentView.Resource.ResourceType = resourceType;
        }

        void Resource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ResourceType")
            {
                // TODO handle resource type changing
            }
        }

        public void Discard()
        {
            CurrentView.Initialize();
        }

        public void Cancel()
        {
        }

        public bool CanSave
        {
            get
            { 
                return true; // TODO Add CanSave logic
            }
        }

        public void Save()
        {
            CurrentView.Save(Properties.Settings.Default.TempReferenceFilename);
        }

        public bool CanSwitchViews
        {
            get
            {
                return true; // TODO add logic to enable/disable view switching
            }
        }

        public void SwitchToLinkView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateLinkViewModel));

            if (nextVM == null)
                nextVM = new CreateLinkViewModel();

            CurrentView = nextVM;
        }

        public void SwitchToCodeView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateCodeViewModel));

            if (nextVM == null)
                nextVM = new CreateCodeViewModel();

            CurrentView = nextVM;
        }

        public void SwitchToFooterView()
        {
            System.Windows.Forms.MessageBox.Show("Switching to footer vm not implemented.");
        }

        public void SwitchToImageView()
        {
            System.Windows.Forms.MessageBox.Show("Switching to image vm not implemented.");
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
