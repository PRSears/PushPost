using PushPost.Models.HtmlGeneration.Embedded;
using System.Data.Linq;


namespace PushPost.Models.Database
{
    public class PostsDataContext : DataContext
    {
        public Table<PostTableLayer> Posts;
        public Table<Footer> Footnotes;
        public Table<Tag> Tags;
        public Table<Photo> Photos;

        public PostsDataContext(string ConnectionString):base(ConnectionString)
        {
            // Any logic required in the future will go here.
        }
    }
}

