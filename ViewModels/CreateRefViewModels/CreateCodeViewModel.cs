using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels.CreateRefViewModels
{
    internal class CreateCodeViewModel : IRefViewModel
    {
        public NotifyingResource Resource
        {
            get;
            set;
        }

        public Code Code
        {
            get
            {
                return (Resource as Code);
            }
            set
            {
                Resource = value;
            }
        }

        public void Initialize()
        {
            Resource = new Code();
        }

        public void Save(string filename)
        {
            Models.Temp.References.Save(Resource, filename);
        }

        public CreateCodeViewModel()
        {
            Initialize();
        }
    }
}