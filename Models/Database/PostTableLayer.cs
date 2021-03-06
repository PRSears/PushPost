﻿using Extender.Databases;
using Extender.ObjectUtils;
using PushPost.Models.HtmlGeneration;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace PushPost.Models.Database
{
    /// <remarks>
    /// This class acts as an intermediary between any implementations of
    /// the abstract Post class and Linq to SQL functions.  
    /// 
    /// Handles converting between objects that LINQ can use to manipulate the 
    /// database, and the concrete implementations of Post.
    /// </remarks>
    [Table(Name = "Posts")]
    public class PostTableLayer : IStorable
    {
        private Guid _PostID;
        [Column(IsPrimaryKey=true, Storage="_PostID")]
        public Guid UniqueID
        {
            get
            {
                _PostID = new Guid(this.GetHashData());
                return _PostID;
            }
        }

        private string _Title;
        [Column(Storage="_Title")]
        public string Title
        {
            get
            {
                return _Title;
            }
            private set
            {
                _Title = value;
            }
        }

        private string _Author;
        [Column(Storage = "_Author")]
        public string Author
        {
            get
            {
                return _Author;
            }
            private set
            {
                _Author = value;
            }
        }

        private string _MainText;
        [Column(Storage = "_MainText", DbType = "NVARCHAR(MAX)")]
        public string MainText
        {
            get
            {
                return _MainText;
            }
            private set
            {
                _MainText = value;
            }
        }

        private DateTime _Timestamp;
        public DateTime Timestamp
        {
            get
            {
                if (_Timestamp == null || _Timestamp.Equals(DateTime.MinValue))
                {
                    _Timestamp = new DateTime(_Timestamp_Ticks);
                }

                return _Timestamp;
            }
            private set
            {
                _Timestamp = value;
            }
        }

        private long _Timestamp_Ticks;
        [Column(Storage = "_Timestamp_Ticks")]
        public long Timestamp_Ticks
        {
            get
            {
                _Timestamp_Ticks = this.Timestamp.Ticks;
                return _Timestamp_Ticks;
            }
            set
            {
                _Timestamp_Ticks = value;
                this.Timestamp = new DateTime(_Timestamp_Ticks);
            }
        }

        private string _PostCategory;
        [Column(Storage = "_PostCategory")]
        public string PostCategory
        {
            get
            {
                return _PostCategory;
            }
            private set
            {
                _PostCategory = value;
            }
        }

        public List<PushPost.Models.HtmlGeneration.Embedded.Footer> Footers
        {
            get;
            set;
        }

        public List<PushPost.Models.HtmlGeneration.Embedded.Tag> Tags
        {
            get;
            set;
        }

        public List<PushPost.Models.HtmlGeneration.Embedded.Photo> Photos
        {
            get;
            set;
        }

        public PostTableLayer()
        {
            Title           = string.Empty;
            Timestamp       = DateTime.MinValue;
            Author          = string.Empty;
            PostCategory    = PushPost.Models.HtmlGeneration.NavCategory.None.ToString();
            MainText        = string.Empty;

            Footers     = new List<PushPost.Models.HtmlGeneration.Embedded.Footer>();
            Tags        = new List<PushPost.Models.HtmlGeneration.Embedded.Tag>();
            Photos      = new List<PushPost.Models.HtmlGeneration.Embedded.Photo>();
        }

        public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category):this()
        {
            Title           = title;
            Timestamp       = timestamp;
            Author          = author;
            PostCategory    = category;
            MainText        = mainText;
        }

        public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.Models.HtmlGeneration.Embedded.Footer> footers)
            : this(title, author, mainText, timestamp, category)
        {
            Footers = footers;
        }

        public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.Models.HtmlGeneration.Embedded.Tag> tags)
            : this(title, author, mainText, timestamp, category)
        {
            Tags = tags;
        }

        public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.Models.HtmlGeneration.Embedded.Footer> footers, List<PushPost.Models.HtmlGeneration.Embedded.Tag> tags)
            : this(title, author, mainText, timestamp, category, tags)
        {
            Footers = footers;
        }

        public PostTableLayer(Post post) : this()
        {
            this.Title          = post.Title;
            this.Timestamp      = post.Timestamp;
            this.Author         = post.Author;
            this.PostCategory   = post.Category.ToString();
            this.MainText       = post.ParsedMainText;

            this.Footers    = post.Footers;
            this.Tags       = post.Tags;
            this.Photos     = post.Resources.OfType<PushPost.Models.HtmlGeneration.Embedded.Photo>()
                                            .ToList();
        }

        public static PostTableLayer FromPost(Post post)
        {
            PostTableLayer constructed = new PostTableLayer();

            constructed.Title           = post.Title;
            constructed.Timestamp       = post.Timestamp;
            constructed.Author          = post.Author;
            constructed.PostCategory    = post.Category.ToString();
            constructed.MainText        = post.ParsedMainText;

            constructed.Footers = post.Footers;
            constructed.Tags    = post.Tags;
            //constructed.Photos  = post.Resources.Where(r => r is PushPost.Models.HtmlGeneration.Embedded.Photo)
            //                                    .Cast<PushPost.Models.HtmlGeneration.Embedded.Photo>()
            //                                    .ToList();
            constructed.Photos  = post.Resources.OfType<PushPost.Models.HtmlGeneration.Embedded.Photo>()
                                                .ToList();

            return constructed;
        }

        public void ExportTo(ref Post post)
        {
            post.Title      = this.Title;
            post.Timestamp  = this.Timestamp;
            post.Author     = this.Author;
            post.Category   = NavCategory.Parse(this.PostCategory);
            post.MainText   = this.MainText;

            post.Footers    = this.Footers;

            post.Tags       = this.Tags;

            if(this.Photos != null)
                post.Resources.AddRange(this.Photos);

            post.ForceRefreshUniqueID();
        }

        /// <summary>
        /// Attempts to create a new Post object from this PostTableLayer by 
        /// checking the category and calling the appropriate Post implementation's
        /// constructor.
        /// </summary>
        /// <returns>If successful, a Post object based on this PostTableLayer is
        /// returned. Returns null if the Post implementation could not be determined.</returns>
        public Post TryCreatePost()
        {
            NavCategory thisCat = NavCategory.TryParse(this.PostCategory);
            if (thisCat == NavCategory.None) return null;

            return CreatePost(thisCat.PostType);
        }

        /// <summary>
        /// Attempts to create a new Post object from this PostTableLayer.
        /// </summary>
        /// <param name="concreteType">The Type of the concrete
        /// post implementation whose constructor should be called.</param>
        /// <returns>If successful, a Post object based on this PostTableLayer is
        /// returned. Returns null if the Post implementation could not be determined.</returns>
        public Post CreatePost(Type concreteType)
        {
            if (concreteType == null)
                return null;

            Post exported = (Post)Activator.CreateInstance(concreteType);
            this.ExportTo(ref exported);

            return exported;
        }

        /// <summary>
        /// Attempts to create a new Post object from this PostTableLayer.
        /// </summary>
        /// <param name="concreteTypeName">The name (no namespace) of the concrete
        /// post implementation whose constructor should be called.</param>
        /// <returns>If successful, a Post object based on this PostTableLayer is
        /// returned. Returns null if the Post implementation could not be determined.</returns>
        public Post CreatePost(string concreteTypeName)
        {
            return this.CreatePost(Type.GetType(string.Format(
                "{0}.{1}",
                (typeof(Post)).Namespace,
                concreteTypeName.Trim(new char[] {' ', '.'}))));
        }

        public static void ConvertToPost(PostTableLayer layer, ref Post post)
        {
            post.Title      = layer.Title;
            post.Timestamp  = layer.Timestamp;
            post.Author     = layer.Author;
            post.Category   = PushPost.Models.HtmlGeneration.NavCategory.Parse(layer.PostCategory);
            post.MainText   = layer.MainText;

            post.Footers    = layer.Footers;
            post.Tags       = layer.Tags;

            if (layer.Photos != null)
                post.Resources.AddRange(layer.Photos);

            post.ForceRefreshUniqueID();
        }

        public byte[] GetHashData()
        {
            Post converted = this.TryCreatePost();

            if (converted != null)
            {
                return converted.GetHashData();
            }
            else
            {
                List<byte[]> blocks = new List<byte[]>();

                blocks.Add(Encoding.Default.GetBytes(this.Title));
                blocks.Add(Encoding.Default.GetBytes(this.Author));
                blocks.Add(Encoding.Default.GetBytes(this.MainText));
                blocks.Add(Encoding.Default.GetBytes(this.PostCategory));
                blocks.Add(BitConverter.GetBytes(Timestamp.Ticks));

                return Hashing.GenerateHashCode(blocks);
            }
        }

        public void ForceNewUniqueID()
        {
            _PostID = new Guid(this.GetHashData());
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(this.GetHashData(), 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PostTableLayer))
                return false;

            PostTableLayer b = (PostTableLayer)obj;

            return
            (
                this.Title.Equals(b.Title) && 
                this.Author.Equals(b.Author) &&
                this.MainText.Equals(b.MainText) &&
                this.Timestamp.Equals(b.Timestamp) &&
                this.PostCategory.Equals(b.PostCategory) &&
                (this.Footers.Except(b.Footers).ToList().Count == 0) &&
                (this.Tags.Except(b.Tags).ToList().Count == 0) &&
                (this.Photos.Except(b.Photos).ToList().Count == 0)
            );
                
        }

        public override string ToString()
        {
            Post p = this.TryCreatePost();
            if (p != null)
            {
                return p.ToString();
            }
            else
            {
                StringBuilder build = new StringBuilder();

                build.AppendLine("PostTableLayer [" + this.Title + "]");
                build.AppendLine(this.UniqueID.ToString());
                build.AppendLine(this.Timestamp.ToShortDateString());
                build.AppendLine(this.Author);
                build.AppendLine(this.PostCategory);
                build.AppendLine(this.MainText);

                return build.ToString();
            }
        }

    }
}
