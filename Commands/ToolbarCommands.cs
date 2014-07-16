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

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.MenuToolbarCanExecute;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.ImportFromFile();
        }        
    }

    internal class ExportToFileCommand : PostCommands
    {
        public ExportToFileCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.MenuToolbarCanExecute;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.ExportToFile();
        }
    }

    internal class PreviewInBrowserCommand : PostCommands
    {
        public PreviewInBrowserCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.MenuToolbarCanExecute;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.PreviewInBrowser();
        }
    }

    internal class OpenArchiveManagerCommand : PostCommands
    {
        public OpenArchiveManagerCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.MenuToolbarCanExecute;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.OpenArchiveManager();
        }
    }

    internal class OpenPageGeneratorCommand : PostCommands
    {
        public OpenPageGeneratorCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.MenuToolbarCanExecute;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.OpenPageGenerator();
        }
    }

    internal class ViewReferencesCommand : PostCommands
    {
        public ViewReferencesCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.HasReferences;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.ViewReferences();
        }
    }

    internal class ViewFootnotesCommand : PostCommands
    {
        public ViewFootnotesCommand(PostViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        public override bool CanExecute(object parameter)
        {
            return _ViewModel.HasFootnotes;
        }

        public override void Execute(object parameter)
        {
            _ViewModel.ViewFootnotes();
        }
    }
}
