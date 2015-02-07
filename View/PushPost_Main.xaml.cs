using PushPost.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            // If the post object changes we have to resubscribe
            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("Post"))
                    ViewModel.Post.PropertyChanged += ViewModel_Post_PropertyChanged;
            };

            ViewModel.Post.PropertyChanged += ViewModel_Post_PropertyChanged;

            // Fix binding not updating 
            CategoryDropdown.SelectionChanged +=
            (
                (sender, e) =>
                {
                    try
                    {
                        ComboBox cb = sender as ComboBox;
                        BindingOperations.GetBindingExpression(cb, ComboBox.SelectedValueProperty).UpdateTarget();
                    }
                    catch { }
                }
            );
        }

        protected void ViewModel_Post_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Category"))
            {
                if (ViewModel.Post.Category.PostType != ViewModel.Post.GetType())
                {
                    if (ViewModel.PostIsDefault || Extender.WPF.ConfirmationDialog.Show
                        (
                            "Change post type",
                            "Changing the type of post will erase the current post.\n" +
                            "Are you sure you want to continue?"
                        ))
                    {
                        ViewModel.Post = (PushPost.Models.HtmlGeneration.Post)
                            Activator.CreateInstance(ViewModel.Post.Category.PostType);
                        ViewModel.Post.ResetToTemplate();

                        ViewModel.Post.PropertyChanged += ViewModel_Post_PropertyChanged; // resubscribe
                    }
                    else
                    {
                        //ViewModel.Post.QuietSetCategoryString((string)CategoryDropdown.SelectedValue);
                        ViewModel.Post.CategoryString = (string)CategoryDropdown.SelectedValue;
                    }
                }
            }
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
