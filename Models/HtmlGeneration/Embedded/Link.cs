using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    /// <remarks>
    /// IResource implementation for creating hypertext links.
    /// </remarks>
    public class Link : NotifyingResource
    {
        private string _EnclosedText;
        /// <summary>
        /// Text enclosed in the <code><a></code> tag.
        /// </summary>
        public string EnclosedText 
        {
            get
            {
                return _EnclosedText;
            }
            set
            {
                _EnclosedText = value;
                OnPropertyChanged("EnclosedText");
            }
        }
        public Link()
        {}

        public Link(string resourceName, string url, string enclosedText):this()
        {
            _Name = resourceName;
            _Value = url;
            _EnclosedText = enclosedText;
        }

        /// <summary>
        /// Generates the HTML for this link.
        /// </summary>
        /// <returns></returns>
        public override string CreateHTML()
        {
            return string.Format(@"<a href=""{0}"">{1}</a>", Value, EnclosedText);
            //return "<a href=\"" + Value + "\">" + EnclosedText + @"</a>";
        }

    }
}
