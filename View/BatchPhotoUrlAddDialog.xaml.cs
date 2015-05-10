using PushPost.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for BatchPhotoUrlAddDialog.xaml
    /// </summary>
    public partial class BatchPhotoUrlAddDialog : Window
    {
        private BatchUrlAddDialogViewModel ViewModel
        {
            get
            {
                if (DataContext is BatchUrlAddDialogViewModel)
                    return (BatchUrlAddDialogViewModel)DataContext;
                else
                    return null;
            }
            set
            {
                DataContext = value;
            }
        }

        public List<PushPost.Models.HtmlGeneration.Embedded.Photo> Photos
        {
            get;
            set;
        }

        public BatchPhotoUrlAddDialog()
        {
            InitializeComponent();

            this.Photos = new List<Models.HtmlGeneration.Embedded.Photo>();

            ViewModel = new BatchUrlAddDialogViewModel(Photos);
            ViewModel.RegisterCloseAction(() => this.Close());
        }
    }
}
