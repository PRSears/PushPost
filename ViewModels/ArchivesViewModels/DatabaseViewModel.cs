using Extender;
using Extender.Date;
using Extender.Debugging;
using Extender.WPF;
using PushPost.Models.Database;
using PushPost.Models.HtmlGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PushPost.ViewModels.ArchivesViewModels
{
    internal class DatabaseViewModel : ViewModel, IArchiveViewModel
    {

        public string[] SearchFieldOptions
        {
            get;
            private set;
        }

        public string SelectedSearchOption 
        {
            get
            {
                return _SelectedSearchOption;
            }
            set
            {
                _SelectedSearchOption = value;
                OnPropertyChanged("SelectedSearchOption");
            }
        }

        public string SearchField
        {
            get
            {
                return _SearchField;
            }
            set
            {
                _SearchField = value;
                OnPropertyChanged("SearchField");
            }
        }
        public DateTime SearchDateA
        {
            get
            {
                return _SearchDateA;
            }
            set
            {
                _SearchDateA = value;
                OnPropertyChanged("SearchDateA");
            }
        }
        public DateTime SearchDateB 
        {
            get
            {
                return _SearchDateB;
            }
            set
            {
                _SearchDateB = value;
                OnPropertyChanged("SearchDateB");
            }
        }

        public DateRange DateRange
        {
            get
            {
                return new DateRange(SearchDateA, SearchDateB);
            }
        }

        public bool UseDateRange
        {
            get
            {
                return _UseDateRange;
            }
            set
            {
                _UseDateRange = value;
                OnPropertyChanged("UseDateRange");
            }
        }
        public bool SearchWithDate 
        {
            get
            {
                return _SearchWithDate;
            }
            set
            {
                _SearchWithDate = value;
                OnPropertyChanged("SearchWithDate");

                if (_SearchWithDate == false) UseDateRange = _SearchWithDate;
            }
        }
        public ObservableCollection<CheckablePost> DisplayedPosts
        {   
            get
            {
                return _DisplayedPosts;
            }
            set
            {
                _DisplayedPosts = value;
                OnPropertyChanged("DisplayedPosts");
            }
        }

        protected ObservableCollection<CheckablePost> _DisplayedPosts;
        protected ArchiveViewModel  _Parent;
        protected string    _SelectedSearchOption;
        protected string    _SearchField;
        protected DateTime  _SearchDateA;
        protected DateTime  _SearchDateB;
        protected bool      _UseDateRange;
        protected bool      _SearchWithDate;

        public DatabaseViewModel(ArchiveViewModel parent)
        {
            this._DisplayedPosts    = new ObservableCollection<CheckablePost>();
            this.SearchFieldOptions = new string[]
            {
                "Title",
                "Content"
            };
            this._Parent = parent;
            this._SelectedSearchOption  = SearchFieldOptions[0];
            this._SearchField           = string.Empty;
            this._SearchDateA           = DateTime.Now.Subtract(new TimeSpan(0,1,0,0,0)); // an hour ago
            this._SearchDateB           = DateTime.Now.AddHours(1);                       // an hour from now
            this._UseDateRange          = true;
            this._SearchWithDate        = true;
        }

        public void RefreshCollection(Queue<Post> posts)
        {
            _DisplayedPosts = new ObservableCollection<CheckablePost>();

            foreach (Post post in posts)
            {
                DisplayedPosts.Add(new CheckablePost(post));
            }

            OnPropertyChanged("DisplayedPosts");
        }

        public void ExecuteSearch()
        {
            try
            {
                using (Archive db = new Archive())
                {
                    IEnumerable<Post> results = null;

                    if (SearchWithDate && UseDateRange)
                        results = db.TryPullPostsWhere(p => p.Timestamp.InRange(this.DateRange));
                    else if (SearchWithDate && !UseDateRange)
                        results = db.TryPullPostsWhere(p => p.Timestamp.Date.Equals(this.SearchDateA.Date));

                    if (SearchWithDate && results == null) return;

                    if (SelectedSearchOption == SearchFieldOptions[0] && SearchField != string.Empty)
                    { // refine using Title
                        if (results != null && results.Count() > 0)
                            results = results.Where(p => p.Title.Contains(SearchField));
                        else
                            results = db.TryPullPostsWhere(p => p.Title.Contains(SearchField));
                    }
                    else if (SelectedSearchOption == SearchFieldOptions[1] && SearchField != string.Empty)
                    { // refine using Content
                        if (results != null && results.Count() > 0)
                            results = results.Where(p => p.MainText.Contains(SearchField));
                        else
                            results = db.TryPullPostsWhere(p => p.MainText.Contains(SearchField));
                    }


                    if (results == null)
                        return;
                    else RefreshCollection(new Queue<Post>(
                        results.OrderByDescending(p => p.Timestamp)));
                }
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                System.Windows.Forms.MessageBox.Show
                    (e.Message, "Database exception", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        public void ExecuteNextSearch()
        {
            IEnumerable<CheckablePost> results = DisplayedPosts;

            if (SearchWithDate && UseDateRange)
                results = results.Where(cp => cp.Post.Timestamp.InRange(this.DateRange));
            else if (SearchWithDate && !UseDateRange)
                results = results.Where(cp => cp.Post.Timestamp.Date.Equals(this.SearchDateA.Date));

            if(SelectedSearchOption == SearchFieldOptions[0] && SearchField != string.Empty)
            { // refine using title
                results = results.Where(cp => cp.Post.Title.Contains(SearchField));
            }
            else if(SelectedSearchOption == SearchFieldOptions[1] && SearchField != string.Empty)
            { // refine using post content
                results = results.Where(cp => cp.Post.MainText.Contains(SearchField));
            }

            if (results.Count() < 1)
            {
                Debug.WriteMessage("Next search returned no results.", "info");
                return;
            }
            else DisplayedPosts = new ObservableCollection<CheckablePost>(results);
        }

        public static void TestHarness(ArchiveViewModel parent)
        {
            DatabaseViewModel VM = new DatabaseViewModel(parent);
            VM.SearchDateA = DateTime.Now.Subtract(TimeSpan.FromDays(2));
            VM.SearchDateB = DateTime.Now.AddDays(2);

            VM.SelectedSearchOption = VM.SearchFieldOptions[0]; // title
            VM.SearchField = "Directly Submitted Post";

            VM.UseDateRange = true;
            VM.SearchWithDate = true;

            VM.ExecuteSearch();
        }
    }
}
