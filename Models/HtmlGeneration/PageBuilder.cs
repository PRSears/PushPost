using Extender.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public int TextPostsPerPage
        {
            get;
            set;
        }

        public int PhotoPostsPerPage
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

        public Page[] SinglePostPages
        {
            get;
            protected set;
        }

        public PageBuilder()
        {
            if (Properties.Settings.Default.PostsPerPage > 0)
                this.TextPostsPerPage = Properties.Settings.Default.PostsPerPage;
            else
                this.TextPostsPerPage = 10; // fallback

            if (Properties.Settings.Default.PhotoPostsPerPage > 0)
                this.PhotoPostsPerPage = Properties.Settings.Default.PhotoPostsPerPage;
            else
                this.TextPostsPerPage = 10; // fallback

            this.Hrefs            = new List<string>();

            // TODO Load HRefs from config file
            Hrefs.Add("http://fonts.googleapis.com/css?family=Open+Sans:400,300,700,800,300italic,400italic,600,600italic,800italic");
            Hrefs.Add("http://fonts.googleapis.com/css?family=Droid+Sans+Mono");
            Hrefs.Add("../css/styles.css");
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
            this.TextPostsPerPage = postsPerPage;
        }

        public PageBuilder(Post[] posts, int postsPerPage)
            : this(posts)
        {
            this.TextPostsPerPage = postsPerPage;
        }
        #endregion

        /// <summary>
        /// Generates a single, or multiple, HTML files based on the Post objects
        /// in this.Posts. Stores generated Pages in this.Pages, then returns this.Pages.
        /// </summary>
        /// <returns>Newly generated Page list.</returns>
        public virtual void CreatePages()
        {
            if (this.Posts.Length < 1)
                return;

            this.Pages = new List<Page>();
            this.SinglePostPages = GenerateSingles();

            // for each category of Post of the Post(s) in this.Posts
            foreach(var post_batch in this.Posts.GroupBy(p => p.Category))
            {
                Queue<Post> category_group = new Queue<Post>
                    (
                        post_batch.OrderByDescending(p => p.Timestamp)
                    );

                NavCategory peek = category_group.Peek().Category;

                this.Pages.AddRange(GeneratePages(category_group, peek));
            }

        }

        protected virtual Page[] GenerateSingles()
        {
            Page[] singles = new Page[this.Posts.Length];

            for(int i = 0; i < this.Posts.Length; i++)
            {
                singles[i] = new Page
                    (
                        this.Posts[i],
                        new Navigation(Posts[i].Category),
                        new Breadcrumbs()
                    );
                singles[i].Title = singles[i].GenerateTitle();
                singles[i].Hrefs = this.Hrefs;

                if (this.Posts[i] is PhotoPost)
                    singles[i].IncludePrimaryColumn = false; // Don't wrap photo albums in a column
            }

            return singles;
        }

        protected virtual Page[] GeneratePages(Queue<Post> posts, NavCategory category)
        {
            //
            // THOUGHT I could have a PostsPerPage property in the Post object itself, that way any class 
            //         inheriting from it can overwrite as necessary. 
            //
            //         Could just do posts.Peek().PostsPerPage; 
            

            // pick how many posts to use on one page based on the first post in the queue
            int postsPerPage = posts.Peek() is TextPost ? this.TextPostsPerPage : this.PhotoPostsPerPage;

            Int16 requiredPages = (Int16)Math.Ceiling((double)posts.Count() / (double)postsPerPage);
            Page[] generatedPages = new Page[requiredPages];
            
            for (int pageI = 1; pageI <= requiredPages; pageI++)
            {
                List<Post> newPosts = new List<Post>();
                for (int spaceRemaining = postsPerPage; (spaceRemaining > 0) && (posts.Count() > 0); spaceRemaining--)
                    newPosts.Add(posts.Dequeue());

                generatedPages[pageI - 1] = new Page
                (
                    newPosts,
                    new Navigation(category),
                    new Breadcrumbs
                    (
                        MakeLinks(requiredPages, category),
                        pageI
                    )
                );

                generatedPages[pageI - 1].Title = generatedPages[pageI - 1].GenerateTitle();
                generatedPages[pageI - 1].Hrefs = this.Hrefs;
            }

            return generatedPages;
        }
        
        protected List<string> MakeLinks(int numPages, NavCategory category)
        {
            // Don't need navigation links if there's only one page
            if (numPages <= 1)
                return new List<string>(); 

            string[] links = new string[numPages];
            for (int i = 1; i <= numPages; i++)
                links[i - 1] = Page.GenerateFilename(category, i);

            return new List<string>(links);
        }
        
        /// <summary>
        /// Saves the most recently generated Pages (generated when this.CreatePages() is called)
        /// out to an HTML file.
        /// </summary>
        /// <param name="outDirectoryPath">Path to the directory where the pages are to be saved.</param>
        /// <returns>True when saving succeeds.</returns>
        public bool SavePages(string outDirectoryPath)
        {
            if (this.Pages == null || this.SinglePostPages == null)
                CreatePages();

            Page[] allPages = new Page[this.Pages.Count + this.SinglePostPages.Length];

            Array.Copy(this.Pages.ToArray(), 0, allPages, 0, this.Pages.Count);
            Array.Copy(this.SinglePostPages, 0, allPages, this.Pages.Count, this.SinglePostPages.Length);

            try
            {
                foreach (Page page in allPages)
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
                Debug.WriteMessage("Failed to save pages to " + outDirectoryPath, "error");
                ExceptionTools.WriteExceptionText(e, true);

                var result = System.Windows.Forms.MessageBox.Show
                (
                    string.Format
                    (
                        "PushPost's PageBuilder encountered an exception.\n" + 
                        "{0}\n{1}\n\n---\n" + 
                        "Choosing abort will cause the program to terminate. " + 
                        "Retry and ignore will both leave the program " + 
                        "functional, but the SavePages command will not continue.", 
                        e.Message, 
                        e.StackTrace
                    ),
                    "Exception",
                    System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore
                );

                if (result.Equals(System.Windows.Forms.DialogResult.Abort))
                    throw e;

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