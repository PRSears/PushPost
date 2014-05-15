using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI;

namespace PushPost.ClientSide.HtmlGenerators.PostTypes
{
    public partial class TextPost : Post
    {
        public TextPost():base()
        {
        }

        public TextPost(string title, DateTime timestamp, string author) : this()
        {
            Title = title;
            Timestamp = timestamp;
            Author = author;

            //Footers = new List<Embedded.Footer>();
            //Resources = new List<Embedded.IResource>();
            //Tags = new List<Embedded.Tag>();

            //HeaderClass = "post-title";
            //FooterClass = "footer";
            //PreviewLength = 250;
        }

        public TextPost(string title, DateTime timestamp, string author, string body) : this(title, timestamp, author)
        {
            MainText = body;
        }

        #region deprecated Create override
        /*
        public override string Create()
        {
            if (!this.MainText.Equals(string.Empty))
                this.ParseMainText();
        
            StringWriter buffer = new StringWriter();

            using(HtmlTextWriter w = new HtmlTextWriter(buffer))
            {
                // --- Header ---
                w.AddAttribute(HtmlTextWriterAttribute.Headers, HeaderClass);
                w.RenderBeginTag(HtmlTextWriterTag.H1);

                w.Write(this.Title);

                w.RenderEndTag();

                // --- Post body ---
                using(StringReader reader = new StringReader(this.MainText))
                {
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        w.Write(w.NewLine);

                        // Prepend line with '\' to include tag as written (embed as code)
                        if(line.StartsWith("<img"))
                        {
                            w.Write(line);
                            continue;
                        }

                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.Write(line);
                        w.RenderEndTag();
                    }
                }

                // --- Footers --- 
                foreach(string footer in this.Footers)
                {
                    using(StringReader reader = new StringReader(footer))
                    {
                        w.Write(w.NewLine);
                        w.AddAttribute(HtmlTextWriterAttribute.Class, "footer");
                        w.RenderBeginTag(HtmlTextWriterTag.P);

                        w.Write(footer);

                        w.RenderEndTag();
                    }
                }

                // --- Post End ---
                w.Write(w.NewLine + w.NewLine);
                w.WriteComment(" ********* End of post: " + Title + " ********* ");
                w.Write(w.NewLine + w.NewLine + w.NewLine + w.NewLine);
            }

            return buffer.ToString();
        }
         */
        #endregion

        protected override void RenderHeader(HtmlTextWriter w)
        {
            w.AddAttribute(HtmlTextWriterAttribute.Headers, base.HeaderClass);
            w.RenderBeginTag(HtmlTextWriterTag.H1);
            w.Write(this.Title);
            w.RenderEndTag();
            // TODO include author and timestamp in the header somewhere
        }

        protected override void RenderBody(HtmlTextWriter w)
        {
            // TODO FIX first letter of over posts' body getting ignored

            using(StringReader reader = new StringReader(this.ParsedMainText))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    // Prepend line with '\' to include tag as written (embed as code)
                    if (line.StartsWith("<img")) // TODO make sure other tags (links, etc) aren't given new paragraphs as well
                    {
                        w.Write(line);
                        continue;
                    }

                    w.RenderBeginTag(HtmlTextWriterTag.P);
                    w.Write(line);
                    w.RenderEndTag();

                    w.Write(w.NewLine);
                }
            }
        }

        protected override void RenderFooter(HtmlTextWriter w)
        {
            foreach(string footer in this.ParsedFooters)
            {
                w.WriteLine(string.Empty);
                w.WriteLine(footer);
            }
        }

        protected override void RenderComments(HtmlTextWriter w)
        {
            w.WriteComment(" ********* End of post: " + Title + " ********* ");
        }

        public static string TestHarness()
        {
            string header = "TestPost Alpha";

            string body   = "This is a test of automatic text post generation.\n" + 
                            "This should be in a new paragraph.\n" + 
                            "This third paragraph should include markup for Test.jpg. +@(Test)";

            //string footer = "Post footer would normally contain download links to things mentioned in the post, or perhaps the +@(Link_Date).";

            TextPost testTextPost = new TextPost(header, DateTime.Now, "Patrick Sears", body);
            //testTextPost.Footers.Add(footer);
            
            testTextPost.Resources.Add(new Embedded.Image("Test", @"C:/Not/A/Real/Path/Test.jpg"));
            testTextPost.Resources.Add(new Embedded.Link("Link_Date", "https://www.google.ca/search?q=what+time+is+it", "date"));
            
            return testTextPost.Create() + "\n\n\n" +
                "Preview:\n" + testTextPost.CreatePreview();
        }
    }
}
