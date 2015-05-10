using Extender.WPF;
using PushPost.Models.HtmlGeneration.Embedded;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace PushPost.ViewModels
{
    public class BatchUrlAddDialogViewModel : Extender.WPF.ViewModel
    {
        public ICommand OKCommand       { get; private set; }
        public ICommand CancelCommand   { get; private set; }

        public string URLs
        {
            get
            {
                return _URLs;
            }
            set
            {
                _URLs = value;
                OnPropertyChanged("URLs");
            }
        }

        private string _URLs;

        public BatchUrlAddDialogViewModel(List<Photo> photos)
        {
            this.OKCommand = new RelayCommand
            (
                () =>
                {
                    //System.Windows.SplashScreen splash = new System.Windows.SplashScreen(@"/img/working.png");
                    //splash.Show(false);

                    BusySplash.Show
                    (
                        () =>
                        {
                            using(StringReader reader = new StringReader(URLs))
                            {
                                while (reader.Peek() != -1)
                                {
                                    photos.Add
                                    (
                                        new Photo(string.Empty, reader.ReadLine())
                                    );
                                }
                            }
                        },
                        @"/img/working.png"
                    );


                    //splash.Close(System.TimeSpan.FromMilliseconds(200));

                    CloseCommand.Execute(null);
                }
            );

            this.CancelCommand = new RelayCommand
            (
                () => CloseCommand.Execute(null)
            );
        }
    }
}
