using PushPost.Models.HtmlGeneration;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PushPost.ViewModels.ArchivesViewModels
{
    interface IArchiveViewModel
    {
        ObservableCollection<CheckablePost> DisplayedPosts { get; set; }

        void RefreshCollection(Queue<Post> posts);
    }
}
