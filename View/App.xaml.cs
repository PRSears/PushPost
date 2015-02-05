using PushPost.Properties;
using System;
using System.Windows;

namespace PushPost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.IO.StreamWriter LogStream;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // "Workaround" to reduce lag while typing
            // Caps framerate at 5fps.
            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 5 });

            //
            // Redirect output from console / debugging to a text file on disc
            if (Settings.Default.DEBUG && !string.IsNullOrWhiteSpace(Settings.Default.DebugLogPath))
            {
                LogStream = new System.IO.StreamWriter
                (
                    Settings.Default.DebugLogPath,
                    true
                );

                LogStream.AutoFlush = true;

                Console.SetOut(LogStream);

                Extender.Debugging.Debug.WriteMessage
                (
                    "Application Startup.",
                    "info"
                );
            }
        }
    }
}

// TODO_ Clean up ResourceCommands.cs (switch over to RelayCommands)