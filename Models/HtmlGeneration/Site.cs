using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using PushPost.Models.Database;
using Extender;
using Extender.Debugging;
using Microsoft.WindowsAPICodePack.Dialogs;

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

            PageBuilder site = new PageBuilder(
                posts,
                Properties.Settings.Default.PostsPerPage);

            site.CreatePages();
            site.SavePages(exportFolder);
        }

        /// <summary>
        /// Generates a new site with all posts pulled from the database.
        /// </summary>
        public static void Create()
        {
            using(Archive database = new Archive())
            {
                Create(database.Dump());
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

            PageBuilder previewer = new PageBuilder(posts);

            previewer.CreatePages();
            previewer.SavePages(previewFolder);

            string firstFilePath = Path.Combine(
                previewFolder,
                previewer.Pages[0].FullName);

            System.Diagnostics.Process browserProc = new System.Diagnostics.Process();

            browserProc.StartInfo.FileName          = firstFilePath;
            browserProc.StartInfo.UseShellExecute   = true;
            browserProc.Start();
        }

        public static void Preview(Post post)
        {
            Preview(new Post[] { post });
        }
    }
}
