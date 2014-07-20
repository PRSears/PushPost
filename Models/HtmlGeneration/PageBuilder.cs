using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using Extender.Debugging;
using System.Threading.Tasks;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration;

namespace PushPost.Models.HtmlGeneration
{
    /// <remarks>
    /// Class containing functions for rendering and saving one or more
    /// HTML files (pages) - automatically deciding on where the posts
    /// should be split up.
    /// </remarks>
    public class PageBuilder
    {
        public Post[] Posts
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

        public List<Page> Pages
        {
            get;
            protected set;
        }

        public PageBuilder()
        {
            this.PostsPerPage   = 10;
            this.Hrefs          = new List<string>();

            Hrefs.Add("http://fonts.googleapis.com/css?family=Open+Sans:300");
            Hrefs.Add("../css/styles.css");
            //Hrefs.Add("https://google-code-prettify.googlecode.com/svn/loader/skins/sons-of-obsidian.css");
            //Hrefs.Add("../css/gallery.css");
        }
        #region constructor overloads
        public PageBuilder(Post[] posts)
            : this()
        {
            this.Posts = posts;
        }

        public PageBuilder(Post[] posts, List<string> hrefs)
            : this(posts)
        {
            this.Hrefs = hrefs;
        }

        public PageBuilder(List<string> hrefs):this()
        {
            this.Hrefs = hrefs;
        }

        public PageBuilder(Post[] posts, List<string> hrefs, int postsPerPage)
            : this(posts, hrefs)
        {
            this.PostsPerPage = postsPerPage;
        }

        public PageBuilder(Post[] posts, int postsPerPage)
            : this(posts)
        {
            this.PostsPerPage = postsPerPage;
        }
        #endregion

        /// <summary>
        /// Generates a single, or multiple, HTML files based on the Post objects
        /// in this.Posts. Stores generated Pages in this.Pages, then returns this.Pages.
        /// </summary>
        /// <returns>Newly generated Page list.</returns>
        public virtual List<Page> CreatePages()
        {
            Pages = new List<Page>();

            if (this.Posts.Length < 1)
                return Pages;

            // for each category of Post of the Post(s) in this.Posts
            foreach(var post_batch in this.Posts.GroupBy(p => p.Category))
            {
                Queue<Post> category_group = new Queue<Post>
                    (
                        post_batch.OrderByDescending(p => p.Timestamp)
                    );

                NavCategory peek = category_group.Peek().Category;

                Pages.AddRange(GeneratePages(category_group, peek));
            }

            return Pages;
        }

        protected virtual List<Page> GeneratePages(Queue<Post> posts, NavCategory category)
        {
            Int16 requiredPages = (Int16)Math.Ceiling((double)posts.Count() / (double)this.PostsPerPage);
            Page[] generatedPages = new Page[requiredPages];

            for (int pageI = 1; pageI <= requiredPages; pageI++)
            {
                List<Post> newPosts = new List<Post>();
                for (int spaceRemaining = this.PostsPerPage; (spaceRemaining > 0) && (posts.Count() > 0); spaceRemaining--)
                    newPosts.Add(posts.Dequeue());

                generatedPages[pageI - 1] = new Page
                    (
                        new Head(Page.GenerateTitle(category, pageI), this.Hrefs),
                        new Navigation(category),
                        new Breadcrumbs
                            (
                                MakeLinks(requiredPages, category), 
                                pageI
                            ),
                        newPosts
                    );
            }

            return new List<Page>(generatedPages);
        }
        
        protected List<string> MakeLinks(int numPages, NavCategory category)
        {
            string[] links = new string[numPages];
            for (int i = 1; i <= numPages; i++)
                links[i - 1] = Page.GenerateFilename(category, i);

            return new List<string>(links);
        }

        // TODO Sort files into subfolders based on category

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
                    string subfolderPath = Path.Combine(
                        outDirectoryPath,
                        page.Subfolder);

                    if(!Directory.Exists(outDirectoryPath))
                        Directory.CreateDirectory(outDirectoryPath);
                    if(!Directory.Exists(subfolderPath))
                        Directory.CreateDirectory(subfolderPath);

                    string nextFilename = Path.Combine(
                                outDirectoryPath,
                                page.FullName);

                    if (File.Exists(nextFilename))
                        File.Delete(nextFilename);

                    using (StreamWriter stream = File.CreateText(nextFilename))
                    {
                        StringReader buffer = new StringReader(page.Create());
                        while (buffer.Peek() != -1)
                            stream.WriteLine(buffer.ReadLine());
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteMessage("Failed to save pages to " + outDirectoryPath, "warn");
                ExceptionTools.WriteExceptionText(e, true);

                return false;
            }

            return true;
        }

        public static void TestHarness()
        {
            using (PushPost.Models.Database.Archive db = new PushPost.Models.Database.Archive(@"Post_TestDB_2014-29-04_003.mdf"))
            {
                Post[] posts = db.Dump();

                Debug.WriteMessage(posts.Count().ToString());

                string[] refs = { "css/styles.css", "css/gallery.css", "http://fonts.googleapis.com/css?family=Open+Sans:300" };

                PageBuilder build = new PageBuilder(posts, refs.ToList(), 10);

                build.CreatePages();
                build.SavePages(@"E:\code\GitHub\PushPost\bin\Debug\testhtml");
            }
        }
    }
}

// TODO Get optional css class/id names from a .cfg file.