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

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : Window
    {
        public SettingsViewModel ViewModel
        {
            get
            {
                if (DataContext is SettingsViewModel)
                    return (DataContext as SettingsViewModel);
                else
                    return null;
            }
            set
            {
                DataContext = value;
            }
        }

        public SettingsEditor()
        {
            InitializeComponent();

            this.ViewModel = new SettingsViewModel();
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(
            object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Extender.Debugging.Debug.WriteMessage("Settings property changed.", ViewModel.Debug,"info");
            Properties.Settings.Default.Save();
        }
    }
}
