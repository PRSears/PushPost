using PushPost.ViewModels;
using System.Windows;

namespace PushPost
{
    // TODO Autosave timer

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PushPost_Main : Window
    {
        private Extender.WPF.ViewModel ViewModel
        {
            get
            {
                return (Extender.WPF.ViewModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }

        public PushPost_Main()
        {
            InitializeComponent();
            this.ViewModel = new PostViewModel();
            this.ViewModel.RegisterCloseAction(() => this.Close());

            Title = string.Format("PushPost - Post Builder [alpha {0}]", this.GetAssemblyVersion());
        
            //(DataContext as PostViewModel).CloseCommand =
            //    new Extender.WPF.RelayCommand(() => this.Close());
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if ((DataContext as PostViewModel).HasChildrenOpen)
            {
                if (Extender.WPF.ConfirmationDialog.Show(
                    "Confirm",
                    "Exiting now will close all open Views.\n\nAre you sure you want to quit?"))
                    (DataContext as PostViewModel).CloseChildren();
                else
                    e.Cancel = true;
            }

            base.OnClosing(e);
        }

        private string GetAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return vi.FileVersion;
        }
    }
}
