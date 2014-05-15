using System;

namespace PushPost.ClientSide.HtmlGenerators.PostTypes
{
    public class NavCategory
    {
        // If new properties are added, make sure to update the statics, and Equals() method.

        public string Category
        {
            get;
            private set;
        }

        public string MainPageURL
        {
            get;
            private set;
        }

        // !!
        // Remember to change this.AllCategories any time a static constructor is added.
        // !!

        public static NavCategory Photography { get { return new NavCategory("photography"); } }
        public static NavCategory Code { get { return new NavCategory("code"); } }
        public static NavCategory Contact { get { return new NavCategory("contact"); } }
        public static NavCategory Blog { get { return new NavCategory("blog"); } }
        public static NavCategory None { get { return new NavCategory("none"); } }

        public static NavCategory[] AllCategories = { Photography, Code, Contact, Blog };

        private NavCategory(string category)
        {
            Category = category;
            MainPageURL = Category.ToLower() + ".html";
        }

        public override bool Equals(object o)
        {
            NavCategory b;

            if (o is NavCategory)
                b = (NavCategory)o;
            else if (o is string)
                b = new NavCategory((string)o);
            else
                return false;

            return 
            (
                this.Category.Equals(b.Category) &&
                this.MainPageURL.Equals(b.MainPageURL)
            );
        }

        public override int GetHashCode()
        {
            return Category.GetHashCode();
        }

        public override string ToString()
        {
            return Category;
        }

        public static NavCategory Parse(string category)
        {
            foreach (NavCategory cat in AllCategories)
                if (category == cat)
                    return cat;

            throw new ArgumentException("category provided is not a valid NavCategory.");
        }

        public static Boolean operator ==(NavCategory a, NavCategory b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(NavCategory a, NavCategory b)
        {
            return !(a == b);
        }

        public static Boolean operator ==(NavCategory a, string b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(NavCategory a, string b)
        {
            return !a.Equals(b);
        }

        public static Boolean operator ==(string a, NavCategory b)
        {
            return b.Equals(a);
        }

        public static Boolean operator !=(string a, NavCategory b)
        {
            return !b.Equals(a);
        }
    }
}
