using Extender.ObjectUtils;
using PushPost.Models.HtmlGeneration.Embedded;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace PushPost.Models.HtmlGeneration
{
    /// <summary>
    /// Abstract class representing a single post, which will become part
    /// of a Page.
    /// </summary>
    abstract public class Post : INotifyPropertyChanged
    {
        # region Boxed properties
        private string          _Title;
        private string          _Author;
        private string          _MainText;
        private DateTime        _Timestamp;
        private NavCategory     _Category;
        private List<IResource> _Resources;
        #endregion

        protected Guid _PostID;
        /// <summary>
        /// Guid generated from the hash of this Post's properties.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Guid UniqueID 
        {
            get
            {
                // HACK forces an ID to get created if it doesn't exist.
                if (_PostID == null || _PostID.Equals(Guid.Empty))
                    _PostID = new Guid(GetHashData()); 
                return _PostID;
            }
            protected set
            {
                _PostID = value;
            }
        }
        /// <summary>
        /// Title of the post.
        /// </summary>
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }
        public string TitleLink
        {
            get
            {
                return string.Format(@"<a href=""{0}"">{1}</a>",
                            TitlePath,
                            this.Title);
            }
        }
        public string TitlePath
        {
            get
            {
                return Path.Combine
                    (
                        @"..\",
                        Properties.Settings.Default.SinglesSubfolder,
                        string.Format("{0}.html", this.UniqueID.ToString())
                    );
            }
        }
        /// <summary>
        /// Post's author/
        /// </summary>
        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                _Author = value;
                OnPropertyChanged("Author");
            }
        }
        /// <summary>
        /// The main (body) text contained within the post.
        /// </summary>
        public string MainText 
        {
            get
            {
                return _MainText;
            }
            set
            {
                _MainText = value;
                OnPropertyChanged("MainText");
            }
        }
        /// <summary>
        /// List of any footnotes contained within the post.
        /// </summary>
        public List<Footer> Footers; 
        /// <summary>
        /// DateTime object representing the Post's creation time.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public DateTime Timestamp 
        {
            get
            {
                if(_Timestamp == null || _Timestamp.Equals(DateTime.MinValue))
                    _Timestamp = new DateTime(Timestamp_Ticks);

                return _Timestamp;
            }
            set
            {
                _Timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }
        public long Timestamp_Ticks
        {
            get
            {
                return this.Timestamp.Ticks;
            }
            set
            {
                this.Timestamp = new DateTime(value);
            }
        }
        /// <summary>
        /// List of any embedded IResource objects referenced by markup 
        /// for the post. 
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public List<IResource> Resources
        {
            get
            {
                return _Resources;
            }
            set
            {
                _Resources = value;
                OnPropertyChanged("Resources");
            }
        }
        /// <summary>
        /// List of tags (categories/topics/etc) associated with this Post.
        /// </summary>
        public List<Tag> Tags;      
        /// <summary>
        /// Which site-category this Post is posted under.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public NavCategory Category 
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
                OnPropertyChanged("Category");
            }
        }
        public string CategoryString
        {
            get
            {
                if (this.Category.Equals(null))
                    return string.Empty;
                return Category.ToString();
            }
            set
            {
                this.Category = NavCategory.TryParse(value);
            }
        }

        /// <summary>
        /// Boolean switch to include/disclude HTML comments below the post
        /// as a separator.
        /// </summary>
        public bool IncludePostEndComments;

        protected string HeaderClass;
        protected string FooterClass;
        protected string SubHeaderID;
        protected string AuthorClass;
        protected string   DateClass;
        protected string  PostBodyID;

        /// <summary>
        /// Number of character from the MainText to include in the posts' 
        /// preview.
        /// </summary>
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
                RenderBody(writer);
                RenderFooter(writer);
                RenderComments(writer);

                return buffer.ToString();
            }

        }

        /// <summary>
        /// Generates a preview of the post containing title, author, date and an excerpt of the 
        /// first paragraph.
        /// </summary>
        public virtual string CreatePreview()
        {
            Embedded.Footer expand = new Embedded.Footer();
            expand.Name  = "__foot_expand";
            expand.Value = string.Format
                (
                    @" ... 
<a href=""{0}""><i>[Full Post]</i></a>",
                    this.TitlePath
                );

            this.MainText += expand.Markup;

            this.Footers.Add(expand);

            using(StringWriter buffer = new StringWriter())
            using(HtmlTextWriter writer = new HtmlTextWriter(buffer))
            {
                RenderHeader(writer);
                RenderPreviewBody(writer);
                RenderFooter(writer);
                RenderComments(writer);

                return buffer.ToString();
            }

            #region obsolete...
            //using (StringWriter buffer = new StringWriter())
            //using (HtmlTextWriter writer = new HtmlTextWriter(buffer))
            //{
            //    RenderHeader(writer);
            //    writer.Write(writer.NewLine);

            //    if (MainText.Length < PreviewLength)
            //        PreviewLength = MainText.Length;

            //    // Throw an image at the top from the Resources list (if present)
            //    foreach(IResource resource in Resources)
            //    {
            //        if(resource is InlineImage)
            //        {
            //            writer.Write(resource.CreateHTML());
            //            break;
            //        }
            //    }

            //    string preview = ResourceManager.RemoveReferences(MainText.Substring(0, PreviewLength)); // grab the first x characters
            //    writer.Write(preview);

            //    return buffer.ToString();
            //}
            #endregion
        }

        abstract protected void RenderHeader(HtmlTextWriter w);
        abstract protected void RenderBody(HtmlTextWriter w);
        abstract protected void RenderPreviewBody(HtmlTextWriter w);
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
            blocks.Add(Encoding.Default.GetBytes(this.ParsedMainText));
            blocks.Add(Encoding.Default.GetBytes(Category.ToString()));
            blocks.Add(BitConverter.GetBytes(Timestamp.Ticks));

            return Hashing.GenerateHashCode(blocks);
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

        public virtual void Serialize(StreamWriter outputStream, bool parse)
        {
            // Parse everything before serialization to simplify the 
            // deserialization proccess.
            if(parse)
                this._MainText = this.ParsedMainText;

            // Serialize Post
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(this.GetType());

            serializer.Serialize(outputStream, this);
        }

        public static Post Deserialize(string filePath)
        {
            Type postType;

            if (!File.Exists(filePath))
                return null;

            try
            {

                // Attempt to determine Post implementation's type.
                using (FileStream streamedPost = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    System.Xml.XmlReader r = System.Xml.XmlReader.Create(streamedPost);

                    r.MoveToContent();

                    postType = Type.GetType(string.Format(
                        "{0}.{1}",
                        typeof(Post).Namespace,
                        r.Name));

                    // default to abstract Post if type could not be determined
                    if (postType == null) postType = typeof(Post);

                    streamedPost.Position = 0;

                    System.Xml.Serialization.XmlSerializer deserializer =
                        new System.Xml.Serialization.XmlSerializer(postType);

                    return (Post)deserializer.Deserialize(streamedPost);
                }
            }
            catch (Exception e)
            {
                Extender.Debugging.ExceptionTools.WriteExceptionText(
                    e, true,
                    string.Format("Post.Deserialize could not read file at {0}.",
                    filePath));

                return null;
            }
        }

        public Post()
        {
            Title       = string.Empty;
            Author      = string.Empty;
            MainText    = string.Empty;
            Timestamp   = DateTime.MinValue;
            Category    = NavCategory.None;

            Footers     = new List<Embedded.Footer>();
            Resources   = new List<Embedded.IResource>();
            Tags        = new List<Embedded.Tag>();

            HeaderClass             = "post-title";
            FooterClass             = "footer";
            SubHeaderID             = "sub-header";
            AuthorClass             = "author";
            DateClass               = "date";
            PostBodyID              = "post-body";

            PreviewLength           = 250;
            IncludePostEndComments  = true;
        }

        public override string ToString()
        {
            Type type = this.GetType();
            StringBuilder build = new StringBuilder();

            build.AppendLine(string.Format(
                "{0} [{1}]",
                type.Name,
                this.Title));
            build.AppendLine(this.UniqueID.ToString());
            build.AppendLine(this.Timestamp.ToShortDateString());
            build.AppendLine(this.Author);
            build.AppendLine(this.Category.ToString());
            foreach (Tag t in Tags) build.AppendLine(t.ToString());
            build.AppendLine(this.MainText);
            foreach (Footer f in Footers) build.AppendLine(f.ToString());

            return build.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Post other;
            if (obj is Post)
            {
                other = (Post)obj;
            }
            else return false;

            return (this.Title.Equals(other.Title)) &&
                   (this.Author.Equals(other.Author)) &&
                   (this.Category.Equals(other.Category)) &&
                   (this.Timestamp.Equals(other.Timestamp)) &&
                   (this.ParsedMainText.Equals(other.ParsedMainText)) &&
                   (this.Footers.Except(other.Footers).Count() == 0) &&
                   (this.Tags.Except(other.Tags).Count() == 0);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
