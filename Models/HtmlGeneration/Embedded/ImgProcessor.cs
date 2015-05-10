using Extender;
using Extender.IO;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class ImgProcessor
    {
        public int[] OutSizes
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the IResource.Value is updated to the new 
        /// (organized) location of the image.
        /// Note: The new location will be based on the first element in the 
        /// OutSizes array. 
        /// 
        /// Defaults to True.
        /// </summary>
        public bool Move
        {
            get;
            set;
        }

        public ImgProcessor(int[] outSizes)
        {
            this.OutSizes   = outSizes;
            this.Move       = true;
        }

        public ImgProcessor(int outSize) : this(new int[] { outSize }) 
        { }

        public ImgProcessor() : this(new int[] { 0 }) 
        {}

        public ImgProcessor(System.Collections.Specialized.StringCollection outSizes)
        {
            this.OutSizes = new int[outSizes.Count];

            for(int i = 0; i < OutSizes.Length; i++)
            {
                int.TryParse(outSizes[i], out OutSizes[i]);
            }

            this.Move = true;
        }

        #region static organizer overloads
        public static List<string> OrganizeImage(InlineImage image, int[] outSizes)
        {
            ImgProcessor processor = new ImgProcessor(outSizes);
            return processor.Organize(image);
        }

        public static List<string> OrganizeImage(Photo image, int[] outSizes)
        {
            ImgProcessor processor = new ImgProcessor(outSizes);
            return processor.Organize(image);
        }

        public static List<string> OrganizeImage(InlineImage image, System.Collections.Specialized.StringCollection outSizes)
        {
            ImgProcessor processor = new ImgProcessor(outSizes);
            return processor.Organize(image);
        }

        public static List<string> OrganizeImage(Photo image, System.Collections.Specialized.StringCollection outSizes)
        {
            ImgProcessor processor = new ImgProcessor(outSizes);
            return processor.Organize(image);
        }

        #endregion

        public List<string> Organize(InlineImage image)
        {
            return Organize(new InlineImage[] { image });
        }

        public List<string> Organize(Photo image)
        {
            return Organize(new Photo[] { image });
        }

        public List<string> Organize(IEnumerable<IResource> images)
        {
            List<string> successfulPaths = new List<string>();

            foreach(IResource imageResource in images)
            {
                string subfolder;

                if (imageResource is InlineImage)
                {
                    subfolder = Path.Combine
                    (
                        Properties.Settings.Default.SiteExportFolder, 
                        Properties.Settings.Default.ImagesSubfolder
                    );
                }
                else if (imageResource is Photo)
                {
                    subfolder = Path.Combine
                    (
                        Properties.Settings.Default.SiteExportFolder,
                        Properties.Settings.Default.PhotosSubfolder
                    );
                }
                else continue; // IResource that can't be processed

                string origPath = imageResource.Value;

                // if path is relative, convert it to absolute
                if (origPath.StartsWith(@"..\")) 
                {
                    origPath = Path.GetFullPath(Path.Combine
                    (
                        subfolder,
                        origPath
                    ));
                }

                if (!Paths.IsLocalPath(origPath))
                {
                    // if the image is on the web, download a temp copy 
                    // & make sure the original IResource.Value doesn't get changed
                    this.Move = false;
                    string tempLocation = Path.Combine
                    (
                        Path.GetTempPath(), 
                        Path.GetFileName(origPath)
                    );

                    using(System.Net.WebClient client = new System.Net.WebClient())
                    {
                        client.DownloadFile
                        (
                            origPath,
                            tempLocation
                        );

                        origPath = tempLocation;
                    }                    
                }

                if (!File.Exists(origPath))
                {
                    bool result = Extender.WPF.ConfirmationDialog.Show
                    (
                        "Missing image file", 
                        "ImgProcessor could not find the following image file while it was trying to organize:\n " + 
                        origPath +
                        "\n\nDo you want to browse for a replacement?\n(Selecting no will throw an exception)"
                    );

                    if (result == true)
                    {
                        string replacementPath = SelectReplacementImage(Path.GetDirectoryName(origPath), origPath);
                        if (!string.IsNullOrWhiteSpace(replacementPath))
                            File.Copy(replacementPath, origPath); // THOUGHT not sure if this is the best way to go about things.

                        //  Instead of using 'replacementPath' make a copy of whatever is at replacementPath at origPath
                        //  that way the database doesn't need to updated every time something goes missing. The user can 
                        //  just be prompted to put it back.
                    }
                    else
                        throw new FileNotFoundException("ImgProcessor could not find the specified file.", origPath);
                }

                foreach(int size in OutSizes)
                {
                    string outDir = Path.Combine
                    (
                        subfolder, 
                        GetSizeString(size)
                    );

                    string newPath = Path.Combine
                    (
                        subfolder, 
                        GetSizeString(size), 
                        Path.GetFileName(origPath)
                    );

                    if (!Directory.Exists(outDir))
                        Directory.CreateDirectory(outDir);

                    if (origPath.Equals(newPath))
                        continue; 

                    using(Bitmap originalBmp = (Bitmap)Image.FromFile(origPath))
                    {
                        #region // Handle an existing file
                        if (File.Exists(newPath))
                        {
                            using(Bitmap existingBmp = (Bitmap)Image.FromFile(newPath))
                            {
                                if(Extender.ObjectUtils.BitmapCompare.CompareWithMemCmp(originalBmp, existingBmp))
                                {
                                    continue; // no work to do if they're the same.
                                }
                                else
                                {
                                    // Files aren't the same, just have the same name.
                                    bool replace;
                                    if (!Properties.Settings.Default.AutoReplaceImages)
                                    {
                                        replace = Extender.WPF.ConfirmationDialog.Show
                                        (
                                            "File already exists.",
                                            string.Format("There is already a file located at {0}.\nDo you want to replace it?\n\nNote: Selecting no will use the existing image instead.", newPath)
                                        );
                                    }
                                    else replace = true;

                                    if (replace) // replace it
                                    {
                                        existingBmp.Dispose();
                                        File.Delete(newPath);
                                    }
                                    else continue; // skip it
                                }
                            }
                        }
                        #endregion

                        if (size < 0) 
                            throw new System.ArgumentException("ImgProcessor.OutSizes[] contained an invalid size. Size cannot be less than 0.");
                        else if (size == 0)
                        {
                            originalBmp.Save(newPath);
                            successfulPaths.Add(newPath);
                        }
                        else
                        {
                            using (Bitmap resized = originalBmp.ResizeToLongEdge(size))
                            {
                                resized.Save(newPath);
                                successfulPaths.Add(newPath);
                            }
                        }
                    }
                } // foreach(int size in OutSizes)
                
                if(Move) // Update the Value (location) to reflect the new location where neccessary.
                {
                    // Create a path relative to where the pages' html files are located
                    string relativePath = Paths.MakeRelativePath
                    (
                        subfolder,
                        Path.Combine
                        (
                            subfolder,
                            GetSizeString(OutSizes[0]),
                            Path.GetFileName(origPath)
                        )
                    )
                    .Insert(0, @"..\");

                    imageResource.Value = relativePath;
                }

            } // foreach(IResource imageResource in images) 

            return successfulPaths;
        }

        protected string GetSizeString(int size)
        {
            string sizeString;

            if (size == 0) 
                sizeString = "orig";
            else if (size < 0) 
                throw new System.ArgumentException("Invalid size value. Size cannot be less than 0.");
            else 
                sizeString = size.ToString();

            return sizeString;
        }

        private string SelectReplacementImage(string initialDirectory, string missingFilename)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Title = string.Format("Select an image file to replace '{0}'.", missingFilename);
            dialog.InitialDirectory = initialDirectory;

            dialog.Filter = @"Image files (*.jpg, *.jpeg *.png, *.gif)
|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                return dialog.FileName;
            else 
                return string.Empty;
        }
    }
}
