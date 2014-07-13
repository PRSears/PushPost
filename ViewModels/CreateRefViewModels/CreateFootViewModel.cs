using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels.CreateRefViewModels
{
    internal class CreateFootViewModel : IRefViewModel
    {
        public NotifyingResource Resource
        {
            get;
            set;
        }

        public Footer Footer
        {
            get
            {
                return (Resource as Footer);
            }
            set
            {
                Resource = value;
            }
        }

        public void Initialize()
        {
            Resource = new Footer();
        }

        public void Save(string filename)
        {
            Models.Temp.References.Save(Resource, filename);
        }

        public CreateFootViewModel()
        {
            Initialize();
        }
    }
}