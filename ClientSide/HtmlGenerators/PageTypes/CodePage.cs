using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.ClientSide.HtmlGenerators.PostTypes;

namespace PushPost.ClientSide.HtmlGenerators
{
    public class CodePage
    {

        public string PageTitle
        {
            get;
            set;
        }

        public NavCategory Category
        {
            get;
            set;
        }

        public int PageNumber
        {
            get;
            set;
        }

        public List<Post> Posts
        {
            get;
            set;
        }

        public CodePage(string pageTitle, NavCategory category, int pageNumber)
        {
            PageTitle = pageTitle;
            Category = category;
            PageNumber = pageNumber;
        }

        public string GenerateHTML()
        {
            throw new NotImplementedException();
        }
    }
}
