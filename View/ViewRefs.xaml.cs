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
using PushPost.ViewModels;
using PushPost.Models.HtmlGeneration;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for ViewRefs.xaml
    /// </summary>
    public partial class ViewRefs : Window
    {
        private ViewRefsViewModel ViewModel
        {
            get
            {
                if (DataContext is ViewRefsViewModel)
                    return (DataContext as ViewRefsViewModel);
                else
                    return null;
            }
            set
            {
                DataContext = value;
            }
        }

        public ViewRefs()
        {
            Post dummy = TextPost.Dummy();

            Models.HtmlGeneration.Embedded.Code res = new Models.HtmlGeneration.Embedded.Code(
                "a_code", "Console.WriteLine()");

            for (int i = 0; i < 3; i++)
                dummy.Resources.Add(res);

            InitializeComponent();
            ViewModel = new ViewRefsViewModel(dummy);
        }
    }
}
