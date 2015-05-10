using Extender.WPF;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    //TODOh Add ability to add from URL (ie: no organizing)
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

        public ICommand AddMoreCommand      { get; protected set; }
        public ICommand AddFromURLCommand   { get; protected set; }
        public ICommand SubmitCommand       { get; protected set; }
        public ICommand CancelCommand       { get; protected set; }

        public string DefaultImageDescription
        {
            get
            {
                return Properties.Settings.Default.DefaultImageDescription;
            }
        }
        public bool ResizeToggle
        {
            get
            {
                return Properties.Settings.Default.ResizePhotosOnSubmit;
            }
            set
            {
                Properties.Settings.Default.ResizePhotosOnSubmit = value;
                OnPropertyChanged("ResizeToggle");
                OnPropertyChanged("ResizeTo");
            }
        }

        public int ResizeTo
        {
            get
            {
                return ResizeToggle ? Properties.Settings.Default.DefaultPhotoSize : 0;
            }
            set
            {
                Properties.Settings.Default.DefaultPhotoSize = value;

                OnPropertyChanged("ResizeTo");
            }
        }

        public BatchPhotoAddViewModel(PushPost.Models.HtmlGeneration.Post parentPost)
        {
            ParentPost              = parentPost;
            PhotoControls           = new ObservableCollection<PhotoControl>();


            AddMoreCommand = new RelayCommand
            (
                () => OpenAddDialog()
            );

            AddFromURLCommand = new RelayCommand
            (
                () => OpenUrlDialog()
            );

            SubmitCommand = new RelayCommand
            (
                () =>
                {
                    BusySplash.Show
                    (
                        () =>
                        {
                            CopyAndOrganize();

                            this.ParentPost.Resources.AddRange
                            (
                                PhotoControls.Select(pc => pc.Photo)
                            );
                        },
                        @"/img/working.png"
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

        protected void OpenUrlDialog()
        {
            View.BatchPhotoUrlAddDialog dialog = new View.BatchPhotoUrlAddDialog();
            dialog.ShowDialog();

            List<Photo> photos = dialog.Photos;
            foreach (Photo photo in photos)
            {
                photo.PostID = this.ParentPost.UniqueID;
                AddPhotoControl(photo);
            }
        }

        protected void CopyAndOrganize()
        {
            if (!PushPost.Models.HtmlGeneration.Site.CheckSiteExportFolder())
                return; // cancel

            ImgProcessor processor = new ImgProcessor(Properties.Settings.Default.DefaultPhotoSize);
            processor.Organize(PhotoControls.Select(pc => pc.Photo));
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
