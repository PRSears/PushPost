using System.Data.Linq;
using PushPost.ClientSide.HtmlGenerators;
using PushPost.ClientSide.HtmlGenerators.Embedded;


namespace PushPost.ClientSide.Database
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
