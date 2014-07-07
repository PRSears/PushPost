using System;
using PushPost.Commands;
using Extender.Debugging;
using System.Windows.Input;
using System.Collections.Generic;
using PushPost.Models.HtmlGeneration.Embedded;
using PushPost.Models.HtmlGeneration.PostTypes;

namespace PushPost.ViewModels
{
    internal class NewReferenceViewModel
    {
        private IResource _Resource;
        public IResource Resource
        {
            get
            {
                return this._Resource;
            }
        }

        /// <summary>
        /// Initializes a new instance of the NewReferenceViewModel class.
        /// </summary>
        /// <param name="type">
        /// Type of the IResource implementation being constructed.
        /// </param>
        public NewReferenceViewModel(Type type)
        {
            // HACK Doesn't seem like the best way of going about this...

            if      (type == typeof(InlineImage))
                _Resource = new InlineImage();
            else if (type == typeof(Code))
                _Resource = new Code();
            else if (type == typeof(Footer))
                _Resource = new Footer();
            else if (type == typeof(Link))
                _Resource = new Link();
            else
                throw new ArgumentException("resourceType is not an acceptable type.");
        }
        
        /// <summary>
        /// Initializes a new instance of the NewReferenceViewModel class.
        /// </summary>
        /// <param name="boundResource">The IResource to bind the view with.</param>
        public NewReferenceViewModel(IResource boundResource)
        {
            this._Resource = boundResource;
        }

        /// <summary>
        /// Initializes a new instance of the NewReferenceViewModel class, using the default
        /// settings for a new Code reference.
        /// </summary>
        public NewReferenceViewModel():this(typeof(Code))
        {
        }
    }
}
