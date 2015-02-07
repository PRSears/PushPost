using PushPost.ViewModels;
using System.Windows;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for BatchPhotoAdder.xaml
    /// </summary>
    public partial class BatchPhotoAdder : Window
    {
        private BatchPhotoAddViewModel ViewModel
        {
            get
            {
                if (DataContext is BatchPhotoAddViewModel)
                    return (BatchPhotoAddViewModel)DataContext;
                else
                    return null;
            }
            set
            {
                DataContext = value;
            }
        }
        
        public BatchPhotoAdder(PushPost.Models.HtmlGeneration.Post parent)
        {
            InitializeComponent();           

            ViewModel = new BatchPhotoAddViewModel(parent);
            ViewModel.RegisterCloseAction(() => this.Close());

            this.PhotosItemsControl.ItemsSource = ViewModel.PhotoControls;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.CloseConfirmations)
                e.Cancel = !Extender.WPF.ConfirmationDialog.Show();

            base.OnClosing(e);
        }
    }
}
