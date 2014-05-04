using System.IO;
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
                            for(int i = 0; i < Categories.Count; i++)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, Categories[i].MainPageURL);
                                if (Categories[i].Equals(this.CurrentCategory))
                                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "current");
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                    writer.Write(Categories[i].Category.ToUpper());
                                writer.RenderEndTag();

                                if(i < (Categories.Count - 1)) // there's still another category to be added
                                {
                                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "spacer");
                                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                                        writer.Write("+");
                                    writer.RenderEndTag();
                                }
                            }
                        writer.RenderEndTag();
                    writer.RenderEndTag();
                writer.RenderEndTag();

                return buffer.ToString();
            }
        }
    }
}
