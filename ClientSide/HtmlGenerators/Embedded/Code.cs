using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    public class Code : IResource
    {
        public Code(string name, string sampleText)
        {
            Name = name;
            Value = sampleText;

            LineNum = 1; // defaults
            PreformatClass = "prettyprint"; 
        }

        public Code(string name, string sampleText, int lineNum) : this(name, sampleText)
        {
            LineNum = lineNum;
        }

        public Code(string name, string sampleText, string preformatClass) : this(name, sampleText)
        {
            PreformatClass = preformatClass;
        }

        public Code(string name, string sampleText, int lineNum, string preformatClass) : this(name, sampleText, lineNum)
        {
            PreformatClass = preformatClass;
        }

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

        public int LineNum
        {
            get;
            set;
        }

        public string PreformatClass 
        { 
            get; set; 
        }

        public string CreateHTML()
        {
            StringBuilder build = new StringBuilder();
            build.AppendLine("\n<pre class=\"" + this.PreformatClass + " linenums:" + this.LineNum + "\"><code>");
            build.AppendLine(this.Value);
            build.AppendLine(@"</code></pre>");

            return build.ToString();
        }
    }
}
