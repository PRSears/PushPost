using PushPost.ViewModels;
using System;
using System.Windows;

namespace PushPost
{
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
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if ((DataContext as PostViewModel).HasChildrenOpen)
            {
                e.Cancel = !Extender.WPF.ConfirmationDialog.Show(
                    "Confirm",
                    "Exiting now will close all open Views.\n\nAre you sure you want to quit?");
            }

            if (!e.Cancel) (DataContext as PostViewModel).CloseChildren();
            base.OnClosing(e);
            if (!e.Cancel) this.Dispatcher.InvokeShutdown();
        }

        private string GetAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return vi.FileVersion;
        }
    }
}
