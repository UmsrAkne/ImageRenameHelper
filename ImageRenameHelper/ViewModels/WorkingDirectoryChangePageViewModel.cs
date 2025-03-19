using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WorkingDirectoryChangePageViewModel : BindableBase, IDialogAware
    {
        private string workingDirectoryName = string.Empty;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string WorkingDirectoryName
        {
            get => workingDirectoryName;
            set => SetProperty(ref workingDirectoryName, value);
        }

        public DelegateCommand CancelCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand ConfirmCommand => new DelegateCommand(() =>
        {
            var result = new DialogResult(ButtonResult.OK);
            result.Parameters.Add(nameof(WorkingDirectoryName), WorkingDirectoryName);

            RequestClose?.Invoke(result);
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}