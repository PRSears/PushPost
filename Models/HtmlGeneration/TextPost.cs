using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI;

namespace PushPost.Models.HtmlGeneration
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

        protected override void RenderHeader(HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Id, base.HeaderClass);
            w.RenderBeginTag(HtmlTextWriterTag.H1);
            w.Write(this.Title);
            w.RenderEndTag();

            w.AddAttribute(HtmlTextWriterAttribute.Id, base.SubHeaderID);
            w.RenderBeginTag(HtmlTextWriterTag.Div);
                w.Write("by ");
                w.AddAttribute(HtmlTextWriterAttribute.Class, base.AuthorClass);
                w.RenderBeginTag(HtmlTextWriterTag.Span);
                    w.Write(this.Author);
                w.RenderEndTag();
                w.Write(" on ");
                w.AddAttribute(HtmlTextWriterAttribute.Class, base.DateClass);
                w.RenderBeginTag(HtmlTextWriterTag.Span);
                    w.Write(this.Timestamp.ToString(@"MMMM dd, yyyy"));
                w.RenderEndTag();
                w.Write(" at ");
                w.AddAttribute(HtmlTextWriterAttribute.Class, base.DateClass);
                w.RenderBeginTag(HtmlTextWriterTag.Span);
                    w.Write(this.Timestamp.ToString(@"HH:mm"));
                w.RenderEndTag();
            w.RenderEndTag();
        }

        protected override void RenderBody(HtmlTextWriter w)
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

        public static Post TemplatePost()
        {
            TextPost newPost = new TextPost("Enter Title", DateTime.Now, "Enter Author", "Enter body text.");
            newPost.Category = NavCategory.Blog;

            return newPost;
        }

        

        #region Debugging
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
            System.Threading.Thread.Sleep(50);
            Random r = new Random();
            int pad = r.Next(1000, 99999);
            DateTime ctime = DateTime.Now;

            Post dumb = new TextPost();

            dumb.Author = "Patrick Sears";
            dumb.Category = NavCategory.Blog;
            dumb.Title = "Dummy Post";
            dumb.Timestamp = ctime.AddSeconds(pad);
            dumb.MainText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut ornare, mauris eu consequat placerat, augue lectus tempus tellus, ut posuere turpis nunc auctor mi. Morbi vitae dapibus dui. In eget metus pellentesque, venenatis ipsum vitae, pellentesque nulla. Integer in tellus id quam luctus vehicula id vel tortor. Nam condimentum posuere tellus sed eleifend. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Morbi dignissim placerat diam quis laoreet. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam vitae dignissim tortor. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Morbi semper eu orci a fermentum. Proin dignissim quam vitae tempor consectetur. Mauris sodales eros sit amet eros laoreet, ac tempor odio molestie. Etiam rutrum nunc quis nunc varius iaculis."+
                            "\nAliquam orci diam, aliquam faucibus eleifend vitae, lacinia ac tortor. Donec non venenatis arcu, in gravida lacus. In eu sem id massa egestas tristique eget vel ipsum. Sed sit amet cursus eros, fermentum semper diam. Pellentesque vulputate congue eros nec ornare. Mauris facilisis felis elementum, placerat leo ultricies, posuere dui. Mauris arcu ante, sodales non nulla ac, viverra tincidunt dolor. Nulla semper vestibulum urna non mollis. Integer scelerisque tellus vel mauris scelerisque, nec posuere lorem tincidunt. Integer euismod nisl risus, non feugiat velit elementum dapibus. Vivamus sit amet dictum est, ac ultricies ante."+
                            "\nInteger tincidunt sem mi. Ut sodales augue feugiat lobortis imperdiet. Aliquam erat volutpat. Fusce volutpat elit sed eros eleifend, vel hendrerit mauris egestas. Nam accumsan nulla eget diam congue fermentum. Integer auctor dui a magna dictum, a semper nisi aliquet. Duis sit amet mollis libero. Aenean vitae metus magna. ";
            dumb.Footers = new List<Embedded.Footer>();
            dumb.Footers.Add(new Embedded.Footer("ft1", "footnote +@(link_nextPage)", dumb.UniqueID));

            return dumb;
        }
        #endregion
    }
}
