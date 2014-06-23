using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Linq;
using PushPost.ClientSide.HtmlGenerators;
using PushPost.ClientSide.HtmlGenerators.Embedded;

namespace PushPost.ClientSide.Database
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
            protected get;
            protected set;
        }
        protected string _PostDB_ConnectionString;

        public string PostDB_ConnectionString
        {
            get
            {
                return this._PostDB_ConnectionString;
            }
            set
            {
                if (value.Contains(@"|DataDirectory|"))
                    this._PostDB_ConnectionString = value.Replace(@"|DataDirectory|", Directory.GetCurrentDirectory());
                else
                    this._PostDB_ConnectionString = value;
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

        public Archive(string databaseRelativePath)
        {
            this.RelativeFilename = databaseRelativePath;

            this._PostDB_ConnectionString = Archive.GenerateConnectionString(this.RelativeFilename);
            db = new PostsDataContext(this.PostDB_ConnectionString);

            if (!db.DatabaseExists())
                db.CreateDatabase();
        }

        public static string GenerateConnectionString(string relativeFilename)
        {
            return string.Format(
                "Data Source=(LocalDB)\\v11.0;AttachDbFilename={0}\\{1};Integrated Security=False;Pooling=false;",
                Directory.GetCurrentDirectory(),
                relativeFilename);
        }

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

        public Post PullPost(Guid postID)
        {
            // TODO search for, and retrieve post with matching postID from the database
            throw new NotImplementedException();
        }

        public List<Post> PullPosts(HtmlGenerators.PostTypes.NavCategory category)
        {
            // TODO retrieve all posts from the database matching category.  
            throw new NotImplementedException();
        }

        public List<Post> PullPosts(DateTime rangeStart, DateTime rangeEnd)
        {
            // TODO retrieve all posts falling between start and end dates
            throw new NotImplementedException();
        }

        public List<Post> PullPosts(
            DateTime rangeStart, 
            DateTime rangeEnd, 
            HtmlGenerators.PostTypes.NavCategory category)
        {
            // TODO retrieve all posts falling inside the date range and matching category
            throw new NotImplementedException();
        }

        public List<Post> PullPosts(string title)
        {
            throw new NotImplementedException();
        }

        public List<Post> PullPosts(DateTime date)
        {
            throw new NotImplementedException();
        }
        
        public void Dump(string dumpFilename)
        {
            // TODO Dump all posts from the database into a txt file
            throw new NotImplementedException();
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
    }
}
