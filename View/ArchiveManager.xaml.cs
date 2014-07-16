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
using PushPost.ViewModels;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for ArchiveManager.xaml
    /// </summary>
    public partial class ArchiveManager : Window
    {
        public ArchiveManager()
        {
            InitializeComponent();
            DataContext = new ArchiveViewModel();
        }
    }
}
