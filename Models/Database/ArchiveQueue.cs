using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration;
using Extender.Debugging;

namespace PushPost.Models.Database
{
    public class ArchiveQueue
    {
        /// <summary>
        /// Path to the folder containing queued Posts.
        /// </summary>
        public string StoragePath
        {
            get;
            set;
        }

        /// <summary>
        /// Format string to use as the naming scheme of stored (and queued) Posts.
        /// eg: <code>@"{0}//{1}_queued_post.xml</code>
        /// </summary>
        public string FilenameFormat
        {
            get;
            set;
        }


        public ArchiveQueue() : this(Properties.Settings.Default.QueueFolderPath) { }

        public ArchiveQueue(string storagePath)
        {
            this.StoragePath    = storagePath;
            this.FilenameFormat = @"{0}//{1}_queued_post.xml";
        }

        public void Enqueue(Post post)
        {
            using(StreamWriter outStream = File.CreateText(GetFullFilename(Count())))
            {
                post.Serialize(outStream);
            }
        }

        public Post Dequeue()
        {
            if (Count() < 1)
                return null;

            Post dequed = Post.Deserialize(GetFullFilename(0));

            if (File.Exists(GetFullFilename(0)))
                File.Delete(GetFullFilename(0));

            string[] queuedFiles = GetQueuedFiles();
            for (int i = 0; i < queuedFiles.Length; i++)
            {
                File.Move(queuedFiles[i], GetFullFilename(i));
            }

            return dequed;
        }

        protected string[] GetQueuedFiles()
        {
            return Directory.GetFiles(StoragePath, "*.xml");
        }

        /// <summary>
        /// Gets the full filename - including subfolder path - of the queued post
        /// at the specified position. 
        /// </summary>
        /// <param name="position">Position of the queued post to generate filename
        /// for.</param>
        /// <returns>
        /// The full filename generated using this.StoragePath and
        /// this.FilenameFormat.
        /// </returns>
        protected string GetFullFilename(int position)
        {
            return string.Format(
                FilenameFormat,
                StoragePath,
                position.ToString("D3"));
        }

        /// <summary>
        /// Counts all queued Post files found at this.StoragePath
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Directory.EnumerateFiles(StoragePath, "*.xml").Count();
        }

        public Post Peek()
        {
            if (Count() < 1)
                return null;

            try
            {
                return Post.Deserialize(GetFullFilename(0));
            }
            catch (Exception e)
            {
                if(DEBUG)
                {
                    Debug.WriteMessage("ArchiveQueue.Peek() encountered an exception:");
                    Extender.Exceptions.ExceptionTools.WriteExceptionText(e, false);
                }
                return null;
            }
        }

        private bool DEBUG
        {
            get
            {
                return Properties.Settings.Default.DEBUG;
            }
        }

        public static void TestHarness()
        {
            ArchiveQueue aq = new ArchiveQueue();

            aq.Enqueue(TextPost.Dummy());
            aq.Enqueue(TextPost.Dummy());
            aq.Enqueue(TextPost.Dummy());

            if (!Extender.WPF.ConfirmationDialog.Show("Continute?", "Test dequeuing?"))
                return;

            int i = 0;
            List<Post> retrieved = new List<Post>();
            while(aq.Peek() != null)
            {
                retrieved.Add(aq.Dequeue());
                Debug.WriteMessage("Dequed post " + i++);
                System.Threading.Thread.Sleep(2000);
            }

            Debug.WriteMessage(string.Format("Retrieved {0} items.", retrieved.Count), "info");

            foreach(Post p in retrieved)
            {
                Debug.WriteMessage(p.Title, "info");
            }

            Debug.WriteMessage(string.Format("aq now has {0} elements.", aq.Count()));
        }
    }
}
