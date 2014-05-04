using System.Collections.Generic;
using System;

namespace PushPost.ClientSide.HtmlGenerators
{
    public class Page
    {        
        // TODO make a "Page" class that will hold the actual HTML (like my post class) --
        //      and use an interface for classes to generate sets of pages. A "PageGenerator" should
        //      be able to sort and group a set of posts and generate appropriate page(s) HTML.
        //
        // Perhaps instead of an IPageGenerator interface, there should just be a fully implemented 
        // PageGenerator class that takes a list of Post objects - and determines the correct category
        // from the Posts - then generates the appropriate list of pages for those posts.
        //
        // This has the problem that if I later want to add more categories the PageGenerator class 
        // would need to be modified, instead of simply creating a new implementation of the interface.
        //
        // PageGenerator should do a Ceiling(Posts.Count / 10) to get neccessary number of pages 
        // and generate lower breadcrumbs based on that.

        public Head Header
        {
            get;
            set;
        }

        public Navigation NavigationBar
        {
            get;
            set;
        }

        public int PageNumber
        {
            get;
            set;
        }



        public Page(Head header, Navigation navs)
        {
            this.Header = header;
            this.NavigationBar = navs;
        }

        //string PageTitle { get; set; }
        //PostTypes.NavCategory Category { get; set; }
        //int PageNumber { get; set; }
        //string Header { get; set; }
        //List<HtmlGenerators.Post> Posts { get; set; }
        //string Footer { get; set; }
        //PageGenerationMethod GenerationMethod { get; set; }

        //string Create();
        //void ImportHeader(string pathToFile);
        //void ImportFooter(string pathToFile);
    }

    //TODO this has to be moved to the file with PageGenerator
    public class PageGenerationMethod
    {
        private string Method;

        public static PageGenerationMethod DateDescending { get { return new PageGenerationMethod("date_descending"); } }
        public static PageGenerationMethod DateAscending { get { return new PageGenerationMethod("date_ascending"); } }
        public static PageGenerationMethod AuthorDescending { get { return new PageGenerationMethod("author_descending"); } }
        public static PageGenerationMethod AuthorAscending { get { return new PageGenerationMethod("author_ascending"); } }
        public static PageGenerationMethod TitleDescending { get { return new PageGenerationMethod("title_descending"); } }
        public static PageGenerationMethod TitleAscending { get { return new PageGenerationMethod("title_ascending"); } }

        private static PageGenerationMethod[] AllMethods = { DateDescending, DateAscending, AuthorDescending, AuthorAscending, TitleDescending, TitleAscending };

        private PageGenerationMethod(string method)
        {
            this.Method = method;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PageGenerationMethod))
                return false;

            PageGenerationMethod b = (PageGenerationMethod)obj;

            return this.Method.Equals(b.Method);
        }

        public override int GetHashCode()
        {
            return this.Method.GetHashCode();
        }

        public static PageGenerationMethod Parse(string method)
        {
            foreach (PageGenerationMethod o in AllMethods)
                if (o == method)
                    return o;

            throw new ArgumentException("Method provided is not a valid PageGenerationMethod.");
        }

        #region Comparison operators
        public static bool operator ==(PageGenerationMethod a, PageGenerationMethod b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PageGenerationMethod a, PageGenerationMethod b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(PageGenerationMethod a, string b)
        {
            return a.Method.Equals(b);
        }

        public static bool operator !=(PageGenerationMethod a, string b)
        {
            return !a.Method.Equals(b);
        }

        public static bool operator ==(string a, PageGenerationMethod b)
        {
            return a.Equals(b.Method);
        }

        public static bool operator !=(string a, PageGenerationMethod b)
        {
            return !a.Equals(b.Method);
        }
        #endregion
    }
}
