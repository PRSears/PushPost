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
        public string AlbumClass            { get; set; }
        public string AlbumThumbnailClass   { get; set; }
        public string DescriptionSpanClass  { get; set; }
        public string OverlayClass          { get; set; }
        public string PhotoHeaderClass      { get; set; }
        public string ImageIDFormat         { get; set; }

        public string StartButtonClass      { get; set; }
        public string BackSpanClass         { get; set; }
        public string ForwardSpanClass      { get; set; }
        public string StartButtonText       { get; set; }
        public string BackButtonText        { get; set; }
        public string ForwardButtonText     { get; set; }

        protected string DefaultImageDescription
        {
            get
            {
                return Properties.Settings.Default.DefaultImageDescription;
            }
        }

        public PhotoPost() : base()
        {
            AlbumClass              = "photo-container"; // TODO_ Load class IDs from a cfg file or implement a seperate settings panel for them.
            AlbumThumbnailClass     = "album-thumbnail";
            DescriptionSpanClass    = "description";
            FullPostLinkText        = "... [Full Album]";
            PostBodyClass           = "photo-post-body";
            PhotoHeaderClass        = "photo-header";
            OverlayClass            = "dark-overlay";
            WrapperClass            = "photo-wrapper";
            ImageIDFormat           = "image-{0}";

            StartButtonClass        = "start-button";
            BackSpanClass           = "photo-navL";
            ForwardSpanClass        = "photo-navR";
            StartButtonText         = "tap to start";
            BackButtonText          = @"&lt;";
            ForwardButtonText       = @"&gt;";

            Category = NavCategory.Photography;
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

        public override string Create()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                RenderBody(writer);
                RenderFooter(writer);
                RenderComments(writer);

                return buffer.ToString();
            }
        }

        public override string CreatePreview()
        {
            using(StringWriter buffer = new StringWriter())
            using(HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                RenderPreviewBody(writer);
                RenderComments(writer);

                return buffer.ToString();
            }
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
            w.AddAttribute(HtmlTextWriterAttribute.Class, this.PhotoHeaderClass);
            w.RenderBeginTag(HtmlTextWriterTag.Div);
            this.RenderHeader(w); // Title header
            using (StringReader reader = new StringReader(this.CleanedMainText)) // main text
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    w.RenderBeginTag(HtmlTextWriterTag.P);
                    w.Write(line);
                    w.RenderEndTag();
                }
            }
            w.RenderEndTag();            

            w.AddAttribute(HtmlTextWriterAttribute.Class, AlbumClass);
            w.RenderBeginTag(HtmlTextWriterTag.Div);

                // start button
                w.AddAttribute(HtmlTextWriterAttribute.Class, StartButtonClass);
                w.RenderBeginTag(HtmlTextWriterTag.Div);
                    w.RenderBeginTag(HtmlTextWriterTag.Span);
                        w.AddAttribute(HtmlTextWriterAttribute.Href, string.Format(ImageIDFormat, 0).Insert(0, @"#"));
                        w.RenderBeginTag(HtmlTextWriterTag.A);
                            w.Write(StartButtonText);
                        w.RenderEndTag();
                    w.RenderEndTag();
                w.RenderEndTag();

                w.RenderBeginTag(HtmlTextWriterTag.Ul); // image gallery
                Photo[] photos = this.Resources.OfType<Photo>().ToArray();
                for(int i = 0; i < photos.Length; i++)
                {
                    w.AddAttribute(HtmlTextWriterAttribute.Id, string.Format(ImageIDFormat, i));
                    w.RenderBeginTag(HtmlTextWriterTag.Li);

                    //navigation
                    if(i > 0)
                    {
                        w.AddAttribute(HtmlTextWriterAttribute.Class, BackSpanClass);
                        w.RenderBeginTag(HtmlTextWriterTag.Span);
                            w.AddAttribute(HtmlTextWriterAttribute.Href, string.Format(ImageIDFormat, i - 1).Insert(0, @"#"));
                            w.RenderBeginTag(HtmlTextWriterTag.A);
                                w.Write(BackButtonText);
                            w.RenderEndTag();
                        w.RenderEndTag();
                    }

                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.AddAttribute(HtmlTextWriterAttribute.Class, DescriptionSpanClass);
                        w.RenderBeginTag(HtmlTextWriterTag.Span);
                            if(!photos[i].Name.Equals(DefaultImageDescription))
                                w.Write(photos[i].Name);
                        w.RenderEndTag(); // </span>
                        w.RenderEndTag(); // </p>

                        w.AddAttribute(HtmlTextWriterAttribute.Href, photos[i].Value);
                        w.RenderBeginTag(HtmlTextWriterTag.A); // <a>
                        w.Write(photos[i].CreateHTML());
                        w.RenderEndTag(); // </a>

                        //navigation
                        if (i < photos.Length - 1)
                        {
                            w.AddAttribute(HtmlTextWriterAttribute.Class, ForwardSpanClass);
                            w.RenderBeginTag(HtmlTextWriterTag.Span);
                                w.AddAttribute(HtmlTextWriterAttribute.Href, string.Format(ImageIDFormat, i + 1).Insert(0, @"#"));
                                w.RenderBeginTag(HtmlTextWriterTag.A);
                                    w.Write(ForwardButtonText);
                                w.RenderEndTag();
                            w.RenderEndTag();
                        }

                    w.RenderEndTag(); // </li>
                }
                w.RenderEndTag(); // </ul>
            
            w.RenderEndTag(); // </div>
        }

        protected override void RenderPreviewBody(HtmlTextWriter w)
        {
            // <div class=PostBodyClass>
            w.AddAttribute(HtmlTextWriterAttribute.Class, this.PostBodyClass);
            w.RenderBeginTag(HtmlTextWriterTag.Div);
                // <div class=AlbumThumbnailClass
                w.AddAttribute(HtmlTextWriterAttribute.Class, this.AlbumThumbnailClass);
                w.RenderBeginTag(HtmlTextWriterTag.Div);


                    if(GetImgTags().Count > 0)
                    {
                        w.WriteLine(GetImgTags().First());
                    }

                    // <div class="Overlay">
                    w.AddAttribute(HtmlTextWriterAttribute.Class, this.OverlayClass);
                    w.RenderBeginTag(HtmlTextWriterTag.Div);

                    // forcing the <a> tag to exist the way I need
                    w.WriteLine(string.Format(@"<a href=""{0}""><span>.</span></a>", this.TitlePath));

                    // </div class="overlay">
                    w.RenderEndTag();


                    // <h2 id=UniqueID>
                    w.AddAttribute(HtmlTextWriterAttribute.Id, this.UniqueID.ToString());
                    w.RenderBeginTag(HtmlTextWriterTag.H2);
                        w.Write(this.Title);
                    // </h2>
                    w.RenderEndTag();

                    using (StringReader reader = new StringReader(this.CleanedMainText))
                    {
                        string nextLine = string.Empty;
                        while((nextLine = reader.ReadLine()) != null)
                        {
                            // <p>
                            w.RenderBeginTag(HtmlTextWriterTag.P);
                            w.Write(nextLine);
                            // </p>
                            w.RenderEndTag();
                        }
                    }


                // </div class=AlbumThumbnailClass>
                w.RenderEndTag();
            // </div class=PostBodyClass>
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
        
        /// <summary>
        /// Returns the main text, with all img tags wrapped in a link to this post's main (single) file. 
        /// </summary>
        public string LinkifiedMainText
        {
            get
            {
                string parsedMainText = this.ParsedMainText;

                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                int pos = 0;

                int i = parsedMainText.IndexOf("<img");
                int j;
                while(i >= 0 && i >= pos)
                {
                    j = parsedMainText.IndexOf(">", i, parsedMainText.Length - i);
                    buffer.Append(parsedMainText.Substring(pos, i - pos));
                    buffer.Append(string.Format(@"<a href=""{0}"">", this.TitlePath));
                    buffer.Append(parsedMainText.Substring(i, j - i));
                    buffer.Append(@"</a>");

                    pos = j + 1;
                    i = pos + parsedMainText.Substring(pos).IndexOf("<img");
                }

                buffer.Append(parsedMainText.Substring(pos, parsedMainText.Length - pos));

                return buffer.ToString();
            }
        }
        
        /// <summary>
        /// Returns the main text with all img tags removed.
        /// </summary>
        private string CleanedMainText
        {
            get
            {
                string parsedMainText = this.ParsedMainText;

                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                int pos = 0;

                int i = parsedMainText.IndexOf("<img");
                int j;
                while(i >= 0 && i >= pos) // find an img tag
                {
                    j = parsedMainText.IndexOf(">", i, parsedMainText.Length - i);
                    
                    buffer.Append(parsedMainText.Substring(pos, i - pos));
                    pos = j + 1;

                    i = pos + parsedMainText.Substring(pos).IndexOf("<img");
                }

                buffer.Append(parsedMainText.Substring(pos, parsedMainText.Length - pos));

                return buffer.ToString();
            }
        }

        /// <summary>
        /// Searches this.ParsedMainText for any image tags and returns each as an element in an array.
        /// </summary>
        private List<string> GetImgTags()
        {
            string parsedMainText = this.ParsedMainText;
            List<string> imgTags = new List<string>();

            int pos = 0;
            int i = parsedMainText.IndexOf("<img");
            int j;
            while(i >= 0 && i >= pos)
            {
                j = parsedMainText.IndexOf(">", i, parsedMainText.Length - i);

                if (j >= 0)
                    imgTags.Add(parsedMainText.Substring(i, j - i));

                pos = j + 1;
                i = pos + parsedMainText.Substring(pos).IndexOf("<img");
            }

            return imgTags;
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