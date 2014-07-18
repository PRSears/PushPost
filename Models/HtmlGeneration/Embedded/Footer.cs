using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using Extender.Databases;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    /// <remarks>
    /// IResource implementation for creating HTML of a footer embedded in a post.
    /// </remarks>
    [Table(Name = "Footnotes")]
    public class Footer : NotifyingResource, IStorable
    {
        private Guid _UID;
        /// <summary>
        /// Gets a Guid based on the hashed properties of this Footer.
        /// </summary>
        [Column(IsPrimaryKey=true, Storage="_UID")]
        public Guid UniqueID
        {
            get
            {
                if (_UID == null || _UID.Equals(Guid.Empty))
                    this._UID = new Guid(this.GetHashData());

                return this._UID;
            }
        }

        #region Boxed properties

        private string _Class;
        private Guid   _PostID;
        
        #endregion

        /// <summary>
        /// Parent Post's Guid.
        /// </summary>
        [Column]
        public Guid PostID 
        {
            get
            {
                return _PostID;
            }
            set
            {
                _PostID = value;
                OnPropertyChanged("PostID");
            }
        }

        /// <summary>
        /// Full text (including any HTML) contained in this footer.
        /// </summary>
        [Column(Storage="_Value")]
        public override string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// The class name for the div this footer is enclosed in.
        /// </summary>
        public string Class
        {
            get
            {
                return _Class;
            }
            set
            {
                _Class = value;
                OnPropertyChanged("Class");
            }
        }

        private DateTime CreationTime;

        /// <summary>
        /// Renders the HTML for a footer that does NOT contain any IResource references.
        /// </summary>
        /// <returns>The generated HTML as a string.</returns>
        public override string CreateHTML()
        {
            return this.CreateHTML(null);
        }

        /// <summary>
        /// Generates the HTML for this footnote.
        /// </summary>
        /// <param name="resources">List of any IResource objects that may be embedded in 
        /// this footer through a reference.</param>
        /// <returns>The rendered HTML for this footnote, in one string.</returns>
        public string CreateHTML(List<IResource> resources)
        {
            // TODO Double check that this is expanding correctly
            return "<p class=\"" + this.Class + "\">" + ResourceManager.ExpandReferences(Value, resources) + "</p>"; 
        }

        public Footer()
        {
            _Name           = string.Empty;
            _Value          = string.Empty;
            _PostID         = Guid.Empty;
            CreationTime    = DateTime.MinValue;

            _Class = "footer";
        }

        public Footer(string name, string text, Guid postID) : this()
        {
            _Name           = name;
            _Value          = text;
            _PostID         = postID;
            CreationTime    = DateTime.Now;

            _UID = new Guid(this.GetHashData());
        }

        public Footer(string name, string text, Guid postID, string className):this(name, text, postID)
        {
            _Class = className;
        }

        /// <summary>
        /// Generates a byte array from an MD5 hash of this footer's properties.
        /// </summary>
        /// <returns></returns>
        public byte[] GetHashData()
        {
            List<byte[]> blocks = new List<byte[]>();

            blocks.Add(Encoding.Default.GetBytes(this.Name));
            blocks.Add(Encoding.Default.GetBytes(this.Value));
            blocks.Add(Encoding.Default.GetBytes(this.Class));
            blocks.Add(PostID.ToByteArray());
            blocks.Add(BitConverter.GetBytes(CreationTime.Ticks));

            return Extender.ObjectUtils.Hashing.GenerateHashCode(blocks);
        }

        public override bool Equals(object obj)
        {
            Footer other;
            if (obj is Footer)
            {
                other = (Footer)obj;
            }
            else return false;

            return (this.Name.Equals(other.Name)) &&
                   (this.Value.Equals(other.Value)) &&
                   (this.Class.Equals(other.Class)) &&
                   (this.PostID.Equals(other.PostID));
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
