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
    public class PageBuilder
    {
        // Contain properties pertaining to the generation of upper/lower navs, headers
        // and any other data needed to render a page.

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

        public List<string> Hrefs
        {
            get;
            set;
        }

        protected List<Page> Pages;

        public PageBuilder()
        {
            this.Posts = new List<Post>();
            this.PostsPerPage = 10;
            this.Hrefs = new List<string>();
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

            throw new NotImplementedException();
        }

        protected virtual List<Page> GenerateGenericPages(Queue<Post> posts)
        {

            throw new NotImplementedException();
        }

        [Obsolete]
        protected int CategoryInList(List<Queue<Post>> postLists, NavCategory category)
        {
            for(int i = 0; i < postLists.Count; i++)
            {
                if (postLists[i].Peek().Category.Equals(category))
                    return i;
            }

            return -1;
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
                Console.WriteLine(Extender.Exceptions.Debug.Create(e, true));

                return false;
            }

            return true;
        }
    }
}
