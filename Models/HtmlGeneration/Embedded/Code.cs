using PushPost.Models.HtmlGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

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

        private string AttributeString
        {
            get
            {
                //  Due to the way HtmlTextWriter.RenderBeginTag(...) renders the code tag
                // (adds a new line after) there is always an empty line above the code.
                //  So to get the line number to correspond to the correct line we start 
                // line numbering one line before.
                return string.Format("{0} linenums:{1}",
                    PreformatClass,
                    (((LineNum - 1) > 0 ? (LineNum - 1) : LineNum)
                    .ToString()));
            }
        }

        public override string CreateHTML()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter html = new HtmlTextWriter(buffer))
            {
                html.AddAttribute(HtmlTextWriterAttribute.Class, AttributeString);
                html.RenderBeginTag(HtmlTextWriterTag.Pre);
                    html.RenderBeginTag(HtmlTextWriterTag.Code);
                    html.WriteLine(string.Empty);

                    using (StringReader reader = new StringReader(this.Value))
                    {
                        while (reader.Peek() != -1)
                        {
                            html.WriteLine(reader.ReadLine());
                        }
                    }

                    html.RenderEndTag();
                html.RenderEndTag();

                return buffer.ToString();
            }


            //StringBuilder build = new StringBuilder();
            //build.AppendLine("\n<pre class=\"" + this.PreformatClass + " linenums:" + this.LineNum + "\"><code>");
            //build.AppendLine(this.Value);
            //build.AppendLine(@"</code></pre>");
            //return build.ToString();
        }
    }
}
