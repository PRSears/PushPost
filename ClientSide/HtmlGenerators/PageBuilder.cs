using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using Extender.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using PushPost.ClientSide.HtmlGenerators.PostTypes;

namespace PushPost.ClientSide.HtmlGenerators
{
    /// <remarks>
    /// Class containing functions for rendering and saving one or more
    /// HTML files (pages) - automatically deciding on where the posts
    /// should be split up.
    /// </remarks>
    public class PageBuilder
    {
        public List<Post> Posts
        {
            get;
            set;
        }

        public int PostsPerPage
        {
            get;
            set;
        }

        /// <summary>
        /// List of hypertext references to include in the Page's header.
        /// Any stylesheets, fonts, or other resources should be added to this list.
        /// 
        /// Each string in this list need only include the actual ref.
        /// <example>If you want the HTML to render as <code><link href='stylesheet.css' rel='stylesheet' type='text/css' /></code>
        /// you should only include stylesheet.css</example>.
        /// </summary>
        public List<string> Hrefs
        {
            get;
            set;
        }

        protected List<Page> Pages;

        public PageBuilder()
        {
            this.PostsPerPage   = 10;
            this.Hrefs          = new List<string>();
            this.Posts          = new List<Post>();
        }
        #region constructor overloads
        public PageBuilder(List<Post> posts)
            : this()
        {
            this.Posts = posts;
        }

        public PageBuilder(List<Post> posts, List<string> hrefs)
            : this(posts)
        {
            this.Hrefs = hrefs;
        }

        public PageBuilder(List<string> hrefs)
            : this(new List<Post>(), hrefs)
        { }

        public PageBuilder(List<Post> posts, List<string> hrefs, int postsPerPage)
            :this(posts, hrefs)
        {
            this.PostsPerPage = postsPerPage;
        }
        #endregion

        /// <summary>
        /// Generates a single, or multiple, HTML files based on the Post objects
        /// in this.Posts.
        /// </summary>
        /// <returns>The newly generated Page list.</returns>
        public virtual List<Page> CreatePages()
        {
            Pages = new List<Page>();

            if (this.Posts.Count < 1)
                return Pages;

            // for each category of Post of the Post(s) in this.Posts
            foreach(var post_batch in this.Posts.GroupBy(p => p.Category))
            {
                Queue<Post> category_group = new Queue<Post>
                    (
                        post_batch.OrderByDescending(p => p.Timestamp)
                    );

                NavCategory peek = category_group.Peek().Category;

                if      (peek == NavCategory.Photography)
                    Pages.AddRange(GeneratePhotographyPages(category_group));
                else if (peek == NavCategory.Code)
                    Pages.AddRange(GenerateCodePages(category_group));
                else if (peek == NavCategory.Blog)
                    Pages.AddRange(GenerateBlogPages(category_group));
                else
                    Pages.AddRange(GenerateGenericPages(category_group));
            }

            return Pages;
        }

        protected virtual List<Page> GeneratePhotographyPages(Queue<Post> posts)
        {
            throw new NotImplementedException();
        }

        protected virtual List<Page> GenerateCodePages(Queue<Post> posts)
        {

            throw new NotImplementedException();
        }

        protected virtual List<Page> GenerateBlogPages(Queue<Post> posts)
        {
            Int16 requiredPages = (Int16)Math.Ceiling((double)(posts.Count() / this.PostsPerPage));
            List<Page> generatedPages = new List<Page>();

            for (int pageI = 0; pageI < requiredPages; pageI++)
            {
                List<Post> p = new List<Post>();
                while(p.Count < this.PostsPerPage)
                {
                    p.Add(posts.Dequeue());
                }

                generatedPages.Add(
                    new Page
                    (
                        new Head("Blog (p" + pageI + ")", this.Hrefs),
                        new Navigation(NavCategory.Blog),
                        new Breadcrumbs(null, pageI), // TODO replace null with the neccessary links
                        p
                    ));
            }

            return generatedPages;
        }

        // THOUGHT maybe page generation should be something like GeneratePages(NavCategory cat) { ... }
        //         More code could be reused

        /// <summary>
        /// Generates the appropriate html files for the given posts. 
        /// Override this method to add support for more categories.
        /// </summary>
        /// <returns>List<Page> generated pages.</returns>
        protected virtual List<Page> GenerateGenericPages(Queue<Post> posts)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the most recently generated Pages (generated when this.CreatePages() is called)
        /// out to an HTML file.
        /// </summary>
        /// <param name="outDirectoryPath">Path to the directory where the pages are to be saved.</param>
        /// <returns>True when saving succeeds.</returns>
        public bool SavePages(string outDirectoryPath)
        {
            if (this.Pages == null)
                CreatePages();

            try
            {
                foreach (Page page in this.Pages)
                {
                    using (StreamWriter stream = File.CreateText
                        (
                            Path.Combine
                            (
                                outDirectoryPath,
                                String.Format("{1}_{0}.html", page.PageNumber.ToString("D4"), page.Header.Title)
                            )
                        ))
                    {
                        StringReader buffer = new StringReader(page.Create());
                        while (buffer.Peek() != -1)
                            stream.WriteLine(buffer.ReadLine());
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to save pages to " + outDirectoryPath);
                Console.WriteLine(Extender.Exceptions.Debug.CreateExceptionText(e, true));

                return false;
            }

            return true;
        }
    }
}
