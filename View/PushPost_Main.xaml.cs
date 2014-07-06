using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using PushPost.ViewModels;
using System.Windows;
using System;

namespace PushPost
{
    // TODO Autosave timer

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PushPost_Main : Window
    {
        public PushPost_Main()
        {
            InitializeComponent();
            DataContext = new PostViewModel();

            //InitializeCommandBindings();
            //PostResourceReferences = new List<IResource>();

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline), 
                new FrameworkPropertyMetadata { DefaultValue = 5 });
        }

        #region Obsolete
        //public static RoutedCommand OpenRefsDialogCommand = new RoutedCommand();
        //public List<IResource> PostResourceReferences;
        //protected void InitializeCommandBindings()
        //{
        //    CommandBinding openRefsDialog = new CommandBinding(OpenRefsDialogCommand, ExecuteOpenRefsDialog, OpenRefsDialogCanExecute);

        //    this.CommandBindings.Add(openRefsDialog);

        //    this.AddHrefButton.Command = OpenRefsDialogCommand;
        //    this.AddCodeButton.Command = OpenRefsDialogCommand;
        //    this.AddFootButton.Command = OpenRefsDialogCommand;
        //    this.AddImgButton.Command  = OpenRefsDialogCommand;
        //}

        //private void ExecuteOpenRefsDialog(object sender, ExecutedRoutedEventArgs e)
        //{
        //    string bname = (e.Source as Button).Name.ToLower();

        //    if (bname.Contains("href"))
        //        Console.WriteLine("Sender is Href button");
        //    else if(bname.Contains("foot"))
        //        Console.WriteLine("Sender is Footnote button");
        //    else if(bname.Contains("code"))
        //        Console.WriteLine("Sender is Code button");
        //    else if(bname.Contains("img"))
        //        Console.WriteLine("Sender is Image button");
        //    else
        //        Console.WriteLine("Sender is unkown");

        //    var addRefsDialog = new AddRefsDialog(bname, this);
        //    addRefsDialog.Show();

        //    e.Handled = true;
        //}

        //private void OpenRefsDialogCanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //    e.Handled    = true;
        //}

        //private void SubmitPostButton_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    MessageBox.Show("SUBMIT");
        //}
        #endregion
    }
}
