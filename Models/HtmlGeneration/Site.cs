using Extender.Debugging;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Models.Database;
using System.IO;

namespace PushPost.Models.HtmlGeneration
{
    /// <remarks>
    /// Helper class containing static methods for the creation of a full site.
    /// </remarks>
    public class Site
    {
        /// <summary>
        /// Generates a new site with provided posts.
        /// </summary>
        public static void Create(Post[] posts)
        {
            string exportFolder = Properties.Settings.Default.SiteExportFolder;
            if(string.IsNullOrWhiteSpace(exportFolder))
            {
                var dialog              = new CommonOpenFileDialog();
                dialog.IsFolderPicker   = true;
                dialog.Title            = "Select a folder to save the site in";

                CommonFileDialogResult r = dialog.ShowDialog();
                if (r != CommonFileDialogResult.Ok) return;

                exportFolder = dialog.FileName;
                Properties.Settings.Default.SiteExportFolder = exportFolder;
                Properties.Settings.Default.Save();
            }

            Extender.WPF.BusySplash.Show
            (
                () =>
                {
                    PageBuilder site = new PageBuilder
                    (
                        posts,
                        Properties.Settings.Default.PostsPerPage
                    );

                    site.CreatePages();
                    site.SavePages(exportFolder);
                },
                @"/img/working.png"
            );
        }

        /// <summary>
        /// Generates a new site with all posts pulled from the database.
        /// </summary>
        public static void Create()
        {
            try
            {
                using (Archive database = new Archive())
                {
                    Create(database.Dump());
                }
            }
            catch(System.Data.SqlClient.SqlException e)
            {
                System.Windows.Forms.MessageBox.Show
                    (e.Message, "Database exception", System.Windows.Forms.MessageBoxButtons.OK);
                ExceptionTools.WriteExceptionText(e, true);
            }
        }

        /// <summary>
        /// Generates a new site from the provided posts - saving them in a 
        /// temporary location - and opens the first page in the default web
        /// browser.
        /// </summary>
        public static void Preview(Post[] posts)
        {
            string previewFolder = Properties.Settings.Default.PreviewFolderPath;
            if (string.IsNullOrWhiteSpace(previewFolder))
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.Title = "Select a folder to save temporary preview files in";

                CommonFileDialogResult r = dialog.ShowDialog();
                if (r != CommonFileDialogResult.Ok) return;

                previewFolder = dialog.FileName;
                Properties.Settings.Default.PreviewFolderPath = previewFolder;
                Properties.Settings.Default.Save();
            }

            string firstFilePath = string.Empty;

            Extender.WPF.BusySplash.Show
            (
                () =>
                {
                    PageBuilder previewer = new PageBuilder(posts);

                    previewer.CreatePages();
                    previewer.SavePages(previewFolder);
                    firstFilePath = Path.Combine
                    (
                        previewFolder,
                        previewer.Pages[0].FullName
                    );
                },
                @"/img/working.png"
            );

            System.Diagnostics.Process browserProc = new System.Diagnostics.Process();

            browserProc.StartInfo.FileName          = firstFilePath;
            browserProc.StartInfo.UseShellExecute   = true;
            browserProc.Start();
        }

        public static void Preview(Post post)
        {
            Preview(new Post[] { post });
        }

        public static bool CheckSiteExportFolder()
        {
            if(string.IsNullOrWhiteSpace(Properties.Settings.Default.SiteExportFolder))
            {
                System.Windows.Forms.MessageBox.Show("You need to select a folder " +
                    "to create the site in before an image can be added.");

                int pesterings = 0;
                while(!SelectSiteExportFolder() && pesterings < 3)
                {
                    pesterings++;
                }

                if (pesterings > 2 || string.IsNullOrWhiteSpace(Properties.Settings.Default.SiteExportFolder))
                    return false;
            }

            return Directory.Exists(Properties.Settings.Default.SiteExportFolder);
        }

        /// <summary>
        /// Prompts the user to select a folder to export the site into.
        /// </summary>
        /// <returns>Returns true if a folder was successfully set by the user. False otherwise.</returns>
        public static bool SelectSiteExportFolder()
        {
            var dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;
            dialog.Title = string.Format("Select a folder to export the site into");

            CommonFileDialogResult r = dialog.ShowDialog();
            if (r != CommonFileDialogResult.Ok)
            {
                Debug.WriteMessage("User failed to select a SiteExportFolder.", "warn");
                return false;
            }
            else
            {
                Properties.Settings.Default.SiteExportFolder = dialog.FileName;
                return true;
            }
        }
    }
}
