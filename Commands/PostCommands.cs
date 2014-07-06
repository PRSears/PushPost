using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PushPost.ViewModels;
using System.Windows.Input;

namespace PushPost.Commands
{
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

    internal class SubmitQueueCommand :ICommand
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
}
