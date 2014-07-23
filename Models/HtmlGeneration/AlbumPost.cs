using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.Models.HtmlGeneration
{
    public class AlbumPost : Post
    {
        protected string AlbumClass;

        // TODO_ Inherit from TextPost? Would reduce code duplication in headers and footers

        public AlbumPost(string title, string author)
        {
            Title = title;
            Author = author;

            HeaderClass = "album-name";
            FooterClass = "footer";
            AlbumClass  = "photo-container";
        }

        public AlbumPost(string title, string author, string body) : this(title, author)
        {
            MainText = body;
        }

        public AlbumPost()
        {
            //throw new NotImplementedException();
        }

        public void AddImage(string name, string url)
        {
            // TODO_ generate markup for a new image added to the album
        }

        protected override void RenderHeader(System.Web.UI.HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Headers, base.HeaderClass);
            w.RenderBeginTag(HtmlTextWriterTag.H1);
            w.Write(this.Title);
            w.RenderEndTag();
            // TODO_ include author in the header somewhere
        }

        protected override void RenderBody(System.Web.UI.HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Class, AlbumClass);
            w.RenderBeginTag(HtmlTextWriterTag.Div);
            w.RenderBeginTag(HtmlTextWriterTag.Ul);

            using(StringReader reader = new StringReader(MainText))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    IResource nextResource = ResourceManager.GetResourceByName(ResourceManager.FirstResourceName(line), Resources);

                    w.RenderBeginTag(HtmlTextWriterTag.Li);
                    w.RenderBeginTag(HtmlTextWriterTag.P);

                    w.Write(ResourceManager.RemoveReferences(line));
                    w.RenderEndTag();

                    if(nextResource is InlineImage)
                        w.Write(nextResource.CreateHTML());

                    w.RenderEndTag();

                    w.WriteLine(string.Empty);
                }
            }

            w.RenderEndTag();
            w.RenderEndTag();
        }

        protected override void RenderFooter(System.Web.UI.HtmlTextWriter w)
        {
            // no footers needed at the moment
        }

        protected override void RenderComments(System.Web.UI.HtmlTextWriter w)
        {
            w.WriteComment(" ********** End of album: " + Title + " **********");
        }

        public override string CreatePreview()
        {
            // TODO_ make a collage or something to represent the album before it's opened
            throw new NotImplementedException();
        }

        public static Post TemplatePost()
        {
            return new AlbumPost("Enter Title", "Enter Author", "Enter album description.");
        }

        public static string TestHarness()
        {
            AlbumPost testAlbum = new AlbumPost("test album", "patrick");
            testAlbum.MainText = "picture description #1 +@(img1)\n" + 
                                 "picture description #2 +@(img2)\n";

            testAlbum.Resources.Add(new InlineImage("img1", @"c:\img1.jpg"));
            testAlbum.Resources.Add(new InlineImage("img2", @"c:\img2.jpg"));

            return testAlbum.Create();
        }
    }
}
