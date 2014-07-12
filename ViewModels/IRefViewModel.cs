using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels
{
    internal interface IRefViewModel
    {
        NotifyingResource Resource { get; set; }

        void Initialize();

        void Save(string filename);
    }
}
