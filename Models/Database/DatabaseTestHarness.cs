using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.Models.HtmlGeneration.PostTypes;

namespace PushPost.Models.Database
{
    public static class DatabaseTestHarness
    {
        public static string ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\Post_TestDB_2014-29-04_004.mdf;Integrated Security=False;Pooling=false;";

        public static void TestWrites()
        {
            string connection = ConnectionString;

            if (connection.Contains("|DataDirectory|"))
                    connection = connection.Replace("|DataDirectory|", Directory.GetCurrentDirectory());

            using(PostsDataContext db = new PostsDataContext(connection))
            {
                if (!db.DatabaseExists())
                    db.CreateDatabase();

                TextPost t = TestPost();
                Console.WriteLine(t.ToString());

                db.Posts.InsertOnSubmit(PostTableLayer.FromPost(t));

                foreach(Footer footer in t.Footers)
                    db.Footnotes.InsertOnSubmit(footer);
                foreach(Tag tag in t.Tags)
                    db.Tags.InsertOnSubmit(tag);

                db.SubmitChanges();
            }
        }

        public static void TestRead()
        {
            string connection = ConnectionString;

            if (connection.Contains("|DataDirectory|"))
                    connection = connection.Replace("|DataDirectory|", Directory.GetCurrentDirectory());

            using(PostsDataContext db = new PostsDataContext(connection))
            {
                var q = db.Posts.Where(p => p.Author == "Patrick");

                Console.WriteLine("As PostTableLayer: ");
                foreach (var result in q)
                {
                    Console.WriteLine(result.ToString());
                }

                Console.WriteLine("As Post: ");
                foreach(var result in q)
                {
                    Post t = new TextPost();
                    result.ExportTo(ref t);

                    Console.WriteLine(t.ToString());
                }
            }
        }

        public static TextPost TestPost()
        {
            string head = "TestPostDelta";
            string body = "BBBBBBBBBB\n" + 
                          "Body text for database test post.";

            TextPost t = new TextPost(head, DateTime.Now, "Patrick", body);

            List<Footer> footers = new List<Footer>();
            footers.Add(new Footer("fnote1", "Footnote 1!", t.UniqueID));
            footers.Add(new Footer("fnote2", "Footnote 2!", t.UniqueID));

            t.Footers = footers;
            t.Category = NavCategory.Blog;
            
            return t;
        }

        public static bool ListCompareTest()
        {
            List<string> l1 = new List<string>();
            List<string> l2 = new List<string>();

            l1.Add("First item");
            l1.Add("Second item");
            l1.Add("Third item");

            l2.Add("First item");
            l2.Add("Second item");
            l2.Add("Third item");

            return (l1.Except(l2).ToList().Count == 0);
        }
    }
}
