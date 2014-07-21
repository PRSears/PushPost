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
using Extender;
using System.Runtime.InteropServices;

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
            if (GetKeyState(0x01) < 0) return;
            
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
                if (e.NewSize.Width > 160 && e.NewSize.Width < 850)
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
                    MinWidth = 870;
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
    }
}
