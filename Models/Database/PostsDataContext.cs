using System.Data.Linq;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;


namespace PushPost.Models.Database
{
    public class PostsDataContext : DataContext
    {
        public Table<PostTableLayer> Posts;
        public Table<Footer> Footnotes;
        public Table<Tag> Tags;

        public PostsDataContext(string ConnectionString):base(ConnectionString)
        {
            // Any logic required in the future will go here.
        }
    }
}
