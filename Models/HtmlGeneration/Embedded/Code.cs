using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class Code : NotifyingResource
    {
        /// <summary>
        /// Initializes a new instance of the Code class, with no values
        /// for the IResource properties.
        /// </summary>
        public Code()
        {
            // defaults
            _LineNum = 1; 
            _PreformatClass = "prettyprint"; 
        }

        public Code(string name, string sampleText):this()
        {
            _Name = name;
            _Value = sampleText;
        }

        public Code(string name, string sampleText, int lineNum) : this(name, sampleText)
        {
            _LineNum = lineNum;
        }

        public Code(string name, string sampleText, string preformatClass) : this(name, sampleText)
        {
            _PreformatClass = preformatClass;
        }

        public Code(string name, string sampleText, int lineNum, string preformatClass) : this(name, sampleText, lineNum)
        {
            _PreformatClass = preformatClass;
        }

        #region Boxed properties

        private int    _LineNum;
        private string _PreformatClass;

        #endregion

        public int LineNum 
        {
            get
            {
                return _LineNum;
            }
            set
            {
                _LineNum = value;
                OnPropertyChanged("LineNum");
            }
        }

        public string PreformatClass 
        {
            get
            {
                return _PreformatClass;
            }
            set
            {
                _PreformatClass = value;
                OnPropertyChanged("PreformatClass");
            }
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
