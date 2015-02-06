using Extender.Databases;
using System;
using System.Data.Linq.Mapping;
using System.IO;
using System.Web.UI;


namespace PushPost.Models.HtmlGeneration.Embedded
{
    [Table(Name="Photos")]
    public class Photo : NotifyingResource, IStorable
    {
        private Guid _PhotoID;
        [Column(IsPrimaryKey=true, Storage="_PhotoID")]
        public Guid UniqueID
        {
            get
            {
                if (_PhotoID == null || _PhotoID.Equals(Guid.Empty))
                    _PhotoID = new Guid(this.GetHashData());
                return this._PhotoID;
            }
        }

        private Guid _PostID;
        [Column(Storage="_PostID")]
        public Guid PostID
        {
            get
            {
                return _PostID;
            }
            set
            {
                _PostID = value;
                _PhotoID = new Guid(this.GetHashData()); // re-hash if PostID changes
                OnPropertyChanged("PostID");
            }
        }

        [Column(Storage="_Name")]
        public override string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                base.Name = value;
                _PhotoID = new Guid(this.GetHashData()); // re-hash if name changes
            }
        }

        [Column(Storage="_Value")]
        public override string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                base.Value = value;
                _PhotoID = new Guid(this.GetHashData()); // re-hash if value changes
            }
        }

        public Photo()
        {
            _Name = string.Empty;
            _Value = string.Empty;
            _PostID = Guid.Empty;
        }

        public Photo(Guid postID) : this(string.Empty, string.Empty, postID) { }

        public Photo(string name, string link, Guid postID) : this()
        {
            _Name = name;
            _Value = link;
            _PostID = PostID;
        }

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

        public override string ToString()
        {
            return string.Format
                (
                    "[PhotoID {0} | ParentID {3} ] ({1}) {2}",
                    this.UniqueID.ToString(),
                    this.Name,
                    this.Value,
                    this.PostID.ToString()
                );
        }

        public override bool Equals(object obj)
        {
            Photo other;

            if (obj is Photo) other = (Photo)obj;
            else return false;

            return (this.GetHashCode().Equals(other.GetHashCode()));
        }

        public override byte[] GetHashData()
        {
            return Extender.ObjectUtils.Hashing.GenerateHashCode
                (
                    new byte[][] { this.PostID.ToByteArray(), base.GetHashData() }
                );            
        }

        public void ForceNewUniqueID()
        {
            _PhotoID = new Guid(this.GetHashData());
        }
    }
}
