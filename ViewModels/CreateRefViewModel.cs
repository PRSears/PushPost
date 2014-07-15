using PushPost.Commands;
using PushPost.ViewModels.CreateRefViewModels;
using Extender.Debugging;
using Extender.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using PushPost.Models.HtmlGeneration.Embedded;

//
// TODO Overhaul reference adding
//
//      Stop being a fucking idiot, pass the Post object to the ViewModel
//      and call Post.Resources.Add() on the ViewModel's Resource object @
//      window close event

namespace PushPost.ViewModels
{
    internal class CreateRefViewModel : Extender.WPF.ViewModel
    {
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
            };

            // TODO Use actual Type objects instead
            //      Use ResourceTypes[i].ToString() in the combobox...

            Type[] ResourceTypes = new Type[] 
            {
                typeof(Link),
                typeof(Code),
                typeof(InlineImage)
            };

            // Then use Type.ToString() to decide how to init _CurrentView
            // Then 

            ViewHistory     = new List<IRefViewModel>();
            ConfirmClose    = Properties.Settings.Default.CloseConfirmations;
            
            Initialize(ResourceTypeList[resourceType_selectedIndex]);
            Subscribe();

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
                _CurrentView = new CreateLinkViewModel();
            else if (resourceType.ToLower().Contains("code"))
                _CurrentView = new CreateCodeViewModel();
            else if (resourceType.ToLower().Contains("image"))
                _CurrentView = new CreateImageViewModel();
            else
                throw new ArgumentException("Provided resourceType is not supported.");

            CurrentView.Resource.ResourceType = resourceType;
        }

        protected void Subscribe()
        {
            CurrentView.Resource.PropertyChanged += Resource_PropertyChanged;
        }

        void Resource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ResourceType")
            {
                Console.WriteLine("Attempting to switch ViewModels.");

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

        public void Save()
        {
            try
            {
                CurrentView.Save(Properties.Settings.Default.TempReferenceFilename);
            }
            catch (Exception e)
            {
                Console.WriteLine(ExceptionTools.CreateExceptionText(e, true));
                return;
            }

            CloseAction();
        }

        public bool CanSwitchViews
        {
            get
            {   // TODO add logic to enable/disable view switching
                return string.IsNullOrEmpty(CurrentView.Resource.ResourceType); 
            }
        }

        public void SwitchToLinkView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateLinkViewModel));

            if (nextVM == null)
                nextVM = new CreateLinkViewModel();

            CurrentView = nextVM;
            Subscribe();
        }

        public void SwitchToCodeView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateCodeViewModel));

            if (nextVM == null)
                nextVM = new CreateCodeViewModel();

            CurrentView = nextVM;
            Subscribe();
        }

        public void SwitchToImageView()
        {
            AddToHistory(CurrentView);
            IRefViewModel nextVM = RetrieveFromHistory(typeof(CreateImageViewModel));

            if (nextVM == null)
                nextVM = new CreateImageViewModel();

            CurrentView = nextVM;
            Subscribe();
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
