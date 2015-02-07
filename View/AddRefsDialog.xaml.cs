using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for AddRefsWindow.xaml
    /// </summary>
    public partial class AddRefsDialog : Window
    {
        private CreateRefViewModel ViewModel
        {
            get
            {
                if (DataContext is CreateRefViewModel)
                    return (CreateRefViewModel)DataContext;
                else
                    return null;
            }
            set
            {
                DataContext = value;
            }
        }

        public AddRefsDialog() : this(0) { }

        public AddRefsDialog(int initialTypeIndex) : this(null, initialTypeIndex) { }

        public AddRefsDialog(Post post, int initialTypeIndex)
        {
            InitializeComponent();
            this.ViewModel = 
                new CreateRefViewModel(post, NotifyingResource.Types[initialTypeIndex]);

            this.ViewModel.RegisterCloseAction(() => this.Close());
        }

        private void MarkupPreviewText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(MarkupPreviewText.Text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(Properties.Settings.Default.CloseConfirmations)
                e.Cancel = !Extender.WPF.ConfirmationDialog.Show();

            base.OnClosing(e);
        }
    }
}
