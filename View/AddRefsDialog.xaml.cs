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
        //
        // TODO AddRefsDialog could save the ref information to an XML file that any 
        //      window can read from.
        //      Remember to delete file when the application closes +/ opens again.
        //
        //      Simply adding to a list handled by the viewmodel is probably the best 
        //      solution for this task,

        public PushPost_Main ParentWindow
        {
            get;
            set;
        }

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

        public AddRefsDialog()
        {
            InitializeComponent();
            DataContext = new NewReferenceViewModel(typeof(InlineImage)); // HACK debugging
        }

        public AddRefsDialog(Control parent):this()
        {
            if (parent is PushPost_Main)
                ParentWindow = (PushPost_Main)parent;
        }

        public AddRefsDialog(string senderName, Control parent):this(parent)
        {
            // TODO open to correct pane
            this.RefNameField.Text = senderName;
        }

        private void MarkupPreviewText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Markup);
        }

        //
        // TOOD Create Commands for ADD and CANCEL buttons.
        //      Add should add the generated IResource to ParentWindow.PostResourceReferences.

        #region Link RefNameField --> MarkupPreviewText
        /// <summary>
        /// Link the (editable) name field with the label to preview what the markup should look like.
        /// </summary>
        private static void RefNameField_TextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((AddRefsDialog)source).RefNameField.Text = (string)e.NewValue;
        }

        private void RefNameField_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefName = (sender as TextBox).Text;
            this.MarkupPreviewText.Text = Markup;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text", 
                typeof(string), 
                typeof(AddRefsDialog), 
                new PropertyMetadata(null, RefNameField_TextChanged));

        #endregion

    }
}
