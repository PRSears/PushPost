using Extender.Debugging;
using PushPost.Models.HtmlGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PushPost.Models.Database
{
    public class ArchiveQueue : INotifyPropertyChanged
    {
        //private FileSystemWatcher QueueWatcher;
        public event QueueChangedEventHandler QueueChanged;

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

            if (!Directory.Exists(StoragePath))
                 Directory.CreateDirectory(StoragePath);
        }

        protected void OnQueueChanged()
        {
            QueueChangedEventHandler handler = QueueChanged;
            if (handler != null)
                handler(this);
        }

        public void Enqueue(Post post)
        {
            using(StreamWriter outStream = File.CreateText(GetFullFilename(Count())))
            {
                post.Serialize(outStream, true);
            }

            OnQueueChanged();
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

            OnQueueChanged();
            return dequed;
        }

        public void Remove(Post post)
        {
            string[] files = GetQueuedFiles();
            for(int i = 0; i < files.Length; i++)
            {
                Post t = Post.Deserialize(files[i]);
                if (t.Equals(post))
                {
                    if (File.Exists(files[i])) File.Delete(files[i]);
                    
                    for (int n = i + 1; n < files.Length; n++)
                        File.Move(files[n], GetFullFilename(n-1));

                    OnQueueChanged();
                    return;
                }
            }
        }

        public Queue<Post> GetQueue()
        {
            Queue<Post> copy = new Queue<Post>();
            string[] queuedFiles = GetQueuedFiles();

            foreach (string file in queuedFiles)
                copy.Enqueue(Post.Deserialize(file));

            return copy;
        }

        protected string[] GetQueuedFiles()
        {
            if (!Directory.Exists(StoragePath)) return new string[]{};
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
            if (!Directory.Exists(StoragePath)) return 0;
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
                    ExceptionTools.WriteExceptionText(e, false, 
                        "ArchiveQueue.Peek() encountered an exception:");
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

    public delegate void QueueChangedEventHandler(object sender);
}
