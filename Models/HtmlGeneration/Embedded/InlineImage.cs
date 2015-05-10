
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
        
        public InlineImage()
        { }

        public InlineImage(string name, string imagePath)
        {
            _Name  = name;
            _Value = imagePath;
        }

        /// <summary>
        /// Creates HTML markup of an image tag pointing to the local version
        /// of this Image.
        /// </summary>
        public override string CreateHTML()
        {
            // Make sure the path points up a directory
            if (LocalPath.StartsWith(@"..\"))
                return string.Format
                (
                    @"<img src=""{0}""/>",
                    LocalPath
                );
            else
                return string.Format
                (
                    @"<img src=""..\{0}""/>",
                    LocalPath
                );
        }
    }
}
