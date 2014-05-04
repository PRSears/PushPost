using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    public class Link : IResource
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

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

        public string CreateHTML()
        {
            return "<a href=\"" + Value + "\">" + EnclosedText + @"</a>";
        }
    }
}
