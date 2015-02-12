using Extender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// Flag to control whether '..\' should be inserted at the front of the 
        /// path in IResource.Value.
        /// </summary>
        public bool MakeFinalPathRelative
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to determine if each IResource.Value (path) property should be 
        /// changed to point to the new, processed location. 
        /// Defaults to true.
        /// </summary>
        public bool UpdateOriginalValue
        {
            get;
            set;
        }

        public ImageProcessor(string outDirectory) :
            this(outDirectory, new int[] { 0 })
        { }

        public ImageProcessor(string outDirectory, int[] resizes)
        {
            this.OutputDirectory        = outDirectory;
            this.Resizes                = resizes;
            this.MakeFinalPathRelative  = false;
            this.UpdateOriginalValue    = true;
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
        /// <returns>Returns the paths to the successfully organized images.</returns>
        public List<string> Organize(List<InlineImage> images)
        {
            return OrganizeGeneric(images.Cast<IResource>().ToArray());
        }

        /// <summary>
        /// Resizes, copies, and organizes all photos in the provided 
        /// list to this.OutputDirectory.
        /// </summary>
        /// <param name="images">List of photos to be organized.</param>
        /// <returns>Returns the paths to the successfully organized images.</returns>
        public List<string> Organize(List<Photo> images)
        {
            return OrganizeGeneric(images.Cast<IResource>().ToArray());
        }

        /// <summary>
        /// Resizes, copies, and organizes all photos in the provided 
        /// list to this.OutputDirectory.
        /// </summary>
        /// <param name="images">List of photos to be organized.</param>
        /// <returns>Returns the paths to the successfully organized images.</returns>
        public List<string> Organize(IEnumerable<Photo> images)
        {
            return OrganizeGeneric(images.Cast<IResource>().ToArray());
        }

        protected List<string> OrganizeGeneric(IResource[] images)
        {
            List<string> organizedImages = new List<string>();

            foreach(IResource img in images)
            {
                string originalLocation = img.Value;
                if (originalLocation.StartsWith(@"..\")) // make absolute
                    originalLocation = Path.GetFullPath(Path.Combine(OutputDirectory, originalLocation));

                foreach(int size in Resizes)
                {
                    if (!EnsureDirectory(size))
                        continue; // something fucked up while creating the directory

                    if (File.Exists(NewImagePath(originalLocation, size)))
                        File.Delete(NewImagePath(originalLocation, size));

                    if (size < 0) continue; // invalid size
                    else if (size == 0)
                    {
                        File.Copy(originalLocation, NewImagePath(originalLocation, size)); // just need to copy it
                        organizedImages.Add(RelativePath(NewImagePath(originalLocation, size)));
                    }
                    else
                    {
                        using (Bitmap original = (Bitmap)Image.FromFile(originalLocation))
                        using (Bitmap resized = original.ResizeToLongEdge(size))
                        {
                            organizedImages.Add(RelativePath(NewImagePath(originalLocation, size)));
                            resized.Save(NewImagePath(originalLocation, size));
                        }
                    }
                }
            }

            return organizedImages;
        }


        protected string RelativePath(string fullpath)
        {
            string newPath = string.Empty;

            newPath = Extender.IO.Paths.MakeRelativePath
            (
                OutputDirectory,
                fullpath
            );

            if (MakeFinalPathRelative) newPath = newPath.Insert(0, @"..\");

            return newPath;
        }

        protected bool EnsureDirectory(int forSize)
        {
            try
            {
                if (!Directory.Exists(GetNewImageDirectory(forSize)))
                    Directory.CreateDirectory(GetNewImageDirectory(forSize));
            }
            catch(Exception e)
            {
                Extender.Debugging.ExceptionTools.WriteExceptionText(e, true);
                return false;
            }

            return true;
        }
        
        public string NewImagePath(string origPath, int forSize)
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
        public string GetNewImageDirectory(int forSize)
        {
            //return (new FileInfo(GenerateNewImagePath(origPath, forSize)).DirectoryName);
            string size;

            if (forSize == 0) size = "orig";
            else if (forSize < 0) throw new ArgumentException("Invalid size value. Size cannot be less than 0.");
            else size = forSize.ToString();

            return Path.Combine
            (
                OutputDirectory,
                size
            );
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
