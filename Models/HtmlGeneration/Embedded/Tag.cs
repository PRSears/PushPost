using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Extender.Databases;
using System.ComponentModel;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    [Table(Name="Tags")]
    public class Tag : IStorable, INotifyPropertyChanged
    {
        private Guid _TagID;
        [Column(IsPrimaryKey=true, Storage="_TagID")]
        public Guid UniqueID
        {
            get
            {
                if (_TagID == null || _TagID.Equals(Guid.Empty))
                    _TagID = new Guid(this.GetHashData());

                return this._TagID;
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
                _TagID = new Guid(this.GetHashData()); // re-hash if PostID changes
                OnPropertyChanged("PostID");
            }
        }

        private string _Text;
        [Column(Storage = "_Text")]
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text  = value;
                _TagID = new Guid(this.GetHashData()); //rehash if Text changes
                OnPropertyChanged("Text");
            }
        }

        /// <summary>
        /// Simple tag object to store post classifications in a database.
        /// </summary>
        /// <param name="postID">The Guid of the post this tag is associated with.</param>
        /// <param name="text">The text tagging the post.</param>
        public Tag(Guid postID, string text)
        {
            _PostID = postID;
            _Text = text;
        }

        public Tag()
        {
            _PostID = Guid.Empty;
            _Text   = string.Empty;
        }

        [Obsolete]
        public byte[] GetHashCode()
        {

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] name_block = Encoding.Default.GetBytes(this.Text);
            byte[] poid_block = PostID.ToByteArray();

            md5.TransformBlock(name_block, 0, name_block.Length, name_block, 0);
            md5.TransformBlock(poid_block, 0, poid_block.Length, poid_block, 0);

            return md5.Hash;
        }

        public byte[] GetHashData()
        {
            List<byte[]> blocks = new List<byte[]>();

            blocks.Add(this.PostID.ToByteArray());
            blocks.Add(Encoding.Default.GetBytes(this.Text));

            return Extender.ObjectUtils.Hashing.GenerateHashCode(blocks);
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
