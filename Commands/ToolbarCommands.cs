using PushPost.ViewModels;
using System;
using System.Windows.Input;

namespace PushPost.Commands
{
    internal class ImportFromFileCommand : PostCommands
    {
        public ImportFromFileCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {

        }        
    }    
}
