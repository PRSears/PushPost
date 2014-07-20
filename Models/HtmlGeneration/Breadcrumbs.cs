using System;
using System.IO;
using System.Web.UI;
using System.Linq;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration;
using System.Text;

namespace PushPost.Models.HtmlGeneration
{
    /// <remarks>
    /// Class used for storing data relevent to lower navigational links ("breadcrumbs"),
    /// and functions to render them to HTML.
    /// </remarks>
    public class Breadcrumbs
    {
        /// <summary>
        /// List of URLS pointing to the pages to be included in the breadcrumb traversal links.
        /// </summary>
        public List<string> Links
        {
            get;
            set;
        }

        /// <summary>
        /// One based index representing the place in the "Links" list the current page occupies.
        /// This is the same as the page number.
        /// </summary>
        public int CurrentPageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The total number of pages to display links for. Default value is 9.
        /// </summary>
        public int DisplayLinksNum
        {
            get;
            set;
        }
        
        /// <summary>
        /// Optional additional text to be rendered below the navigation links.
        /// </summary>
        public string Copyright
        {
            get;
            set;
        }
                
        /// <summary>
        /// Constructs a new Breadcrumbs generator object.
        /// </summary>
        /// <param name="links">List of URLS pointing to the pages to be included in the breadcrumb 
        /// traversal links.</param>
        /// <param name="currentIndex">One based index representing the place in the "Links" list 
        /// the current page occupies.
        /// This is the same as the page number.</param>
        public Breadcrumbs(List<string> links, int currentIndex, string copyright = "")
        {
            Links               = links;
            CurrentPageIndex    = currentIndex;
            DisplayLinksNum     = 9;            // default to showing links to 9 pages
            Copyright           = copyright;
        }

        /// <summary>
        /// Constructs a new Breadcrumbs generator object with no links, and therefor no navigation
        /// will be generated until the Links list is populated.
        /// </summary>
        public Breadcrumbs(string copyright = "") : this(new List<string>(), 1, copyright) { }
                
        /// <returns>
        /// Returns a string containing the rendered HTML for Breadcrumbs generated from 
        /// the "Links" list. 
        /// </returns>
        public string Create()
        {
            if (Links.Count < 1 || DisplayLinksNum < 1)
                return string.Empty; // No links means no navigation neccessary
            if ((CurrentPageIndex > Links.Count) || (CurrentPageIndex < 1))
                throw new ArgumentException("CurrentIndex is outside the range of Links list.");

            int leftNum    = (int)Math.Floor((DisplayLinksNum - 1) / 2d);
            int rightNum   = DisplayLinksNum - leftNum - 1;

            int firstIndex = (CurrentPageIndex - leftNum) < 1 ? 1 : (CurrentPageIndex - leftNum);
            int finalIndex = CurrentPageIndex + (DisplayLinksNum - (CurrentPageIndex - firstIndex)) - 1;

            if(finalIndex > Links.Count) // if there's not enough room to the right of the current index
            {
                finalIndex = Links.Count;
                int remainder = (DisplayLinksNum - (finalIndex - CurrentPageIndex));
                // Use the remaining slots to display as many pages to the right as possible
                firstIndex = (CurrentPageIndex - remainder - 1) < 1 ? 1 : (CurrentPageIndex - remainder - 1); 
            }

            // create a clone of the list of Links, with position 0 empty to make page number to
            // index conversion easier. (index one == page one, instead of index one == page 2).
            List<string> Links_padded = new List<string>(Links);
            Links_padded.Insert(0, string.Empty); 

            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                //writer.AddAttribute(HtmlTextWriterAttribute.Id, "primary-column");
                //writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    if(this.Copyright != "")
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "copyright");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            writer.Write(this.Copyright);
                        writer.RenderEndTag();
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, "navigation-bar-bottom");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "navigation-links");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        if (CurrentPageIndex > 1)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, Links_padded[CurrentPageIndex - 1]);
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(" < Prev > ");
                            writer.RenderEndTag();
                            writer.WriteLine(string.Empty);
                        }
                        for (int i = firstIndex; i <= finalIndex; i++)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, Links_padded[i]);
                            if (i == CurrentPageIndex) writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-weight:bold"); // bold the current page
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(i);
                            writer.RenderEndTag();
                            writer.WriteLine(string.Empty);
                        }
                        if(CurrentPageIndex < Links.Count)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, Links_padded[CurrentPageIndex + 1]);
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(" < Next > ");
                                writer.RenderEndTag();
                                writer.WriteLine(string.Empty);
                        }
                        writer.RenderEndTag();
                    writer.RenderEndTag();
                //writer.RenderEndTag();


                return buffer.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Breadcrumbs))
                return false;

            Breadcrumbs b = (Breadcrumbs)obj;

            return
            (
                (this.Links.Except(b.Links).Count() == 0)         && 
                 this.CurrentPageIndex.Equals(b.CurrentPageIndex) &&
                 this.DisplayLinksNum.Equals(b.DisplayLinksNum)
            );
        }

        public static string TestHarness()
        {
            string[] links = { "page1.htm", "page2.html", "page3.htm", "page4.htm", "page5.htm", "page6.htm", "page7.htm", "page8.htm", "page9.htm", "page10.htm", "page11.htm" };

            Breadcrumbs t1 = new Breadcrumbs(new List<string>(links), 1);
            Breadcrumbs t2 = new Breadcrumbs(new List<string>(links), 3);
            Breadcrumbs t3 = new Breadcrumbs(new List<string>(links), 5);
            Breadcrumbs t4 = new Breadcrumbs(new List<string>(links), 7);
            Breadcrumbs t5 = new Breadcrumbs(new List<string>(links), 9);
            Breadcrumbs t6 = new Breadcrumbs(new List<string>(links), 10);
            Breadcrumbs t7 = new Breadcrumbs(new List<string>(links), 11);

            StringBuilder build = new StringBuilder();

            build.AppendLine("Page 1 selected:\n");
            build.AppendLine(t1.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 3 selected:\n");
            build.AppendLine(t2.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 5 selected:\n");
            build.AppendLine(t3.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 7 selected:\n");
            build.AppendLine(t4.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 9 selected:\n");
            build.AppendLine(t5.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 10 selected:\n");
            build.AppendLine(t6.Create());
            build.AppendLine(string.Empty);


            build.AppendLine("Page 11 selected:\n");
            build.AppendLine(t7.Create());
            build.AppendLine(string.Empty);

            return build.ToString();
        }
    }
}
