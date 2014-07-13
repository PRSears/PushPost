using PushPost.ViewModels;
using System;
using System.Windows.Input;

namespace PushPost.Commands
{
    internal class ViewCreateLinkCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public ViewCreateLinkCommand(CreateRefViewModel viewModel)
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
            return _ViewModel.CanSwitchViews;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SwitchToLinkView();
        }
    }

    internal class ViewCreateCodeCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public ViewCreateCodeCommand(CreateRefViewModel viewModel)
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
            return _ViewModel.CanSwitchViews;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SwitchToCodeView();
        }
    }

    internal class ViewCreateImageCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public ViewCreateImageCommand(CreateRefViewModel viewModel)
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
            return _ViewModel.CanSwitchViews;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SwitchToImageView();
        }
    }

    internal class ViewCreateFootCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public ViewCreateFootCommand(CreateRefViewModel viewModel)
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
            return _ViewModel.CanSwitchViews;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SwitchToFooterView();
        }
    }

    internal class SaveRefCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public SaveRefCommand(CreateRefViewModel viewModel)
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
            return _ViewModel.CanSave;
        }

        public void Execute(object parameter)
        {
            _ViewModel.Save();
        }
    }

    internal class CancelRefCommand : ICommand
    {
        private CreateRefViewModel _ViewModel;

        public CancelRefCommand(CreateRefViewModel viewModel)
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
            return true;
        }

        public void Execute(object parameter)
        {
            _ViewModel.Cancel();
        }
    }

    internal class BrowseForImageCommand : ICommand
    {
        private ViewModels.CreateRefViewModels.CreateImageViewModel _ViewModel;

        public BrowseForImageCommand(
            ViewModels.CreateRefViewModels.CreateImageViewModel viewModel)
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
            return _ViewModel.CanOpenFileBrowser;
        }

        public void Execute(object parameter)
        {
            _ViewModel.OpenFileBrowser();
        }
    }
}
