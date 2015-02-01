using System.IO;
using System.Web.UI;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public class Photo : NotifyingResource
    {
        public override string CreateHTML()
        {
            using(StringWriter buffer = new StringWriter())
            using(HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, Value);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);

                return buffer.ToString();
            }
        }
    }
}
