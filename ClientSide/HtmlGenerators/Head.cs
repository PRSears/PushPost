using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.ClientSide.HtmlGenerators
{
    public class Head
    {
        public string Title
        {
            get;
            set;
        }

        public List<string> Stylesheets
        {
            get;
            set;
        }

        public List<string> FontURLs
        {
            get;
            set;
        }

        public Head(string title)
        {
            this.Title = title;
        }

        public Head(string title, List<string> stylesheets):this(title)
        {
            this.Stylesheets = stylesheets;
        }

        public Head(string title, List<string> stylesheets, List<string> fonts):this(title, stylesheets)
        {
            this.FontURLs = fonts;
        }

        public string Create()
        {
            // HACK should use HtmlTextWriter instead of this abomination
            StringBuilder build = new StringBuilder();

            build.AppendLine(@"<head>");
            build.AppendLine("\t<title>" + this.Title + @"</title>");

            foreach (string font in this.FontURLs)
                build.AppendLine("<link href='" + font + "' rel='stylesheet' type='text/css'>");
            foreach (string stylesheet in this.Stylesheets)
                build.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + stylesheet + "\"/>");

            build.Append("</head>");

            return build.ToString();
        }
    }
}
