using System.Collections.Generic;
using System.Web.UI;
using TidyManaged;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace PushPost.ClientSide.HtmlGenerators
{
    public class Page
    {        
        // TODO make a "Page" class that will hold the actual HTML (like my post class) --
        //      and use an interface for classes to generate sets of pages. A "PageGenerator" should
        //      be able to sort and group a set of posts and generate appropriate page(s') HTML.
        //
        // Perhaps instead of an IPageGenerator interface, there should just be a fully implemented 
        // PageGenerator class that takes a list of Post objects - and determines the correct category
        // from the Posts - then generates the appropriate list of pages for those posts.
        //
        // This has the problem that if I later want to add more categories the PageGenerator class 
        // would need to be modified, instead of simply creating a new implementation of the interface.
        //      - or just extend +/ override relevent members? Although all the PageGenerator would do
        //        is generate the pages with a Generate() function, so overriding that would essentially 
        //        be re-writing the entire object.
        //
        // PageGenerator should do a Ceiling(Posts.Count / 10) to get neccessary number of pages 
        // and generate lower breadcrumbs based on that.

        public int PageNumber
        {
            get;
            set;
        }

        public string DoctypeString
        {
            get;
            set;
        }

        public string FinalComment
        {
            get;
            set;
        }
        
        public Head Header
        {
            get;
            set;
        }

        public Navigation UpperNavigation
        {
            get;
            set;
        }

        public Breadcrumbs LowerNavigation
        {
            get;
            set;
        }

        public List<Post> Posts
        {
            get;
            set;
        }

        public string PrimaryColumnID
        {
            get;
            set;
        }

        public Page
            (
                string title, 
                string doctypeString = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">"
            )
        {
            DoctypeString   = doctypeString;
            Header          = new Head(title);
            UpperNavigation = new Navigation();
            LowerNavigation = new Breadcrumbs();
            Posts           = new List<Post>();
            PrimaryColumnID = "primary-column"; // TODO remove hard-coded string
            FinalComment    = "This page was generated automatically by PushPost.";
        }
        #region Constructor overloads
        public Page(Head header):this(header.Title)
        {
            this.Header = header;
        }

        public Page(string title, Navigation upperNavigation):this(title)
        {
            this.UpperNavigation = upperNavigation;
        }

        public Page(string title, Breadcrumbs lowerNavigation):this(title)
        {
            this.LowerNavigation = lowerNavigation;
        }

        public Page(string title, List<Post> posts):this(title)
        {
            this.Posts = posts;
        }

        public Page(Head header, Navigation upperNavigation, Breadcrumbs lowerNavigation):this(header)
        {
            this.UpperNavigation = upperNavigation;
            this.LowerNavigation = lowerNavigation;
        }

        public Page(Head header, Navigation upperNavigation, Breadcrumbs lowerNavigation, List<Post> posts)
            : this(header, upperNavigation, lowerNavigation)
        {
            this.Posts = posts;
        }

        #endregion

        public virtual string Create()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter w = new HtmlTextWriter(buffer))
            {
                w.WriteLine(DoctypeString);

                // <html>
                w.RenderBeginTag(HtmlTextWriterTag.Html);

                // <head>
                w.WriteLine(this.Header.Create());

                // <body>
                w.RenderBeginTag(HtmlTextWriterTag.Body);
                    w.WriteLine(this.UpperNavigation.Create());
                    w.AddAttribute(HtmlTextWriterAttribute.Id, this.PrimaryColumnID);
                    w.RenderBeginTag(HtmlTextWriterTag.Div);
                        foreach (Post post in this.Posts) w.WriteLine(post.Create());
                    w.RenderEndTag();
                    w.WriteLine(this.LowerNavigation.Create());
                w.RenderEndTag();
                // </body>

                w.RenderEndTag();
                // </html>

                w.WriteComment(FinalComment);

                using (Document formatter = Document.FromString(buffer.ToString()))
                {
                    formatter.ShowWarnings = false;
                    formatter.Quiet = true;
                    formatter.OutputHtml = true;
                    formatter.IndentSpaces = 4;
                    formatter.WrapAt = 116;
                    formatter.AddVerticalSpace = true;
                    formatter.IndentBlockElements = AutoBool.Yes;
                    formatter.CleanAndRepair();

                    return formatter.Save();
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Page))
                return false;

            Page b = (Page)obj;

            return
            (
                this.Header.Equals(b.Header)                   &&
                this.UpperNavigation.Equals(b.UpperNavigation) &&
                this.LowerNavigation.Equals(b.LowerNavigation) &&
                (this.Posts.Except(b.Posts).Count() == 0)
            );
        }

        public static string TestHarness()
        {
            List<string> hypertextReferences = new List<string>();
            hypertextReferences.Add(@"css/styles.css");
            hypertextReferences.Add(@"css/gallery.css");
            hypertextReferences.Add(@"http://fonts.googleapis.com/css?family=Open+Sans:300");

            List<string> links = new List<string>();
            links.Add("blog_p1");
            links.Add("blog_p2");
            links.Add("blog_p3");

            Head t1_Head = new Head("Test Page 01", hypertextReferences);
            Navigation upNav = new Navigation(PostTypes.NavCategory.Blog);
            Breadcrumbs loNav = new Breadcrumbs(links, 1, "(C) Patrick Sears 2014");

            List<Post> testPosts = new List<Post>();
            for (int i = 0; i < 11; i++)
                testPosts.Add(new PostTypes.TextPost(i.ToString("D2") + " Post", DateTime.Now.AddDays(i), "Patrick Sears", "Post #" + i.ToString("D2") + "\nBlog post!\nAnother paragraph!\n\nHoly shit!!+@(Test)"));

            Page tp = new Page(t1_Head, upNav, loNav, testPosts);

            return tp.Create();
        }
    }

    //TODO_ Perhaps the PageGenerationMethod class should be moved to the PageBuilder.cs file,
    //      or given a file of its own.
    public class PageGenerationMethod
    {
        private string Method;

        public static PageGenerationMethod DateDescending   { get { return new PageGenerationMethod("date_descending"); } }
        public static PageGenerationMethod DateAscending    { get { return new PageGenerationMethod("date_ascending"); } }
        public static PageGenerationMethod AuthorDescending { get { return new PageGenerationMethod("author_descending"); } }
        public static PageGenerationMethod AuthorAscending  { get { return new PageGenerationMethod("author_ascending"); } }
        public static PageGenerationMethod TitleDescending  { get { return new PageGenerationMethod("title_descending"); } }
        public static PageGenerationMethod TitleAscending   { get { return new PageGenerationMethod("title_ascending"); } }

        private static PageGenerationMethod[] AllMethods = { DateDescending, DateAscending, AuthorDescending, AuthorAscending, TitleDescending, TitleAscending };

        private PageGenerationMethod(string method)
        {
            this.Method = method;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PageGenerationMethod))
                return false;

            PageGenerationMethod b = (PageGenerationMethod)obj;

            return this.Method.Equals(b.Method);
        }

        public override int GetHashCode()
        {
            return this.Method.GetHashCode();
        }

        public static PageGenerationMethod Parse(string method)
        {
            foreach (PageGenerationMethod o in AllMethods)
                if (o == method)
                    return o;

            throw new ArgumentException("Method provided is not a valid PageGenerationMethod.");
        }

        #region Comparison operators
        public static bool operator ==(PageGenerationMethod a, PageGenerationMethod b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PageGenerationMethod a, PageGenerationMethod b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(PageGenerationMethod a, string b)
        {
            return a.Method.Equals(b);
        }

        public static bool operator !=(PageGenerationMethod a, string b)
        {
            return !a.Method.Equals(b);
        }

        public static bool operator ==(string a, PageGenerationMethod b)
        {
            return a.Equals(b.Method);
        }

        public static bool operator !=(string a, PageGenerationMethod b)
        {
            return !a.Equals(b.Method);
        }
        #endregion
    }
}
