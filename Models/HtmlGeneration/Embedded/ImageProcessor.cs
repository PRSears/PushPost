﻿using Extender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Debug = Extender.Debugging.Debug;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class ImageProcessor
    {
        public string OutputDirectory
        {
            get;
            set;
        }

        public int[] Resizes
        {
            get;
            set;
        }

        public ImageProcessor(string outDirectory) :
            this(outDirectory, new int[] { 0 })
        { }

        public ImageProcessor(string outDirectory, int[] resizes)
        {
            this.OutputDirectory = outDirectory;
            this.Resizes = resizes;
        }

        public ImageProcessor(string outDirectory, System.Collections.Specialized.StringCollection resizes) :
            this(outDirectory)
        {
            Resizes = new int[resizes.Count];
            for(int i = 0; i < Resizes.Length; i++)
            {
                int.TryParse(resizes[i], out Resizes[i]);
            }
        }

        /// <summary>
        /// Resizes, copies, and organizes all images in the provided 
        /// list to this.OutputDirectory.
        /// </summary>
        /// <param name="images">List of images to be organized.</param>
        /// <returns>Returns the number of successfully organized images.</returns>
        public int Organize(List<InlineImage> images)
        {
            return OrganizeGeneric(images.Cast<IResource>().ToList());
        }

        /// <summary>
        /// Resizes, copies, and organizes all photos in the provided 
        /// list to this.OutputDirectory.
        /// </summary>
        /// <param name="images">List of photos to be organized.</param>
        /// <returns>Returns the number of successfully organized images.</returns>
        public int Organize(List<Photo> images)
        {
            return OrganizeGeneric(images.Cast<IResource>().ToList());
        }

        protected int OrganizeGeneric(List<IResource> images)
        {
            int imagesOrganized = 0;

            foreach (IResource img in images)
            {
                string originalLocation = img.Value;
                foreach (int size in Resizes)
                {
                    // Make sure the directory is there
                    if (!EnsureDirectory(originalLocation, size))
                    {
                        Debug.WriteMessage
                        (
                            string.Format("There was a problem creating the directory '{0}'",
                                GetNewImageDirectory(originalLocation, size)),
                            "warn"
                        );
                        continue;
                    }

                    // Find a unique filename
                    int version = 1;
                    while (File.Exists(GenerateNewImagePath(originalLocation, size)))
                    {
                        img.Value = img.Value.InsertBeforeExtension(string.Format
                            (
                                "_{0}",
                                (++version).ToString("D3")
                            ));
                    }

                    // Resize and save to new location(s)
                    using (Bitmap original = (Bitmap)Image.FromFile(originalLocation))
                    {
                        if (size < 0) continue;
                        else if (size == 0)
                        {
                            original.Save(GenerateNewImagePath(originalLocation, size));
                            imagesOrganized++;
                        }
                        else
                        {
                            using (Bitmap resized = original.ResizeToLongEdge(size))
                            {
                                resized.Save(GenerateNewImagePath(originalLocation, size));
                                imagesOrganized++;
                            }
                        }
                    }
                }
                
                // Change the path to point to one of the new organized images.
                img.Value = Extender.IO.Paths.MakeRelativePath
                    (
                        OutputDirectory, 
                        GenerateNewImagePath(img.Value, Resizes[0])
                    );
            }

            return imagesOrganized;
        }

        protected bool EnsureDirectory(string origPath, int forSize)
        {
            try
            {
                if (!Directory.Exists(GetNewImageDirectory(origPath, forSize)))
                    Directory.CreateDirectory(GetNewImageDirectory(origPath, forSize));
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates a full path (including filename and extension) pointing
        /// to the new file after organizing.
        /// </summary>
        /// <param name="origPath">Path of the original file to be organized.</param>
        /// <param name="forSize">The size of the organized image. Used for sorting into a subdirectory.</param>
        /// <returns>Full path of the new location for the image of specified size.</returns>
        public string GenerateNewImagePath(string origPath, int forSize)
        {
            string size;

            if (forSize == 0) size = "orig";
            else if (forSize < 0) throw new ArgumentException("Invalid size value. Size cannot be less than 0.");
            else size = forSize.ToString();

            return Path.Combine
            (
                OutputDirectory,
                size,
                Path.GetFileName(origPath)
            );
        }

        /// <param name="origPath">Path of the original file to be organized.</param>
        /// <param name="forSize">The size of the organized image. Used for sorting into a subdirectory.</param>
        /// <returns>Gets the directory component of the path of the new location of the image of specified size.</returns>
        public string GetNewImageDirectory(string origPath, int forSize)
        {
            return (new FileInfo(GenerateNewImagePath(origPath, forSize)).DirectoryName);
        }

        /// <summary>
        /// Organizes the provided image to specified output folder for the sizes described in the collection.
        /// </summary>
        public static void OrganizeImage(InlineImage img, string outFolder, System.Collections.Specialized.StringCollection sizes)
        {
            ImageProcessor processor = new ImageProcessor(outFolder, sizes);

            processor.Organize(new List<InlineImage>(new InlineImage[] { img }));
        }
    }
}
