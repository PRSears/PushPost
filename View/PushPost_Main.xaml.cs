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
            Title = string.Format("PushPost - Post Builder [alpha {0}]", this.GetAssemblyVersion());

            #region Framerate cap hack...
            // HACK to reduce lag while typing
            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline), 
                new FrameworkPropertyMetadata { DefaultValue = 5 });
            #endregion
        }

        private string GetAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            return vi.FileVersion;
        }
    }
}
