using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    public class InlineImage : IResource
    {
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
            // TODO implement ClientSide.Images to handle conversion, resizing, 
            //      collage creation, etc.
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
            return "<img src=\"" + this.Value + "\">"; 
        }
    }
}
