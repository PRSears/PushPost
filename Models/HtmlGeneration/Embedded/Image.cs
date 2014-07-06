using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class InlineImage : IResource
    {
        //
        // TODO implement Model.ClientSide.Images to handle conversion, resizing, 
        //      collage creation, etc.
        // TODO I'll need a system to track where the resized / current image is stored 
        //      so that the upload-to-s3 classes can check to see if the images exist on 
        //      on the server and - if they're not - upload them.

        /// <summary>
        /// Gets or sets the name of the reference.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the value (local path to the image file) of this InlineImage 
        /// resource reference.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the path to the local instance of the image file referenced by this
        /// IResource.
        /// </summary>
        public string LocalPath
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets the path to the online (S3 bucket) instance of the image file 
        /// referenced by this IResource.
        /// </summary>
        public string WebPath
        {
            get
            {
                // TODO generate a path to the image in an S3 bucket
                throw new NotImplementedException();
            }
        }

        public InlineImage(string name, string imagePath)
        {
            Name = name;
            Value = imagePath;
        }

        public void Resize(System.Drawing.Size newSize)
        {
            throw new NotImplementedException();
        }

        public void Convert(System.Drawing.Imaging.ImageFormat newFormat)
        {
            
            throw new NotImplementedException();
        }

        public void Upload()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates HTML markup of an image tag pointing to the local version
        /// of this Image.
        /// </summary>
        public string CreateHTML()
        {
            return this.CreateHTML(true);
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
                local ? LocalPath : WebPath
                );
        }
    }
}
