using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    public abstract class NotifyingResource : IResource
    {
        protected string _Name;
        protected string _Value;

        /// <summary>
        /// Name of the reference for markup.
        /// </summary>
        public string Name 
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Value to be inserted into this Resources HTML tag.
        /// </summary>
        public string Value 
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        public abstract string CreateHTML();
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
