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
    interface IArchiveViewModel
    {
        ObservableCollection<CheckablePost> DisplayedPosts { get; set; }

        void RefreshCollection(Queue<Post> posts);
    }
}
