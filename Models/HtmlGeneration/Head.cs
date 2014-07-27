using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PushPost.Models.HtmlGeneration
{
    /// <summary>
    /// Stores data pertaining to the generation of HTML for the Head
    /// section of a page.
    /// </summary>
    public class Head
    {
        public string Title
        {
            get;
            set;
        }

        public List<string> HypertextReferences
        {
            get;
            set;
        }

        public string PrettyPrintScriptSource
        {
            get;
            set;
        }

        public Head(string title)
        {
            this.Title = title;

            this.PrettyPrintScriptSource = 
            @"<script src=""https://google-code-prettify.googlecode.com/svn/loader/run_prettify.js?skin=sons-of-obsidian"" defer=""defer""></script>";
        }

        public Head(string title, List<string> hypertextReferences):this(title)
        {
            this.HypertextReferences = hypertextReferences;
        }

        /// <summary>
        /// Generates full HTML for the Head section of a webpage with data
        /// contained in this object. 
        /// </summary>
        /// <returns>Full HTML representation of this object.</returns>
        public string Create()
        {
            // HACK should use HtmlTextWriter instead of this abomination
            StringBuilder build = new StringBuilder();

            build.AppendLine(@"<head>");
            build.AppendLine("\t<title>" + this.Title + @"</title>");

            foreach (string href in this.HypertextReferences)
                build.AppendLine("<link href='" + href + "' rel='stylesheet' type='text/css'/>");

            build.AppendLine(this.PrettyPrintScriptSource);

            build.Append("</head>");

            return build.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Head))
                return false;

            Head b = (Head)obj;

            return
            (
                this.Title.Equals(b.Title) &&
               (this.HypertextReferences.Except(b.HypertextReferences).Count() == 0 )       
            );
        }
    }
}
