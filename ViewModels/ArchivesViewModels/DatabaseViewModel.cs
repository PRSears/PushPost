using Extender.Date;
using Extender.Debugging;
using Extender.WPF;
using Extender;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Models.Database;
using PushPost.Models.HtmlGeneration;
using PushPost.ViewModels.ArchivesViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PushPost.ViewModels.ArchivesViewModels
{
    internal class DatabaseViewModel : ViewModel
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

                if(!SearchDateA.Equals(DateTime.MinValue) || !SearchDateB.Equals(DateTime.MinValue))
                    _DateRange = new Extender.Date.DateRange(_SearchDateA, _SearchDateB);

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

                if (!SearchDateA.Equals(DateTime.MinValue) || !SearchDateB.Equals(DateTime.MinValue))
                    _DateRange = new Extender.Date.DateRange(_SearchDateA, _SearchDateB);

                OnPropertyChanged("SearchDateB");
            }
        }

        public DateRange DateRange
        {
            get
            {
                return _DateRange;
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

        protected ArchiveViewModel  _Parent;
        protected string    _SelectedSearchOption;
        protected string    _SearchField;
        protected DateTime  _SearchDateA;
        protected DateTime  _SearchDateB;
        protected DateRange _DateRange;
        protected bool      _UseDateRange;
        protected bool      _SearchWithDate;

        public DatabaseViewModel(ArchiveViewModel parent)
        {
            this.SearchFieldOptions = new string[]
            {
                "Title",
                "Content"
            };
            this._Parent = parent;
            this._SelectedSearchOption  = SearchFieldOptions[0];
            this._SearchField           = string.Empty;
            this._SearchDateA           = DateTime.Now;
            this._SearchDateB           = DateTime.Now;
            this._UseDateRange          = true;
            this._SearchWithDate        = true;
        }

        public void ExecuteSearch()
        {
            using (Archive db = new Archive())
            {
                IEnumerable<Post> results = null;

                if (SearchWithDate && UseDateRange)
                    results = db.TryPullPostsWhere(p => p.Timestamp.InRange(this.DateRange));
                else if(SearchWithDate && !UseDateRange)
                    results = db.TryPullPostsWhere(p => p.Timestamp.Equals(this.SearchDateA));


                if (SelectedSearchOption == SearchFieldOptions[0] && SearchField != string.Empty)
                { // refine using Title
                    if (results != null && results.Count() > 0)
                        results = results.Where(p => p.Title.Equals(SearchField));
                    else
                        results = db.TryPullPostsWhere(p => p.Title.Equals(SearchField));
                }
                else if (SelectedSearchOption == SearchFieldOptions[1] && SearchField != string.Empty)
                { // refine using Content
                    if (results != null && results.Count() > 0)
                        results = results.Where(p => p.MainText.Contains(SearchField));
                    else
                        results = db.TryPullPostsWhere(p => p.MainText.Contains(SearchField));
                }


                if (results == null)
                {
                    Debug.WriteMessage("Search returned no results.", "info");
                    return;
                }
                else _Parent.RefreshCollection(new Queue<Post>(
                             results.OrderByDescending(p => p.Timestamp)));
            }
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
