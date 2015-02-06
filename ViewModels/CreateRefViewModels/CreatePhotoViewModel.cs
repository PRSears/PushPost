using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels.CreateRefViewModels
{
    // TODOh Create a batch photo add
    // TODOh Create options for pointing to web, or using relative local files

    public class CreatePhotoViewModel : IRefViewModel
    {
        public NotifyingResource Resource
        {
            get;
            set;
        }

        public Photo Photo
        {
            get
            {
                return Resource as Photo;
            }
            set
            {
                Resource = value;
            }
        }
        
        public CreatePhotoViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
            Resource = new Photo();
        }

        public void Save(string filename)
        {
            ResourceSerializer serializer = new ResourceSerializer();
            this.Save(serializer);
        }

        public void Save(ResourceSerializer serializer)
        {
            serializer.Save(Resource);
        }
    }
}
