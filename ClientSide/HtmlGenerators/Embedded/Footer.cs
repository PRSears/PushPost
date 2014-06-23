using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using Extender.Databases;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    /// <remarks>
    /// IResource implementation for creating HTML of a footer embedded in a post.
    /// </remarks>
    [Table(Name = "Footnotes")]
    public class Footer : IResource, IStorable
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

        /// <summary>
        /// Parent Post's Guid.
        /// </summary>
        [Column]
        public Guid PostID
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the reference for markup.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Full text (including any HTML) contained in this footer.
        /// </summary>
        [Column]
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// The class name for the div this footer is enclosed in.
        /// </summary>
        public string Class
        {
            get;
            set;
        }

        private DateTime CreationTime;

        /// <summary>
        /// Renders the HTML for a footer that does NOT contain any IResource references.
        /// </summary>
        /// <returns>The generated HTML as a string.</returns>
        public string CreateHTML()
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
            Name = string.Empty;
            Value = string.Empty;
            PostID = Guid.Empty;
            CreationTime = DateTime.MinValue;

            this.Class = "footer";
        }

        public Footer(string name, string text, Guid postID) : this()
        {
            Name = name;
            Value = text;
            PostID = postID;
            CreationTime = DateTime.Now;

            _UID = new Guid(this.GetHashData());
        }

        public Footer(string name, string text, Guid postID, string className):this(name, text, postID)
        {
            this.Class = className;
        }

        [Obsolete]
        public byte[] GetHashCode()
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] name_block = Encoding.Default.GetBytes(this.Name);
            byte[] valu_block = Encoding.Default.GetBytes(this.Value);
            byte[] poid_block = PostID.ToByteArray();
            byte[] date_block = BitConverter.GetBytes(CreationTime.Ticks);

            md5.TransformBlock(name_block, 0, name_block.Length, name_block, 0);
            md5.TransformBlock(valu_block, 0, valu_block.Length, valu_block, 0);
            md5.TransformBlock(poid_block, 0, poid_block.Length, poid_block, 0);
            md5.TransformFinalBlock(date_block, 0, date_block.Length);

            return md5.Hash;
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
            blocks.Add(PostID.ToByteArray());
            blocks.Add(BitConverter.GetBytes(CreationTime.Ticks));

            return Extender.ObjectUtils.Hashing.GenerateHashCode(blocks);
        }
    }
}
