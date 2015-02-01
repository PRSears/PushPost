using Extender;
using System;

namespace PushPost.Models.HtmlGeneration
{
    public class NavCategory
    {
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

        public Type PostType
        {
            get;
            private set;
        }

        // !!
        // If new properties are added, make sure to update the statics, and Equals() method.
        // Remember to change this.AllCategories any time a static constructor is added.
        // !!

        public static NavCategory Photography 
        { 
            get 
            { 
                return new NavCategory("photography", typeof(PhotoPost)); 
            } 
        }
        public static NavCategory Code
        {
            get
            {
                return new NavCategory("code", typeof(TextPost));
            }
        }
        public static NavCategory Contact
        {
            get
            {
                return new NavCategory("contact", typeof(TextPost)); 
            }
        }
        public static NavCategory Blog
        {
            get
            {
                return new NavCategory("blog", typeof(TextPost));
            }
        }

        public static NavCategory None
        {
            get
            {
                return new NavCategory("none");
            }
        }

        // TODO think of a non-shitty way to load these from a file / user entered
        //      has to correspond to a type of page to generate
        public static NavCategory[] AllCategories = { Contact, Code, Blog, Photography };

        private NavCategory(string category, Type type)
        {
            Category    = category;
            PostType    = type;
            MainPageURL = System.IO.Path.Combine(
                Category,
                Page.GenerateFilename(this, 1));

            //Category.ToLower() + ".html";
        }

        private NavCategory(string category) : this(category, null) { }

        private NavCategory() : this("none") { }
        
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
            catch
            {
                Extender.Debugging.Debug.WriteMessage(string.Format
                    ("Parsing {0} failed.", category), "eroor");
                return NavCategory.None;
            }
        }

        public override bool Equals(object obj)
        {
            NavCategory other;

            if (obj is NavCategory)
                other = (NavCategory)obj;
            else if (obj is string)
                other = new NavCategory((string)obj);
            else
                return false;

            return 
            (
                this.Category.Equals(other.Category, StringComparison.OrdinalIgnoreCase) &&
                this.MainPageURL.Equals(other.MainPageURL, StringComparison.OrdinalIgnoreCase)
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

        public static Boolean operator ==(NavCategory a, NavCategory b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);

            return a.Equals(b);
        }

        public static Boolean operator !=(NavCategory a, NavCategory b)
        {
            return !(a == b);
        }
    }
}
