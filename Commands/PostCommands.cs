using PushPost.ViewModels;
using System;
using System.Windows.Input;

namespace PushPost.Commands
{
    internal abstract class PostCommands : ICommand
    {
        protected PostViewModel _ViewModel;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    internal class SubmitPostCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public SubmitPostCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanSubmitPost;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SubmitNow();
        } 
    }

    internal class QueuePostCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public QueuePostCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanSubmitPost;
        }

        public void Execute(object parameter)
        {
            _ViewModel.QueuePostForSubmit();
        } 
    }

    internal class RemovePostCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public RemovePostCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanRemovePost;
        }

        public void Execute(object parameter)
        {
            _ViewModel.QueueForRemoval();
        } 
    }

    internal class DiscardNewPostCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public DiscardNewPostCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true; // THOUGHT Not sure always returning true is a good idea, 
                         //         but I can't think of any situation where a simple
                         //         dicard operation shouldn't be allowed to happen.
        }

        public void Execute(object parameter)
        {
            _ViewModel.Discard();
        } 
    }

    internal class SubmitQueueCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public SubmitQueueCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanSubmitPost;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SubmitQueue();
        }
    }

    internal class AddIResourceCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public AddIResourceCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanAddResource;
        }

        public void Execute(object parameter)
        {
            int startIndex = -1;

            if      (parameter is int)
                startIndex = (int)parameter;
            else if (parameter is string)
                int.TryParse((string)parameter, out startIndex);

            if (startIndex < 0)
                startIndex = 0;

            _ViewModel.OpenAddRefsDialog(startIndex);
        }
    }

    internal class AddFootnoteCommand : ICommand
    {
        private PostViewModel _ViewModel;

        public AddFootnoteCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanAddResource;
        }

        public void Execute(object parameter)
        {
            _ViewModel.OpenAddFootnoteDialog();
        }
    }
}
