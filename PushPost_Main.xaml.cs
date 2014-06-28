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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PushPost
{
    // TODO Autosave timer

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PushPost_Main : Window
    {
        public static RoutedCommand BloopCommand = new RoutedCommand();

        public PushPost_Main()
        {
            InitializeComponent();
            InitializeCommandBindings();

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline), 
                new FrameworkPropertyMetadata { DefaultValue = 5 });
        }

        protected void InitializeCommandBindings()
        {
            CommandBinding bind = new CommandBinding(BloopCommand, ExecuteBloop, BloopCanExecute);

            this.CommandBindings.Add(bind);

            ManageTagsButton.Command = BloopCommand;
        }

        private void ExecuteBloop(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Test button pressed.");
            e.Handled = true;
        }

        private void BloopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute    = true;
            e.Handled       = true;
        }
    }
}
