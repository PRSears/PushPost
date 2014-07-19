using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using PushPost.ViewModels;
using System.Windows;
using System;

namespace PushPost
{
    // TODO Autosave timer

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PushPost_Main : Window
    {
        public PushPost_Main()
        {
            InitializeComponent();
            DataContext = new PostViewModel();
            RegisterCloseAction();

            Title = string.Format("PushPost - Post Builder [alpha {0}]", this.GetAssemblyVersion());
        }

        protected void RegisterCloseAction()
        {
            PostViewModel vm = (DataContext as PostViewModel);

            if (vm.CloseAction == null)
                vm.CloseAction = new Action(() => this.Close());
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
