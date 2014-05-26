using PushPost.ClientSide.HtmlGenerators;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Extender.ObjectUtils;
using System.Text;
using System.Linq;
using System;

namespace PushPost.ClientSide.Database
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
		[Column(Storage = "_MainText")]
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
		[Column(Storage = "_Timestamp")]
		public DateTime Timestamp
		{
			get
			{
				return _Timestamp;
			}
			private set
			{
				_Timestamp = value;
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

		public List<PushPost.ClientSide.HtmlGenerators.Embedded.Footer> Footers
		{
			get;
			set;
		}

		public List<PushPost.ClientSide.HtmlGenerators.Embedded.Tag> Tags
		{
			get;
			set;
		}

		public PostTableLayer()
		{
			Title = string.Empty;
			Timestamp = DateTime.MinValue;
			Author = string.Empty;
			PostCategory = PushPost.ClientSide.HtmlGenerators.PostTypes.NavCategory.None.ToString();
			MainText = string.Empty;

			Footers = new List<PushPost.ClientSide.HtmlGenerators.Embedded.Footer>();
			Tags = new List<PushPost.ClientSide.HtmlGenerators.Embedded.Tag>();
		}

		public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category):this()
		{
			Title = title;
			Timestamp = timestamp;
			Author = author;
			PostCategory = category;
			MainText = mainText;
		}

		public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.ClientSide.HtmlGenerators.Embedded.Footer> footers)
			: this(title, author, mainText, timestamp, category)
		{
			Footers = footers;
		}

		public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.ClientSide.HtmlGenerators.Embedded.Tag> tags)
			: this(title, author, mainText, timestamp, category)
		{
			Tags = tags;
		}

		public PostTableLayer(string title, string author, string mainText, DateTime timestamp, string category, List<PushPost.ClientSide.HtmlGenerators.Embedded.Footer> footers, List<PushPost.ClientSide.HtmlGenerators.Embedded.Tag> tags)
			: this(title, author, mainText, timestamp, category, tags)
		{
			Footers = footers;
		}

		public PostTableLayer(Post post) : this()
		{
			this.Title = post.Title;
			this.Timestamp = post.Timestamp;
			this.Author = post.Author;
			this.PostCategory = post.Category.ToString();
			this.MainText = post.MainText;

			this.Footers = post.Footers;
			this.Tags = post.Tags;
		}

		public static PostTableLayer FromPost(Post post)
		{
			PostTableLayer constructed = new PostTableLayer();

			constructed.Title = post.Title;
			constructed.Timestamp = post.Timestamp;
			constructed.Author = post.Author;
			constructed.PostCategory = post.Category.ToString();
			constructed.MainText = post.MainText;

			constructed.Footers = post.Footers;
			constructed.Tags = post.Tags;

			return constructed;
		}

		public void ExportTo(ref Post post)
		{
			post.Title = this.Title;
			post.Timestamp = this.Timestamp;
			post.Author = this.Author;
			post.Category = PushPost.ClientSide.HtmlGenerators.PostTypes.NavCategory.Parse(this.PostCategory);
			post.MainText = this.MainText;

			post.Footers = this.Footers;
			post.Tags = this.Tags;

			post.ForceRefreshUniqueID();
		}

		public static void ConvertToPost(PostTableLayer layer, ref Post post)
		{
			post.Title = layer.Title;
			post.Timestamp = layer.Timestamp;
			post.Author = layer.Author;
			post.Category = PushPost.ClientSide.HtmlGenerators.PostTypes.NavCategory.Parse(layer.PostCategory);
			post.MainText = layer.MainText;

			post.Footers = layer.Footers;
			post.Tags = layer.Tags;

			post.ForceRefreshUniqueID();
		}

		public byte[] GetHashData()
		{
			List<byte[]> blocks = new List<byte[]>();

			foreach (PushPost.ClientSide.HtmlGenerators.Embedded.Footer footer in this.Footers)
				blocks.Add(Encoding.Default.GetBytes(footer.Value));
			foreach (PushPost.ClientSide.HtmlGenerators.Embedded.Tag tag in this.Tags)
				blocks.Add(Encoding.Default.GetBytes(tag.Text));
			blocks.Add(Encoding.Default.GetBytes(this.Title));
			blocks.Add(Encoding.Default.GetBytes(this.Author));
			blocks.Add(Encoding.Default.GetBytes(this.MainText));
			blocks.Add(BitConverter.GetBytes(Timestamp.Ticks));

			return Hashing.GenerateHashCode(blocks);
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
				(this.Tags.Except(b.Tags).ToList().Count == 0)
			);
				
		}

		public override string ToString()
		{
			StringBuilder build = new StringBuilder();

			build.AppendLine("--- PostTableLayer ---");
			build.AppendLine(this.Title);
			build.AppendLine(this.Timestamp.ToShortDateString());
			build.AppendLine(this.Author);
			build.AppendLine(this.PostCategory);
			build.AppendLine(this.MainText);
			// TODO_ include footers & tags & cleanup formatting
			build.AppendLine("-/- PostTableLayer -/-");

			return build.ToString();
		}

	}
}
