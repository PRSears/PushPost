using PushPost.Models.HtmlGeneration.Embedded;

namespace PushPost.ViewModels.CreateRefViewModels
{
    public class CreatePhotoViewModel : IRefViewModel
    {
        public System.Windows.Input.ICommand SwitchToBatchModeCommand { get; set; }

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
