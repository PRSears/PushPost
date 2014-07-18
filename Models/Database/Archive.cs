using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Linq;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using Extender.Date;
using Extender.Exceptions;

namespace PushPost.Models.Database
{
    // TODO load connection string from a config file

    /// <remarks>
    /// This class handles the storage and retrieval of posts from the database archives.
    /// It's essentially just a wrapper for the DataContext to provide some sanity checks,
    /// and cut down on duplicated code.
    /// </remarks>
    public class Archive : IDisposable
    {
        private bool _disposed = false;

        protected PostsDataContext db;
        protected string RelativeFilename
        {
            get;
            set;
        }
        protected string _PostDBConnectionString;

        public string PostDBConnectionString
        {
            get
            {
                return this._PostDBConnectionString;
            }
            set
            {
                if (value.Contains(@"|DataDirectory|"))
                    this._PostDBConnectionString = value.Replace(@"|DataDirectory|", Directory.GetCurrentDirectory());
                else
                    this._PostDBConnectionString = value;
            }
        }
        public Table<PostTableLayer> PostTable
        {
            get
            {
                if (db == null)
                    return null;

                return db.Posts;
            }
        }
        public Table<Footer> FootnotesTable
        {
            get
            {
                if (db == null)
                    return null;

                return db.Footnotes;
            }
        }
        public Table<Tag> TagsTable
        {
            get
            {
                if (db == null)
                    return null;

                return db.Tags;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Archive class for the database at the provided (relative) path.
        /// </summary>
        /// <param name="databaseRelativePath">Relative path (including filename and extension) 
        /// to the database Archive will handle.</param>
        public Archive(string databaseRelativePath)
        {
            this.RelativeFilename = databaseRelativePath;

            this._PostDBConnectionString = Archive.GenerateConnectionString(this.RelativeFilename);
            db = new PostsDataContext(this.PostDBConnectionString);

            if (!db.DatabaseExists())
                db.CreateDatabase();
        }

        /// <summary>
        /// Initializes a new instance of the Archive using the default database filename 
        /// loaded from the exe.config file.
        /// </summary>
        public Archive() : this(Properties.Settings.Default.DBRelativeFilename) { }

        /// <summary>
        /// Creates a connection string for connecting to a database inside the current
        /// working directory.
        /// </summary>
        /// <param name="relativeFilename">Relative path (including filename and extension) 
        /// to the database you're generating the connection string for.</param>
        /// <returns></returns>
        public static string GenerateConnectionString(string relativeFilename)
        {
            return string.Format(
                "Data Source=(LocalDB)\\v11.0;AttachDbFilename={0}\\{1};Integrated Security=False;Pooling=false;",
                Directory.GetCurrentDirectory(),
                relativeFilename);
        }

        /// <summary>
        /// Executes pending operations on the database.
        /// </summary>
        public void SubmitChanges()
        {
            // TODO surround in trycatch and display a warning popup on errors
            db.SubmitChanges();
        }

        /// <summary>
        /// Checks to see if newPost is a duplicate, and if not, adds it to the
        /// pending inserts. Call SubmitChanges() to push the pending changes to the 
        /// database.
        /// </summary>
        /// <param name="newPost">New Post object to add to the database.</param>
        public void CommitPost(Post newPost)
        {
            CommitPost(PostTableLayer.FromPost(newPost), this.db);
        }

        protected void CommitPost(PostTableLayer newPost, PostsDataContext database)
        {
            // Check to see if it already exists in the database.
            if (database.Posts.Where(p => p.UniqueID == newPost.UniqueID).Count() > 0)
                return;

            // Insert newPost into Posts table
            database.Posts.InsertOnSubmit(newPost);
            // Insert newPost's footers into the Footnotes table
            foreach (Footer f in newPost.Footers)
                database.Footnotes.InsertOnSubmit(f);
            // Insert newPost's tags into the Tags table
            foreach (Tag t in newPost.Tags)
                database.Tags.InsertOnSubmit(t);
        }

        /// <summary>
        /// Checks to see if each newPost is a duplicate, and if not adds them to the 
        /// pending inserts. Call SubmitChanges() to push the pending changes to the database.
        /// </summary>
        /// <param name="newPosts">List of new Post objects to add to the database.</param>
        public void CommitPosts(List<Post> newPosts)
        {
            foreach (Post p in newPosts)
                this.CommitPost(p);
        }

        /// <summary>
        /// Removes the old post from the database, then adds the new one.
        /// Call SubmitChanges() to implement the changes.
        /// </summary>
        /// <param name="oldPost">Post to remove</param>
        /// <param name="newPost">Post to add</param>
        public void ReplacePost(Post oldPost, Post newPost)
        {
            DeletePost(oldPost);
            CommitPost(newPost);
        }

        /// <summary>
        /// Removes all posts with matching UniqueID from the database, and backs them up in
        /// a seperate 'trash' database.
        /// </summary>
        /// <param name="post"></param>
        public void DeletePost(Post post)
        {
            var matches = db.Posts.Where(p => p.UniqueID == post.UniqueID);
            if (matches.Count() < 1)
                return;

            foreach (PostTableLayer q in matches)
            {
                AddToTrash(q);
                db.Posts.DeleteOnSubmit(q);
            }
        }

        /// <summary>
        /// Backs up the post to a secondary trash database (at 'normaldb.mdf_trash.mdf')
        /// </summary>
        protected void AddToTrash(PostTableLayer post)
        {
            string trashConnectionString = Archive.GenerateConnectionString(this.RelativeFilename + "_trash.mdf");
            using (PostsDataContext db_trash = new PostsDataContext(trashConnectionString))
            {
                if (!db_trash.DatabaseExists())
                    db_trash.CreateDatabase();

                CommitPost(post, db_trash);
                db_trash.SubmitChanges();
            }
        }

        /// <summary>
        /// Searches for and retrieves the first post with matching UniqueID to the 'postID' provided.
        /// </summary>
        /// <param name="postID">UniqueID of the Post to retrieve.</param>
        public Post PullPost(Guid postID)
        {
            var queried = db.Posts.Where(p => p.UniqueID == postID);
            Post pulled = queried.First().TryCreatePost();

            if (pulled == null)
                throw new DatabasePullException(postID.ToString());

            return pulled;
        }

        /// <summary>
        /// Retrieves all posts from the database that satisfy the query.
        /// </summary>
        /// <param name="query">
        /// The function to test each post in the database with.
        /// </param>
        /// <returns>List of Posts where the 'query' function returned true. If any posts could not
        /// be parsed from the PostTableLayer they will appear in the list as null.</returns>
        public List<Post> PullPostsWhere(Func<PostTableLayer, bool> query)
        {
            var queried = db.Posts.Where(query);

            if (queried.Count() < 1)
                throw new DatabasePullException(query.ToString());

            List<Post> pulled = new List<Post>();
            foreach (PostTableLayer layer in queried)
                pulled.Add(layer.TryCreatePost());

            return pulled;
        }
        
        /// <summary>
        /// Dumps all retrievable posts from the database into text file at specified location.
        /// </summary>
        public void Dump(string dumpFilename)
        {
            using(StreamWriter dumpStream = File.CreateText(
                    Path.Combine(Directory.GetCurrentDirectory(), dumpFilename)))
            {
                try
                {
                    foreach (PostTableLayer p in this.PostTable)
                    {
                        dumpStream.WriteLine(p.ToString() + "\n");
                    }
                }
                catch(Exception e)
                {
                    ExceptionTools.WriteExceptionText(e, true);
                }
            }
        }

        /// <summary>
        /// Dumps all retrievable posts from the database into an (unsorted) array of Post objects.
        /// </summary>
        public Post[] Dump()
        {
            Post[] dumped = new Post[this.PostTable.Count()];
            int i = 0;

            foreach(PostTableLayer layer in this.PostTable)
            {
                dumped[i] = layer.TryCreatePost();
                i++;
            }

            return dumped;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                _disposed = true;
            }
        }

        public static void TestHarness()
        {
            List<Post> test_posts = new List<Post>();
            for (int i = 0; i < 12; i++)
            {
                test_posts.Add(HtmlGeneration.TextPost.Dummy());
                Extender.Debugging.Debug.WriteMessage(test_posts[i].ToString());
            }

            Archive db = new Archive(@"Post_TestDB_2014-17-07_001.mdf");
            db.CommitPosts(test_posts);
            db.SubmitChanges();
            db.Dump(@"2014-29-04_003.dump.txt");
        }
    } 

    public class DatabasePullException : System.Configuration.ConfigurationException
    {
        public DatabasePullException(string criteria) :
            base
             (
                 string.Format
                     (
                         "Post(s) with criteria [{0}] could not be found in the database, or its category could not be determined",
                         criteria
                     )
             ) { }
    }
}
