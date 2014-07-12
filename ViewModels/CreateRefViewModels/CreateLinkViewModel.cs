using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels.CreateRefViewModels
{
    internal class CreateLinkViewModel : IRefViewModel
    {
        public NotifyingResource Resource
        {
            get;
            set;
        }

        public Link Link
        {
            get
            {
                return (Resource as Link);
            }
            set
            {
                Resource = value;
            }
        }

        public void Initialize()
        {
            Resource = new Link();
        }

        public void Save(string filename)
        {
            Models.Temp.References.Save(Resource, filename);
        }

        public CreateLinkViewModel()
        {
            Initialize();
        }
    }
}