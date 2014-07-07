using System;
using Extender.Strings;

namespace PushPost.Models.HtmlGeneration.PostTypes
{
    public class NavCategory
    {
        //
        // TODO_ Perhaps try to match unknown categories against a categories.cfg 
        //       file with (optional) extra listings.
        //  
        // Dear Future Me,
        //      Just use typeof(class) and pass around the Type object,
        //      instead of this convoluted mess.
        //      

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
        // If new properties are added, make sure to update the statics, and Equals() method.
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

        public string ToTitleString()
        {
            return Category.ToPropercase();
        }

        public static NavCategory Parse(string category)
        {
            foreach (NavCategory comparator in AllCategories)
                if (category.ToLower() == comparator.Category.ToLower())
                    return comparator;

            foreach (NavCategory comparator in AllCategories)
                if (category.ToLower().Contains(comparator.ToString()))
                {
                    Extender.Debugging.Debug.WriteMessage("Parsed category was not a direct match.", "warn");
                    return comparator;
                }

            throw new ArgumentException("category provided is not a valid NavCategory.");
        }

        /// <summary>
        /// Tries to parse the category, and returns NavCategory.None if the parse fails.
        /// </summary>
        public static NavCategory TryParse(string category)
        {
            try
            {
                return Parse(category);
            }
            catch(ArgumentException e)
            {
                Extender.Debugging.Debug.WriteMessage(string.Format
                    ("Parsing {0} failed.", category), "eroor");
                return NavCategory.None;
            }
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
