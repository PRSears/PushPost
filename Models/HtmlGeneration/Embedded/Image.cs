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

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string CreateHTML()
        {
            // TODO Generate path pointing to S3 bucket instead of local file
            return string.Format(@"<img src=""{0}""/>");
            //return "<img src=\"" + this.Value + "\">"; 
        }
    }
}
