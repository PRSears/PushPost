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
using System.Runtime.CompilerServices;

namespace PushPost
{
    /// <summary>
    /// Interaction logic for AddRefsWindow.xaml
    /// </summary>
    public partial class AddRefsWindow : Window
    {

        //private string _RefName;
        public string RefName
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                if(RefName != value)
                {
                    SetValue(TextProperty, value);
                }
            }
        }

        public string Markup
        {
            get
            {
                return string.Format(@"+@({0})", RefName);
            }
        }

        public AddRefsWindow()
        {
            InitializeComponent();


        }

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(AddRefsWindow), new PropertyMetadata(null, RefNameField_TextChanged));

        private static void RefNameField_TextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((AddRefsWindow)source).RefNameField.Text = (string)e.NewValue;
        }

        private void RefNameField_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefName = (sender as TextBox).Text;
            this.MarkupPreviewText.Text = Markup;
        }
    }
}
