using Extender.Debugging;
using Extender.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using PushPost.Models.Database;
using PushPost.Models.HtmlGeneration;
using PushPost.ViewModels.ArchivesViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PushPost.ViewModels.ArchivesViewModels
{
    internal class QueueViewModel : ViewModel
    {
        protected ArchiveViewModel _Parent;

        public QueueViewModel(ArchiveViewModel parent)
        {
            this._Parent = parent;
        }
    }
}
