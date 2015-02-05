using PushPost.Models.HtmlGeneration.Embedded;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace PushPost.Models.HtmlGeneration
{
    public class PhotoPost : Post
    {
        protected string AlbumClass;

        public PhotoPost() : base()
        {
            AlbumClass  = "photo-container"; // TODO_ Load class IDs from a cfg file or implement a seperate settings panel for them.
            Category    = NavCategory.Photography;
        }

        public PhotoPost(string title, DateTime timestamp, string author) : this()
        {
            Title       = title;
            Timestamp   = timestamp;
            Author      = author;
        }

        public PhotoPost(string title, DateTime timestamp, string author, string body)
            : this(title, timestamp, author)
        {
            MainText = body;
        }


        protected override void RenderHeader(System.Web.UI.HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Id, this.UniqueID.ToString());
            w.RenderBeginTag(HtmlTextWriterTag.H1);
            w.Write(this.TitleLink);
            w.RenderEndTag();
        }

        protected override void RenderBody(System.Web.UI.HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Class, AlbumClass);
            w.RenderBeginTag(HtmlTextWriterTag.Div);

                w.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach(IResource r in this.Resources)
                {
                    Photo photo;

                    if (r is Photo) photo = (Photo)r;
                    else continue;

                    w.RenderBeginTag(HtmlTextWriterTag.Li);
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.RenderBeginTag(HtmlTextWriterTag.Span);
                            w.Write(photo.Name);
                        w.RenderEndTag(); // </span>
                        w.RenderEndTag(); // </p>

                        w.Write(photo.CreateHTML());

                    w.RenderEndTag(); // </li>
                }
                w.RenderEndTag(); // </ul>
            
            w.RenderEndTag(); // </div>
        }

        protected override void RenderPreviewBody(HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Id, base.PostBodyID);
            w.RenderBeginTag(HtmlTextWriterTag.Div);
            using (StringReader reader = new StringReader(this.ParsedMainText))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    w.RenderBeginTag(HtmlTextWriterTag.P);
                    w.Write(line);
                    w.RenderEndTag();
                }
            }

            // TODO Decide how the preview appears:
            //
            //      1) Show the first photo only
            //      2) Create a simple collage (maybe in a stack?)
            //      3) Show all the photos, but smaller (seems stupid)

            List<IResource> photos = Resources.Where(r => (r is Photo))
                                              .ToList();

            if(photos.Count > 0)
                w.Write(photos[0].CreateHTML());

            w.RenderEndTag();
        }

        protected override void RenderFooter(HtmlTextWriter w)
        {
            // no footers needed at the moment
        }
        protected override void RenderComments(HtmlTextWriter w)
        {
            if (this.IncludePostEndComments)
                w.WriteComment(" ********* End of photo post: " + Title + " ********* ");
        }

        public static Post TemplatePost()
        {
            return new PhotoPost("Enter Title", DateTime.Now, "Enter Author", "Enter album description.");
        }

        public override void ResetToTemplate()
        {
            PhotoPost template = (PhotoPost)TemplatePost();

            this.Title      = template.Title;
            this.Author     = template.Author;
            this.MainText   = template.MainText;
            this.Timestamp  = template.Timestamp;
            this.Category   = template.Category;

            this.Footers    = new List<Embedded.Footer>(); 
            this.Resources  = new List<Embedded.IResource>();
            this.Tags       = new List<Embedded.Tag>();
        }

        public static string TestHarness()
        {
            PhotoPost testAlbum = new PhotoPost("Test Album", DateTime.Now, DefaultAuthor, 
                "This is the text that will appear as a description for the album");

            Photo test_photo1 = new Photo();
            test_photo1.Name = "Hover text 1";
            test_photo1.Value = @"http://psears.web.s3.amazonaws.com/gallery/0002.jpg";

            Photo test_photo2 = new Photo();
            test_photo2.Name = "hover text 2";
            test_photo2.Value = @"http://psears.web.s3.amazonaws.com/gallery/0001.jpg";

            //testAlbum.Photos.Add(test_photo1);
            //testAlbum.Photos.Add(test_photo2);

            return testAlbum.Create() + "\n\n" + testAlbum.CreatePreview();
        }

        #region Settings.Settings aliases
        private static string DefaultAuthor
        {
            get
            {
                return Properties.Settings.Default.DefaultAuthor;
            }
        }
        #endregion
    }
}