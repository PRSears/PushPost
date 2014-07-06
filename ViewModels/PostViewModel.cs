using System;
using PushPost.Commands;
using Extender.Debugging;
using System.Windows.Input;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration;
using PushPost.Models.HtmlGeneration.PostTypes;

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

        #region ICommands 

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the PostViewModel class, using the default 
        /// settings for a blog type post.
        /// </summary>
        public PostViewModel():this(NavCategory.Blog){}

        /// <summary>
        /// Initializes a new isntance of the PostViewModel class.
        /// </summary>
        public PostViewModel(NavCategory category)
        {
            InitPost(category);

            this.QueuePostCommand       = new QueuePostCommand(this);
            this.SubmitPostCommand      = new SubmitPostCommand(this);
            this.RemovePostCommand      = new RemovePostCommand(this);
            this.SubmitQueueCommand     = new SubmitQueueCommand(this);
            this.DiscardCommand         = new DiscardNewPostCommand(this);
        }
        private void InitPost(NavCategory category)
        {
            if (category == NavCategory.Blog || category == NavCategory.Code || category == NavCategory.Contact)
                _Post = TextPost.TemplatePost();
            else if (category == NavCategory.Photography)
                _Post = new AlbumPost();
            else
            {
                Debug.WriteMessage("PostViewModel.InitPost encountered an unknown Post category.", "warn");
                _Post = new TextPost();
            }
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

        public void SubmitNow()
        {
            // QueuePostForSubmit()
            // Queue.SubmitQueue()...
            System.Windows.Forms.MessageBox.Show("Submitted post: " + Post.ToString());
        }

        public void Discard()
        {
            InitPost(Post.Category);
            System.Windows.Forms.MessageBox.Show("Discarded post.");
        }
    }
}
