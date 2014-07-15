using PushPost.Models.HtmlGeneration;
using PushPost.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PushPost
{
    /// <summary>
    /// Interaction logic for AddRefsWindow.xaml
    /// </summary>
    public partial class AddRefsDialog : Window
    {
        public AddRefsDialog()
        {
            InitializeComponent();
            DataContext = new CreateRefViewModel();
            RegisterCloseAction();
        }

        public AddRefsDialog(int initialTypeIndex) : this(null, initialTypeIndex) { }

        public AddRefsDialog(Post post, int initialTypeIndex)
        {
            InitializeComponent();
            DataContext = new CreateRefViewModel(
                post,
                Models.HtmlGeneration.Embedded.NotifyingResource.Types[initialTypeIndex]);
            RegisterCloseAction();
        }

        public AddRefsDialog(Post post, Type initialType)
        {
            InitializeComponent();
            DataContext = new CreateRefViewModel(post, initialType);
            RegisterCloseAction();
        }

        protected void RegisterCloseAction()
        {
            CreateRefViewModel vm = (DataContext as CreateRefViewModel);

            if (vm.CloseAction == null)
                vm.CloseAction = new Action(() => this.Close());
        }

        private void MarkupPreviewText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(MarkupPreviewText.Text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if((DataContext as CreateRefViewModel).ConfirmClose)
            {
                System.Windows.Forms.DialogResult r = System.Windows.Forms.MessageBox.Show(
                    "Are you sure?\nAny unsaved changes will be lost.",
                    "Confirm close",
                    System.Windows.Forms.MessageBoxButtons.YesNo
                    );

                if (r != System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = true;
            }

            base.OnClosing(e);
        }
    }
}
