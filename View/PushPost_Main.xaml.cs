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
        private PostViewModel ViewModel
        {
            get
            {
                if (DataContext is PostViewModel)
                    return (PostViewModel)DataContext;
                else
                    return null;
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

            Title = string.Format("PushPost {0} - Post Builder", this.GetShortAssemblyVersion());
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel.HasChildrenOpen)
            {
                e.Cancel = !Extender.WPF.ConfirmationDialog.Show(
                    "Confirm",
                    "Exiting now will close all open Views.\n\nAre you sure you want to quit?");
            }

            if (!e.Cancel) ViewModel.CloseChildren();
            base.OnClosing(e);
            if (!e.Cancel) this.Dispatcher.InvokeShutdown();
        }

        private string GetAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return vi.FileVersion;
        }

        private string GetShortAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return string.Format("{0}.{1}", vi.FileMajorPart, vi.FileMinorPart);

        }
    }
}
