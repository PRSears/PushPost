using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Extender;
using Extender.Debugging;
using Extender.Drawing;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class InlineImage : NotifyingResource
    {        
        /// <summary>
        /// Gets the path to the local instance of the image file referenced by this
        /// IResource.
        /// </summary>
        public string LocalPath
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the path to the instance of this image file after it has been resized
        /// and renamed. 
        /// </summary>
        public string RelativeWebPath
        {
            get
            {
                return Path.Combine(
                    @"..\",
                    Properties.Settings.Default.ImagesSubfolder,
                    Resizes[2].ToString(),
                    Path.GetFileName(LocalPath));
            }
        }

        /// <summary>
        /// Generates the relative path to this image for the variant of specified size.
        /// </summary>
        /// <param name="forSize">Length of the resized image's longest edge, in pixels.
        /// 0 will return the Fullname for the original size file.</param>
        /// <returns>Relative path to the image file.</returns>
        public string Fullname(int forSize)
        {
            string size;

            if (forSize == 0)
            {
                size = "orig";
            }
            else if (forSize < 0) throw new ArgumentException("Invalid size option in Fullname.");
            else size = forSize.ToString();

            return Path.Combine(
                Properties.Settings.Default.SiteExportFolder,
                Properties.Settings.Default.ImagesSubfolder,
                size,
                Path.GetFileName(LocalPath));
        }

        public string DirectoryName(int forSize)
        {
            return (new FileInfo(Fullname(forSize)).DirectoryName);
        }

        private System.Collections.Specialized.StringCollection ImageSizes
        {
            get
            {
                return Properties.Settings.Default.ImageSizes;
            }
        }
        protected int[] Resizes { get; private set; }

        public InlineImage()
        {
            Resizes = new int[ImageSizes.Count];
            for(int i = 0; i < Resizes.Length; i++)
            {
                int.TryParse(ImageSizes[i], out Resizes[i]);
            }
        }

        public InlineImage(string name, string imagePath):this()
        {
            _Name  = name;
            _Value = imagePath;
        }

        public void Proccess()
        {
            foreach(int size in Resizes)
            {
                // Ensure directory exists
                if(!Directory.Exists(DirectoryName(size)))
                {
                    Directory.CreateDirectory(DirectoryName(size));
                }

                // Ensure filename is unique
                int v = 2;
                string newValue = string.Empty;
                while(File.Exists(Fullname(size)))
                {
                    newValue = this.Value.InsertBeforeExtension(v.ToString("D3"));
                }

                if (!string.IsNullOrEmpty(newValue)) this._Value = newValue;

                // Resize & save
                using (Bitmap original = (Bitmap)Image.FromFile(LocalPath))
                {
                    if      (size  < 0) continue;
                    else if (size == 0)
                    {
                        original.Save(this.Fullname(size), ImageFormat.Png);
                        continue;
                    }

                    using(Bitmap resized = original.ResizeToLongEdge(size))
                    {
                        resized.Save(Fullname(size));
                    }
                    //original.ResizeToLongEdge(size).Save(Fullname(size));
                }
            }
        }

        /// <summary>
        /// Creates HTML markup of an image tag pointing to the local version
        /// of this Image.
        /// </summary>
        public override string CreateHTML()
        {
            return this.CreateHTML(false);
        }

        /// <summary>
        /// Creates HTML markup of an image tag pointing to this Image.
        /// </summary>
        /// <param name="local">
        /// If true, generated HTML will point to a local instance of the image.
        /// Otherwise, the markup will point to the instance uploaded to an S3 bucket.
        /// </param>
        public string CreateHTML(bool local)
        {
            return string.Format(
                @"<img src=""{0}""/>",
                local ? LocalPath : RelativeWebPath
                );
        }
    }
}
