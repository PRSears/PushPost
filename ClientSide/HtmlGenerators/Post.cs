using PushPost.ClientSide.HtmlGenerators.Embedded;
using System.Collections.Generic;
using Extender.ObjectUtils;
using System.Web.UI;
using System.Text;
using System.IO;
using System;

namespace PushPost.ClientSide.HtmlGenerators
{
    abstract public class Post
    {
        protected Guid _PostID;
        public Guid UniqueID 
        {
            get
            {
                if (_PostID == null || _PostID.Equals(Guid.Empty))
                    _PostID = new Guid(GetHashData());
                return _PostID;
            }
            protected set
            {
                _PostID = value;
            }
        }

        public string Title;
        public string Author;
        public string MainText;
        public List<Footer> Footers; 
        public DateTime Timestamp;
        public List<IResource> Resources; 
        public List<Tag> Tags;      
        public PostTypes.NavCategory Category;

        protected string HeaderClass;
        protected string FooterClass;

        protected int PreviewLength;

        /// <summary>
        /// Generates HTML markup for this post. 
        /// </summary>
        public virtual string Create()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                RenderHeader(writer);

                writer.Write(writer.NewLine); // single line spacing

                RenderBody(writer);

                writer.Write(writer.NewLine); // single line spacing

                RenderFooter(writer);

                writer.Write(writer.NewLine + writer.NewLine); // double line spacing

                RenderComments(writer);

                writer.Write(writer.NewLine + writer.NewLine + writer.NewLine + writer.NewLine); // quad line spacing

                return buffer.ToString();
            }

        }

        /// <summary>
        /// Generates a preview of the post containing title, author, date and an excerpt of the 
        /// first paragraph.
        /// </summary>
        public virtual string CreatePreview()
        {
            using (StringWriter buffer = new StringWriter())
            using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                RenderHeader(writer);
                writer.Write(writer.NewLine);

                if (MainText.Length < PreviewLength)
                    PreviewLength = MainText.Length;

                // Throw an image at the top from the Resources list (if present)
                foreach(IResource resource in Resources)
                {
                    if(resource is Image)
                    {
                        writer.Write(resource.CreateHTML());
                        break;
                    }
                }

                string preview = ResourceManager.RemoveReferences(MainText.Substring(0, PreviewLength)); // grab the first x characters
                writer.Write(preview);

                return buffer.ToString();
            }

        }

        abstract protected void RenderHeader(HtmlTextWriter w);
        abstract protected void RenderBody(HtmlTextWriter w);
        abstract protected void RenderFooter(HtmlTextWriter w);
        abstract protected void RenderComments(HtmlTextWriter w);

        public virtual string ParsedMainText
        {
            get
            {
                if (MainText.Equals(string.Empty))
                    return string.Empty;

                return ResourceManager.ExpandReferences(MainText, Resources);
            }
        }

        public virtual List<string> ParsedFooters
        {
            get
            {
                List<string> parsed = new List<string>();

                foreach(Footer footer in Footers)
                {
                    parsed.Add(footer.CreateHTML());
                }

                return parsed;
            }
        }

        public virtual byte[] GetHashData()
        {
            List<byte[]> blocks = new List<byte[]>();

            blocks.Add(Encoding.Default.GetBytes(this.Title));
            blocks.Add(Encoding.Default.GetBytes(this.Author));
            blocks.Add(Encoding.Default.GetBytes(this.MainText));
            blocks.Add(BitConverter.GetBytes(Timestamp.Ticks));

            return Hashing.GenerateHashCode(blocks);
        }

        public override string ToString()
        {
            StringBuilder build = new StringBuilder();

            build.AppendLine("--- Post ---");
            build.AppendLine(this.Title);
            build.AppendLine(this.Timestamp.ToShortDateString());
            build.AppendLine(this.Author);
            build.AppendLine(this.Category.ToString());
            build.AppendLine(this.MainText);
            // TODO include footers & tags & cleanup formatting
            build.AppendLine("-/- Post -/-");

            return build.ToString();
        }

        public virtual void AddTag(string tag)
        {
            Tags.Add(new Tag(this.UniqueID, tag));
        }

        public virtual void AddTags(string[] tags)
        {
            foreach (string tag in tags)
                Tags.Add(new Tag(this.UniqueID, tag));
        }

        public void ForceRefreshUniqueID()
        {
            _PostID = new Guid(this.GetHashData());
        }

        public Post()
        {
            Title = string.Empty;
            Timestamp = DateTime.MinValue;
            Author = string.Empty;
            Category = PostTypes.NavCategory.None;
            MainText = string.Empty;

            Footers = new List<Embedded.Footer>();
            Resources = new List<Embedded.IResource>();
            Tags = new List<Embedded.Tag>();

            HeaderClass = "post-title";
            FooterClass = "footer";
            PreviewLength = 250;
        }

        #region Deprecated shorthand-parsing
        /*
        protected virtual string ParseText(string text)
        {
            return ParseText(text, true);
        }

        protected virtual string RemoveResources(string text)
        {
            return ParseText(text, false);
        }

        protected virtual string ParseText(string text, bool insert)
        {
            string parsed = text;

            // replace occurrences of +@(ResourceName) with HTML markup for the resource
            for (int i = 0; i < parsed.Length; i++)
            {
                if (parsed[i].Equals('+') && i < parsed.Length - 2)
                {
                    int replaceIndex = i;
                    if (parsed[++i].Equals('@') && parsed[++i].Equals('('))
                    {
                        string resource = string.Empty;
                        while (!parsed[++i].Equals(')'))
                            resource += parsed[i];

                        parsed = parsed.Remove(replaceIndex, i - replaceIndex + 1); // get rid of the shorthand
                        if(insert)
                            parsed = parsed.Insert(replaceIndex, GetResource(resource).CreateHTML()); // insert the full HTML
                    }
                }
            }

            return parsed;
        }

        protected virtual int NextResourceIndex(string text)
        {

        }

        protected virtual List<string> ResourceNames(string text)
        {

        }

        private IResource GetResource(string resourceName)
        {
            foreach(IResource r in this.Resources)
            {
                if (r.Name.Equals(resourceName))
                    return r;
            }

            return null;
        }
        */
        #endregion
    }
}
