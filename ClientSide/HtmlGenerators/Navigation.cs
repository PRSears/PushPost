﻿using System.IO;
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using PushPost.ClientSide.HtmlGenerators.PostTypes;

namespace PushPost.ClientSide.HtmlGenerators
{
    public class Navigation
    {
        public List<PostTypes.NavCategory> Categories
        {
            get;
            set;
        }

        public PostTypes.NavCategory CurrentCategory
        {
            get;
            set;
        }

        public Navigation()
        {
            Categories = new List<PostTypes.NavCategory>(PostTypes.NavCategory.AllCategories);
            CurrentCategory = PostTypes.NavCategory.Contact; 
        }

        public Navigation(NavCategory currentCategory):this()
        {
            CurrentCategory = currentCategory;
        }

        public Navigation(List<PostTypes.NavCategory> categories):this()
        {
            this.Categories = categories;
        }

        public Navigation(List<NavCategory> categories, NavCategory currentCategory):this(currentCategory)
        {
            this.Categories = categories;
        }
        
        /// <returns>
        /// Returns a string containing the rendered HTML for Navigation generated from 
        /// the list of Categories. 
        /// </returns>
        public string Create()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, "navigation-bar");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, "header-center");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "logo-image");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.RenderEndTag();

                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "navigation-links");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.WriteLine(string.Empty);
                            for(int i = 0; i < Categories.Count; i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, Categories[i].MainPageURL);
                                if (Categories[i].Equals(this.CurrentCategory))
                                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "current");
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                    writer.Write(Categories[i].Category.ToUpper());
                                writer.RenderEndTag();
                                writer.WriteLine(string.Empty);
                                if(i < (Categories.Count - 1)) // there's still another category to be added
                                {
                                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "spacer");
                                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                                        writer.Write("+");
                                    writer.RenderEndTag();
                                    writer.WriteLine(string.Empty);
                                }
                            }
                        writer.RenderEndTag();
                    writer.RenderEndTag();
                writer.RenderEndTag();

                return buffer.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Navigation))
                return false;

            Navigation b = (Navigation)obj;

            return
            (
                (this.Categories.Except(b.Categories).Count() == 0) &&
                 this.CurrentCategory.Equals(b.CurrentCategory)
            );
        }

        public static string TestHarness()
        {
            StringBuilder build = new StringBuilder();

            Navigation n1 = new Navigation();
            Navigation n2 = new Navigation(NavCategory.Photography);

            build.AppendLine("Default constructor: ");
            build.AppendLine(n1.Create());
            build.AppendLine(string.Empty);
            build.AppendLine("Set category to photography: ");
            build.AppendLine(n2.Create());

            return build.ToString();
        }
    }
}
