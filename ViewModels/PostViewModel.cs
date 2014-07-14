using System;
using PushPost.Commands;
using Extender.Debugging;
using System.Windows.Input;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration;

namespace PushPost.ViewModels
{
    internal class PostViewModel
    {
        private Post _Post;

        public Post Post
        {
            get
            {
                return _Post;
            }
        }
        public NavCategory[] CategoriesList
        {
            get
            {
                return NavCategory.AllCategories;
            }
        }

        public ICommand SubmitPostCommand
        {
            get;
            private set;
        }
        public ICommand SubmitQueueCommand
        {
            get;
            private set;
        }
        public ICommand QueuePostCommand
        {
            get;
            private set;
        }
        public ICommand RemovePostCommand
        {
            get;
            private set;
        }
        public ICommand DiscardCommand
        {
            get;
            private set;
        }
        public ICommand AddIResourceCommand
        {
            get;
            private set;
        }
        public ICommand AddFootnoteCommand
        {
            get;
            private set;
        }

        public Action CloseAction { get; set; }

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel()
        {
            _Post = TextPost.TemplatePost();

            this.QueuePostCommand       = new QueuePostCommand(this);
            this.SubmitPostCommand      = new SubmitPostCommand(this);
            this.RemovePostCommand      = new RemovePostCommand(this);
            this.SubmitQueueCommand     = new SubmitQueueCommand(this);
            this.DiscardCommand         = new DiscardNewPostCommand(this);
            this.AddIResourceCommand    = new AddIResourceCommand(this);
            this.AddFootnoteCommand     = new AddFootnoteCommand(this);
        }

        /// <summary>
        /// Initializes a new isntance of the PostViewModel class.
        /// </summary>
        /// <param name="category">Category of Post the view is to be bound with.</param>
        public PostViewModel(NavCategory category):this()
        {
            InitializeByCategory(category);
        }

        /// <summary>
        /// Initializes a new instance of the PostViewModel class.
        /// </summary>
        /// <param name="type">Type of Post the view is to be bound with.</param>
        public PostViewModel(Type type):this()
        {
            InitializeByType(type);
        }

        private void InitializeByType(Type type)
        {
            if      (type == typeof(TextPost))
                _Post = TextPost.TemplatePost();
            else if (type == typeof(AlbumPost))
                _Post = AlbumPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }

        private void InitializeByCategory(NavCategory category)
        {
            if (category == NavCategory.Blog || category == NavCategory.Code || category == NavCategory.Contact)
                _Post = TextPost.TemplatePost();
            else if (category == NavCategory.Photography)
                _Post = AlbumPost.TemplatePost();
            else
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                // Doesn't override the default _Post the parameterless constructor set.
        }

        public bool CanSubmitPost 
        { 
            get
            {
                // TODO validate post
                return true;
            }
        }

        public bool CanSubmitQueue
        {
            get
            {
                // TODO (re)check all posts in queue
                return true;
            }
        }

        public bool CanRemovePost
        {
            get
            {
                // TODO Check to see if the post has valid Title/ID/etc
                // try Archive.remove
                return true;
            }
        }

        public bool CanAddResource
        {
            get
            {
                // TODO implement CanAddResource logic
                return true;
            }
        }

        public void QueuePostForSubmit()
        {
            System.Windows.Forms.MessageBox.Show("Post queued.");
        }

        public void QueueForRemoval()
        {
            System.Windows.Forms.MessageBox.Show("Removing post.");
        }

        public void SubmitQueue()
        {
            System.Windows.Forms.MessageBox.Show("Submitted queue");
        }

        public void OpenAddRefsDialog(int startIndex)
        {
            AddRefsDialog dialog = new AddRefsDialog(startIndex);
            dialog.Show();
        }

        public void OpenAddFootnoteDialog()
        {
            System.Windows.Forms.MessageBox.Show("Footnotes not implemented.");
        }

        public void SubmitNow()
        {
            // QueuePostForSubmit()
            // Queue.SubmitQueue()...
            System.Windows.Forms.MessageBox.Show("Submitted post: " + Post.ToString());
        }

        public void Discard()
        {
            InitializeByType(_Post.GetType());
            System.Windows.Forms.MessageBox.Show("Discarded post.");
        }
    }
}
