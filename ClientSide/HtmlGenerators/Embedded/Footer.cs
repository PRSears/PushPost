using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    [Table(Name = "Footnotes")]
    public class Footer : IResource, Database.IStorable
    {
        private Guid _UID;
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

        [Column]
        public Guid PostID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        [Column]
        public string Value
        {
            get;
            set;
        }

        public string Class
        {
            get;
            set;
        }

        private DateTime CreationTime;

        public string CreateHTML()
        {
            return this.CreateHTML(null);
        }

        public string CreateHTML(List<IResource> resources)
        {
            // TODO Double check that this is expanding correctly
            return "<p class=\"" + this.Class + "\">" + ResourceManager.ExpandReferences(Value, resources) + @"</p>"; 
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
