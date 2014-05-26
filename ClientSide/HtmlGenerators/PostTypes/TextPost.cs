using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI;

namespace PushPost.ClientSide.HtmlGenerators.PostTypes
{
    /// <remarks>
    /// Concrete implementation of Post containing methods to generate HTML
    /// for a single text post.
    /// </remarks>
    public partial class TextPost : Post
    {
        public TextPost()
            : base()
        {
        }

        public TextPost(string title, DateTime timestamp, string author)
            : this()
        {
            Title = title;
            Timestamp = timestamp;
            Author = author;
        }

        public TextPost(string title, DateTime timestamp, string author, string body)
            : this(title, timestamp, author)
        {
            MainText = body;
        }

        // TODO Move all hard-coded class strings into properties of the Post class
        protected override void RenderHeader(HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Id, base.HeaderClass);
            w.RenderBeginTag(HtmlTextWriterTag.H1);
            w.Write(this.Title);
            w.RenderEndTag();

            w.AddAttribute(HtmlTextWriterAttribute.Id, "sub-header");
            w.RenderBeginTag(HtmlTextWriterTag.Div);
                w.Write("by ");
                w.AddAttribute(HtmlTextWriterAttribute.Class, "author");
                w.RenderBeginTag(HtmlTextWriterTag.Span);
                    w.Write(this.Author);
                w.RenderEndTag();
                w.Write(" on ");
                w.AddAttribute(HtmlTextWriterAttribute.Class, "date");
                w.RenderBeginTag(HtmlTextWriterTag.Span);
                    w.Write(this.Timestamp);
                w.RenderEndTag();
            w.RenderEndTag();
        }

        protected override void RenderBody(HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Id, "post-body");
            w.RenderBeginTag(HtmlTextWriterTag.Div);
            using (StringReader reader = new StringReader(this.ParsedMainText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    w.RenderBeginTag(HtmlTextWriterTag.P);
                    w.Write(line);
                    w.RenderEndTag();
                }
            }
            w.RenderEndTag();
        }

        protected override void RenderFooter(HtmlTextWriter w)
        {
            foreach (string footer in this.ParsedFooters)
            {
                w.WriteLine(string.Empty);
                w.WriteLine(footer);
            }
        }

        protected override void RenderComments(HtmlTextWriter w)
        {
            if(this.IncludePostEndComments)
                w.WriteComment(" ********* End of post: " + Title + " ********* ");
        }

        public static string TestHarness()
        {
            string header = "TestPost Alpha";

            string body = "This is a test of automatic text post generation.\n" +
                            "This should be in a new paragraph.\n" +
                            "This third paragraph should include markup for Test.jpg. +@(Test)";

            //string footer = "Post footer would normally contain download links to things mentioned in the post, or perhaps the +@(Link_Date).";

            TextPost testTextPost = new TextPost(header, DateTime.Now, "Patrick Sears", body);
            //testTextPost.Footers.Add(footer);

            testTextPost.Resources.Add(new Embedded.InlineImage("Test", @"C:/Not/A/Real/Path/Test.jpg"));
            testTextPost.Resources.Add(new Embedded.Link("Link_Date", "https://www.google.ca/search?q=what+time+is+it", "date"));

            return testTextPost.Create() + "\n\n\n" +
                "Preview:\n" + testTextPost.CreatePreview();
        }

        /// <summary>
        /// Returns a preset fake post for testing purposes.
        /// </summary>
        public static Post Dummy()
        {
            Post dumb = new TextPost();

            dumb.Author = "Patrick Sears";
            dumb.Category = NavCategory.Blog;
            dumb.Title = "Dummy Post";
            dumb.MainText = "";
            dumb.Footers = new List<Embedded.Footer>();
            dumb.Footers.Add(new Embedded.Footer("ft1", "footnote +@(link_nextPage)", dumb.UniqueID));

            return dumb;
        }
    }
}
