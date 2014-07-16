using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.Generic;
using System.ComponentModel;
using Extender.ObjectUtils;
using System.Web.UI;
using System.Text;
using System.IO;
using System;

namespace PushPost.Models.HtmlGeneration
{
    /// <summary>
    /// Abstract class representing a single post, which will become part
    /// of a Page.
    /// </summary>
    abstract public class Post : INotifyPropertyChanged
    {
        # region Boxed properties
        private string        _Title;
        private string        _Author;
        private string        _MainText;
        private DateTime      _Timestamp;
        private NavCategory   _Category;
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
        public DateTime Timestamp 
        {
            get
            {
                return _Timestamp;
            }
            set
            {
                _Timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }
        /// <summary>
        /// List of any embedded IResource objects referenced by markup 
        /// for the post. 
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public List<IResource> Resources; 
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
                    if(resource is InlineImage)
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
            blocks.Add(Encoding.Default.GetBytes(Category.ToString()));
            blocks.Add(BitConverter.GetBytes(Timestamp.Ticks));

            return Hashing.GenerateHashCode(blocks);
        }

        public override string ToString()
        {

            StringBuilder build = new StringBuilder();

            build.AppendLine("\tPost [" + this.Title + "]");
            build.AppendLine(this.UniqueID.ToString());
            build.AppendLine(this.Timestamp.ToShortDateString());
            build.AppendLine(this.Author);
            build.AppendLine(this.Category.ToString());
            build.AppendLine(this.MainText);

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

        public virtual void Serialize(StreamWriter outputStream)
        {
            // Parse everything before serialization to simplify the 
            // deserialization proccess.
            this._MainText = this.ParsedMainText;

            // Serialize Post
            System.Xml.Serialization.XmlSerializer serializer = 
                new System.Xml.Serialization.XmlSerializer(this.GetType());

            serializer.Serialize(outputStream, this);            
        }

        public static Post Deserialize(string filePath)
        {
            Type postType;

            // Attempt to determine Post implementation's type.
            using(FileStream streamedPost = new FileStream(
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

            // TODO handle attached IResource objects as well
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
