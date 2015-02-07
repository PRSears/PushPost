using Extender.WPF;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    public class BatchPhotoAddViewModel : Extender.WPF.ViewModel
    {
        #region boxed properties
        private ObservableCollection<PhotoControl> _PhotoControls;
        #endregion
        public ObservableCollection<PhotoControl> PhotoControls
        {
            get
            {
                return _PhotoControls;
            }
            set
            {
                _PhotoControls = value;
                OnPropertyChanged("PhotoControls");
            }
        }
        protected PushPost.Models.HtmlGeneration.Post ParentPost
        {
            get;
            set;
        }

        public ICommand AddMoreCommand  { get; protected set; }
        public ICommand SubmitCommand   { get; protected set; }
        public ICommand CancelCommand   { get; protected set; }

        public string DefaultImageDescription { get; set; }

        public BatchPhotoAddViewModel(PushPost.Models.HtmlGeneration.Post parentPost)
        {
            ParentPost              = parentPost;
            DefaultImageDescription = "Enter a description";
            PhotoControls           = new ObservableCollection<PhotoControl>();

            AddMoreCommand = new RelayCommand
            (
                () => OpenAddDialog()
            );

            SubmitCommand = new RelayCommand
            (
                () =>
                {
                    // TODOh Process images to output folder

                    this.ParentPost.Resources.AddRange
                    (
                        PhotoControls.Select(pc => pc.Photo)
                    );

                    CloseCommand.Execute(null);
                },
                () => PhotoControls.Count > 0
            );

            CancelCommand = new RelayCommand
            (
                () =>
                {
                    CloseCommand.Execute(null);
                }
            );
        }

        protected void OpenAddDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = true;
            dialog.Title = "Select images to add";
            
            dialog.Filter = @"Image files (*.jpg, *.jpeg *.png, *.gif)
|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            DialogResult result = dialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                foreach(string filename in dialog.FileNames)
                {
                    AddPhotoControl(new Photo
                        (
                            DefaultImageDescription,
                            filename,
                            this.ParentPost.UniqueID
                        ));
                }
            }
        }

        public void AddPhotoControl(Photo photo)
        {
            PhotoControl newControl = new PhotoControl(photo);

            newControl.RemovingFromControls += 
                (sender) =>
                {
                    this.PhotoControls.Remove((PhotoControl)sender);
                    OnPropertyChanged("PhotoControls");
                };

            PhotoControls.Add(newControl);
        }

        public void AddPhotoControls(System.Collections.Generic.IEnumerable<Photo> photos)
        {
            foreach(Photo photo in photos)
            {
                this.AddPhotoControl(photo);
            }
        }

        private void TestHarness()
        {
            Photo p1 = new Photo("placeholder desc 1", "E:/gallery_bak/0001.jpg");
            Photo p2 = new Photo("placeholder desc 2", "E:/gallery_bak/0002.jpg");
            Photo p3 = new Photo("placeholder desc 3", "E:/gallery_bak/0003.jpg");

            AddPhotoControl(p1);
            AddPhotoControl(p2);
            AddPhotoControl(p3);
        }
    }

    public class PhotoControl
    {
        public Photo Photo { get; set; }
        public ICommand ChangeSourceCommand { get; protected set;  }
        public ICommand RemoveCommand { get; protected set; }

        public event RemoveFromControlsEventHandler RemovingFromControls;

        public PhotoControl()
        {
            ChangeSourceCommand = new RelayCommand
            (
                () => ChangeSource()
            );

            RemoveCommand = new RelayCommand
            (
                () => this.OnRemovingFromControls()
            );
        }

        public PhotoControl(Photo photo) : this()
        {
            this.Photo = photo;
        }

        public void ChangeSource()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Title = "Select a replacement image";

            dialog.Filter = @"Image files (*.jpg, *.jpeg *.png, *.gif)
|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            DialogResult result = dialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                Photo.Value = dialog.FileName;
            }
        }

        protected void OnRemovingFromControls()
        {
            RemoveFromControlsEventHandler handler = RemovingFromControls;

            if(handler != null)
                handler(this);
        }
    }

    public delegate void RemoveFromControlsEventHandler(object sender);
}
