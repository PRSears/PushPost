using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    [Serializable()]
    public abstract class NotifyingResource : IResource
    {
        protected string _ResourceType;
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
                OnPropertyChanged("Markup");
            }
        }

        /// <summary>
        /// Value to be inserted into this Resources HTML tag.
        /// </summary>
        public virtual string Value 
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
        
        /// <summary>
        /// A string indicating which concrete type of resource this is.
        /// </summary>
        public string ResourceType
        {
            get
            {
                return _ResourceType;
            }
            set
            {
                _ResourceType = value;
                OnPropertyChanged("ResourceType");
            }
        }

        public virtual void QuietSetResourceType(string newType)
        {
            _ResourceType = newType;
        }

        public string Markup
        {
            get
            {
                return string.Format(@"+@({0})", _Name);
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
