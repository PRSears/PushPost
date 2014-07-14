using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PushPost.ViewModels;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Runtime.CompilerServices;

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
            DataContext = new CreateRefViewModel(0);
            RegisterCloseAction();
        }

        public AddRefsDialog(int initialTypeIndex)
        {
            InitializeComponent();
            DataContext = new CreateRefViewModel(initialTypeIndex);
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
    }
}
