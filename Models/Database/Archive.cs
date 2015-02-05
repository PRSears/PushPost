using Extender.Debugging;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;

namespace PushPost.Models.Database
{
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
        public Table<Photo> PhotosTable
        {
            get
            {
                if (db == null)
                    return null;

                return db.Photos;
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
            // TODO Break post up if it's too long

            // Check to see if it already exists in the database.
            if (database.Posts.Where(p => p.UniqueID == newPost.UniqueID).Count() > 0)
                return;

            // Insert newPost into Posts table
            database.Posts.InsertOnSubmit(newPost);
            // Insert newPost's footers into the Footnotes table
            foreach (Footer f in newPost.Footers)
            {
                f.PostID = newPost.UniqueID;
                f.ForceNewUniqueID();
                database.Footnotes.InsertOnSubmit(f);
            }
            // Insert newPost's tags into the Tags table
            foreach (Tag t in newPost.Tags)
            {
                t.PostID = newPost.UniqueID;
                t.ForceNewUniqueID();
                database.Tags.InsertOnSubmit(t);
            }
            // Insert newPost's photos into the Photos table
            foreach (Photo p in newPost.Photos)
            {
                p.PostID = newPost.UniqueID;
                p.ForceNewUniqueID();
                database.Photos.InsertOnSubmit(p);
            }
        }

        /// <summary>
        /// Checks to see if each newPost is a duplicate, and if not adds them to the 
        /// pending inserts. Call SubmitChanges() to push the pending changes to the database.
        /// </summary>
        /// <param name="newPosts">List of new Post objects to add to the database.</param>
        public void CommitPosts(Post[] newPosts)
        {
            foreach (Post p in newPosts)
                this.CommitPost(p);
        }

        protected void CommitPosts(PostTableLayer[] newPosts, PostsDataContext database)
        {
            foreach(PostTableLayer p in newPosts)
                this.CommitPost(p, database);
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
        public void DeletePost(Post post)
        {
            var matches = db.Posts.Where(p => p.UniqueID.Equals(post.UniqueID));
            if (matches.Count() < 1)
            {
                Debug.WriteMessage(string.Format(
                    "DeletePost could not find post with GUID [{0}]", post.UniqueID.ToString()), "info");
                return;
            }

            db.Posts.DeleteAllOnSubmit(matches);
            AddToTrash(matches.ToArray());
        }

        /// <summary>
        /// Removes all posts with matching UniqueID from the database, and backs them up in
        /// a seperate 'trash' database.
        /// </summary>
        public void DeletePost(string title, DateTime date)
        {
            var matches = db.Posts.Where(p => p.Title.Equals(title) && p.Timestamp.Date.Equals(date.Date));
            if(matches.Count() < 1)
            {
                Debug.WriteMessage(string.Format("DeletePost could not find post [{0} {1}] in the database",
                    title, date.ToShortDateString()), "info");
                return;
            }

            db.Posts.DeleteAllOnSubmit(matches);
            AddToTrash(matches.ToArray());
        }

        /// <summary>
        /// Removes all posts found in the database with UniqueIDs matching any UniqueIDs of the provided
        /// posts.
        /// </summary>
        /// <param name="posts">Array of posts to search for and (if found) remove from the database.</param>
        public void DeletePosts(Post[] posts)
        {
            //var matches = db.Posts.Join(
            //    posts.Select(ids => ids.UniqueID),
            //    layer => layer.UniqueID,
            //    id => id,
            //    (layer, id) => layer);

            var matches = db.Posts.Where(m => posts.Select(p => p.UniqueID)
                                                   .Contains(m.UniqueID));

            if(matches.Count() < 1)
            {
                Debug.WriteMessage("DeletePosts could not find any matching Post UniqueIDs", "info");
                return;
            }

            db.Posts.DeleteAllOnSubmit(matches);
            AddToTrash(matches.ToArray());
        }

        protected string TrashConnectionString
        {
            get
            {
                return Archive.GenerateConnectionString(this.RelativeFilename + "_trash.mdf");
            }
        }

        /// <summary>
        /// Backs up the post to a secondary trash database (at 'normaldb.mdf_trash.mdf')
        /// </summary>
        protected void AddToTrash(PostTableLayer post)
        {
            using (PostsDataContext db_trash = new PostsDataContext(this.TrashConnectionString))
            {
                if (!db_trash.DatabaseExists())
                    db_trash.CreateDatabase();

                CommitPost(post, db_trash);
                db_trash.SubmitChanges();
            }
        }

        /// <summary>
        /// Backs up the posts to a secondary trash database (at 'normaldb.mdf_trash.mdf')
        /// </summary>
        protected void AddToTrash(PostTableLayer[] posts)
        {
            using (PostsDataContext db_trash = new PostsDataContext(this.TrashConnectionString))
            {
                if (!db_trash.DatabaseExists())
                    db_trash.CreateDatabase();

                CommitPosts(posts, db_trash);
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

            pulled.Footers  = db.Footnotes.Where(f => f.PostID.Equals(postID))
                                          .ToList();

            pulled.Tags     = db.Tags.Where(t => t.PostID.Equals(postID))
                                     .ToList();

            pulled.Resources.AddRange
                (
                    db.Photos.Where(p => p.PostID.Equals(postID))
                             .ToList()
                );

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
        public Post[] TryPullPostsWhere(Func<PostTableLayer, bool> query)
        {
            var queried = db.Posts.Where(query).ToArray();

            if (queried.Length < 1)
            {
                Debug.WriteMessage(string.Format("Query ({0}) returned no results.",
                    query.ToString()),
                    "info");
                return null;
            }

            //List<Post> pulled = new List<Post>();
            //foreach (PostTableLayer layer in queried)
            //    pulled.Add(layer.TryCreatePost());

            Post[] pulled = new Post[queried.Length];
            for (int i = 0; i < pulled.Length; i++)
            {
                pulled[i] = queried[i].TryCreatePost();
                if(pulled[i] != null)
                {
                    pulled[i].Footers   = db.Footnotes.Where(f => f.PostID.Equals(pulled[i].UniqueID))
                                                      .ToList();

                    pulled[i].Tags      = db.Tags.Where(t => t.PostID.Equals(pulled[i].UniqueID))
                                                 .ToList();

                    pulled[i].Resources.AddRange
                        (
                            db.Photos.Where(p => p.PostID.Equals(pulled[i].UniqueID))
                                     .ToList()
                        );
                }
            }

            return pulled;
        }

        public Post[] PullPostsWhere(Func<PostTableLayer, bool> query)
        {
            Post[] pulled = TryPullPostsWhere(query);

            if (pulled == null) throw new DatabasePullException(query.ToString());
            else return pulled;
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
                dumped[i] = this.PullPost(layer.UniqueID);
                i++;
            }

            return dumped;
        }

        public void DumpToDebug()
        {
            foreach(PostTableLayer p in db.Posts)
            {
                Console.WriteLine("0: " + p.UniqueID.ToString());
                Console.WriteLine("1: " + p.TryCreatePost().UniqueID.ToString() + "\n");
            }
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
                    // THOUGHT should I submit all pending operations before
                    //         disposal?
                    //         No, probabaly not. Would lead to unexpected behaviour
                    db.Dispose();
                }
                _disposed = true;
            }
        }

        public static void TestHarness()
        {
            List<Post> stored = new List<Post>();
            List<Post> toFind = new List<Post>();

            for(int i = 0; i < 10; i++)
            {
                stored.Add(TextPost.Dummy());
            }

            toFind.Add(stored[2]);
            toFind.Add(stored[4]);
            toFind.Add(stored[6]);

            Console.WriteLine("All posts in stored: ");
            foreach (Post post in stored) Console.WriteLine(post.UniqueID.ToString());

            Console.WriteLine("\nLooking for these: ");
            foreach (Post post in toFind) Console.WriteLine(post.UniqueID.ToString());

            //var matches = stored.Join(
            //    toFind.Select(ids => ids.UniqueID),
            //    layer => layer.UniqueID,
            //    id => id,
            //    (layer, id) => layer);

            var matches = stored.Where(s => toFind.Select(t => t.UniqueID)
                                                  .Contains(s.UniqueID));

            Console.WriteLine("\nRetrieved: ");
            foreach (var match in matches) Console.WriteLine(match.UniqueID.ToString());

            #region commit / delete
            //using (Archive arc = new Archive("test_harness_002.mdf"))
            //{
            //    Console.Write("Initial dump: \n");
            //    arc.DumpToDebug();

            //    Post testPost = TextPost.TemplatePost();
            //    arc.CommitPost(testPost);
            //    arc.SubmitChanges();

            //    Console.WriteLine("\nAfter post added: ");
            //    arc.DumpToDebug();

            //    Console.WriteLine("\nRetrieving post.");
            //    Post retrieved = arc.PullPostsWhere(p => p.Title == "Enter Title").ToArray()[0];

            //    arc.DeletePost(retrieved);
            //    arc.SubmitChanges();

            //    Console.WriteLine("\nAfter post removed: ");
            //    arc.DumpToDebug();

            //}
            #endregion
        }
    } 

    public class DatabasePullException : System.Configuration.ConfigurationException
    {
        public DatabasePullException(string criteria) : base
            (
                string.Format
                (
                    "Post(s) with criteria [{0}] could not be found in the database, or its category could not be determined",
                    criteria
                )
            ) { }
    }
}
