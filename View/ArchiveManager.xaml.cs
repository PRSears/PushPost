using PushPost.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace PushPost.View
{
    /// <summary>
    /// Interaction logic for ArchiveManager.xaml
    /// </summary>
    public partial class ArchiveManager : Window
    {
        public ArchiveManager(Models.Database.ArchiveQueue archiveQueue)
        {
            InitializeComponent();
            DataContext         = new ArchiveViewModel(archiveQueue);

            SizeChanged        += ArchiveManager_SizeChanged;
            _resizeTimer.Tick  += _resizeTimer_Tick;
        }

        #region Auto-collapse hack job
        private System.Windows.Threading.DispatcherTimer _resizeTimer =
            new System.Windows.Threading.DispatcherTimer
            {
                Interval    = new TimeSpan(0, 0, 0, 0, 600),
                IsEnabled   = false
            };
        private bool _Collapsed     = false;

        private void _resizeTimer_Tick(object sender, EventArgs e)
        {
            // VK_LBUTTON = 0x01,
            // Returns negative when the button is DOWN and 0 when the button is UP
            if (GetKeyState(VK_LBUTTON) < 0) return;
            
            MaxWidth        = int.MaxValue;
            MinWidth        = 160;

            _resizeTimer.IsEnabled = false;
        }

        private void ArchiveManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();

            if (!_Collapsed)
            {
                if (e.NewSize.Width > 160 && e.NewSize.Width < 950)
                {
                    MaxWidth = 160;
                    _Collapsed = true;
                }
                else
                {
                    MaxWidth = int.MaxValue;
                    _Collapsed = false;
                }
            }
            else
            {
                if(e.NewSize.Width >= 170)
                {
                    MaxWidth = int.MaxValue;
                    MinWidth = 1050;
                    _Collapsed = false;
                }
                else
                {
                    Width = 160;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern short GetKeyState(UInt16 virtualKeyCode);

        public const UInt16 VK_LBUTTON = 0x01;
        #endregion

        private void DisplayedPostsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
                (sender as ListBox).UnselectAll();
        }
    }
}
