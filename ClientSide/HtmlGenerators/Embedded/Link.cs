using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    /// <remarks>
    /// IResource implementation for creating hypertext links.
    /// </remarks>
    public class Link : IResource
    {
        /// <summary>
        /// Name of the reference for markup.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// URL being linked to.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Text enclosed in the <code><a></code> tag.
        /// </summary>
        public string EnclosedText
        {
            get;
            set;
        }

        public Link(string resourceName, string url, string enclosedText)
        {
            Name = resourceName;
            Value = url;
            EnclosedText = enclosedText;
        }

        /// <summary>
        /// Generates the HTML for this link.
        /// </summary>
        /// <returns></returns>
        public string CreateHTML()
        {
            return string.Format(@"<a href=""{0}"">{1}</a>", Value, EnclosedText);
            //return "<a href=\"" + Value + "\">" + EnclosedText + @"</a>";
        }
    }
}
