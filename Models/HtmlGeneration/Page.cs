using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using TidyManaged;

namespace PushPost.Models.HtmlGeneration
{
    /// <remarks>
    /// Class to store data relating to a single page for the website. 
    /// Contains methods to generate a page's HTML, and static methods to 
    /// determine page titles and filenames.
    /// </remarks>
    public class Page
    {
        public int PageNumber
        {
            get
            {
                return this.LowerNavigation.CurrentPageIndex;
            }
            set
            {
                this.LowerNavigation.CurrentPageIndex = value;
            }
        }

        public bool IsSingle
        {
            get;
            set;
        }

        public string Title
        {
            get
            {
                return this.Header.Title;
            }
            set
            {
                this.Header.Title = value;
            }
        }

        public List<string> Hrefs
        {
            get
            {
                return this.Header.HypertextReferences;
            }
            set
            {
                this.Header.HypertextReferences = value;
            }
        }

        public string FileName
        {
            get
            {
                if (IsSingle)
                {
                    return Page.GenerateFilename(this.Posts[0].UniqueID.ToString(), 0);
                }
                else
                {
                    return Page.GenerateFilename(this.PageCategory, this.PageNumber);
                }
            }
        }

        /// <summary>
        /// Name of this Page's file, including the category subfolder.
        /// </summary>
        public string FullName
        {
            get
            {
                return Path.Combine(Subfolder, FileName);
            }
        }

        /// <summary>
        /// Subfolder (by PageCategory) this Page is to be stored in.
        /// </summary>
        public string Subfolder
        {
            get
            {
                if (IsSingle)
                {
                    return Properties.Settings.Default.SinglesSubfolder;
                }
                else
                {
                    return PageCategory.ToString();
                }
            }
        }

        public string SiteName
        {
            get;
            set;
        }

        /// <summary>
        /// Builds a Page's filename.
        /// </summary>
        /// <param name="category">The category the Page resides in.</param>
        /// <param name="pageNumber">The page number of the Page.</param>
        /// <returns>Returns a filename of the format: <example>blog_0001.html</example></returns>
        public static string GenerateFilename(NavCategory category, int pageNumber)
        {
            return GenerateFilename(category.ToString(), pageNumber);
        }

        /// <summary>
        /// Builds a Page's filename.
        /// </summary>
        /// <param name="category">The category the Page resides in.</param>
        /// <param name="pageNumber">The page number of the Page. Specify 0 page number
        /// to get the filename for an individual posts' page.</param>
        /// <returns>Returns a filename of the format: <example>blog_0001.html</example></returns>
        public static string GenerateFilename(string category, int pageNumber)
        {
            if(pageNumber == 0) // single post in the page
            {
                return string.Format("{0}.html", category);
            }
            else
            {
                return string.Format("{0}_p{1}.html", category, pageNumber.ToString("D4"));
            }
        }
        
        /// <summary>
        /// Builds a title for a Page.
        /// </summary>
        /// <returns>Return a title of the format: <example>Blog (page 1)</example></returns>
        public string GenerateTitle()
        {
            if(IsSingle)
            {
                return string.Format("{0} - {1}", 
                    this.SiteName, 
                    this.Posts[0].Title);
            }
            else
            {
                return string.Format("{0} - {1}{2}", 
                    this.SiteName, 
                    this.PageCategory, 
                    this.PageNumber.ToString("D2"));
            }
        }

        public NavCategory PageCategory
        {
            get
            {
                return this.UpperNavigation.CurrentCategory;
            }
            set
            {
                this.UpperNavigation.CurrentCategory = value;
            }
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

        /// <summary>
        /// Html div ID to apply to the primary column of this page. (Where all page content
        /// will appear).
        /// </summary>
        public string PrimaryColumnID
        {
            get;
            set;
        }

        public string WrapperID
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
            PrimaryColumnID = "primary-column";
            WrapperID       = "wrapper";
            FinalComment    = "This page was generated automatically by PushPost.";
            SiteName = Properties.Settings.Default.WesbiteName;
        }

        public Page(List<Post> posts, Navigation upperNavigation, Breadcrumbs lowerNavigation) : this(string.Empty)
        {
            this.Posts              = posts;
            this.UpperNavigation    = upperNavigation;
            this.LowerNavigation    = lowerNavigation;

            //this.IsSingle = Posts.Count > 1 ? false : true;
        }

        public Page(Post post, Navigation upperNavigation, Breadcrumbs lowerNavigation)
            : this(new List<Post> { post }, upperNavigation, lowerNavigation)
        {
            this.IsSingle = true;
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


        /// <summary>
        /// Uses HtmlTextWriter to render this page with the content supplied by the it's properties.
        /// </summary>
        /// <returns>String containing all of this page's HTML.</returns>
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
                    
                    // <wrapper>
                    w.AddAttribute(HtmlTextWriterAttribute.Id, WrapperID);
                    w.RenderBeginTag(HtmlTextWriterTag.Div);

                        w.WriteLine(this.UpperNavigation.Create());
                        w.AddAttribute(HtmlTextWriterAttribute.Id, this.PrimaryColumnID);
                        w.RenderBeginTag(HtmlTextWriterTag.Div);
                            foreach (Post post in this.Posts)
                            {
                                if (!this.IsSingle)
                                    w.WriteLine(post.CreatePreview());
                                else
                                    w.WriteLine(post.Create());
                            }
                        w.RenderEndTag();

                    w.RenderEndTag();
                    // </Wrapper>

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
                    formatter.IndentBlockElements = AutoBool.Auto;
                    formatter.CleanAndRepair();

                    return RemoveEmptyLines(
                        formatter.Save().Replace("<br>", System.Environment.NewLine));
                }
            }
        }

        protected string RemoveEmptyLines(string page)
        {
            using(StringReader reader = new StringReader(page))
            {
                StringBuilder buffer = new StringBuilder();
                string next;
                while(reader.Peek() != -1)
                {
                    next = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(next))
                        buffer.AppendLine(next);
                }

                return buffer.ToString();
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
            Navigation upNav = new Navigation(NavCategory.Blog);
            Breadcrumbs loNav = new Breadcrumbs(links, 1, "(C) Patrick Sears 2014");

            List<Post> testPosts = new List<Post>();
            for (int i = 0; i < 11; i++)
                testPosts.Add(new TextPost(i.ToString("D2") + " Post", DateTime.Now.AddDays(i), "Patrick Sears", "Post #" + i.ToString("D2") + "\nBlog post!\nAnother paragraph!\n\nHoly shit!!+@(Test)"));

            Page tp = new Page(t1_Head, upNav, loNav, testPosts);

            //return tp.Create();
            return "Test harness currently broken.";
        }
    }

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
