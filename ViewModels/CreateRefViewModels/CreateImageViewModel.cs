using PushPost.Models.HtmlGeneration.Embedded;
using System;
using System.Windows.Input;

namespace PushPost.ViewModels.CreateRefViewModels
{
    internal class CreateImageViewModel : IRefViewModel
    {
        public NotifyingResource Resource
        {
            get;
            set;
        }

        protected Uri DefaultImage
        {
            get
            {
                return new Uri("/img/noimg.png", UriKind.Relative);
            }
        }

        public InlineImage Image
        {
            get
            {
                return (Resource as InlineImage);
            }
            set
            {
                Resource = value;
            }
        }

        public bool CanOpenFileBrowser
        {
            get
            {
                return true;
            }
        }

        public ICommand BrowseForImageCommand
        {
            get;
            private set;
        }

        public CreateImageViewModel()
        {
            Initialize();

            BrowseForImageCommand = new Extender.WPF.RelayCommand
            (
                () => this.OpenFileBrowser(),
                () => this.CanOpenFileBrowser
            );
        }

        public void Initialize()
        {
            Resource = new InlineImage();
            Resource.Value = DefaultImage.ToString();
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

        public void OpenFileBrowser()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.DefaultExt   = ".jpg";
            dialog.Filter       = "Image files (*.jpg, *.jpeg, *.bmp, *.png, *.gif, *.tiff)" +
                                  "|*.jpg;*.jpeg;*.bmp;*.png;*.gif;*.tiff";

            Nullable<bool> result = dialog.ShowDialog();

            if(result == true)
                Image.Value = dialog.FileName;
        }
    }
}