using System;
using System.Linq;
using BrowserController.Controllers;
using ImageRenameHelper.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImagesViewModel : BindableBase, IMessenger
    {
        private int selectedIndex;
        private bool enabledCursorPositionSyncMode;

        public event EventHandler<string> SystemMessagePublished;

        public FileListViewModel PngInfoFileListViewModel { get; } = new () { SupportedExtension = "png", };

        public FileListViewModel ImageToImageTargetFileListViewModel { get; } = new () { SupportedExtension = "png", };

        public bool EnabledCursorPositionSyncMode
        {
            get => enabledCursorPositionSyncMode;
            set => SetProperty(ref enabledCursorPositionSyncMode, value);
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (!EnabledCursorPositionSyncMode)
                {
                    return;
                }

                PngInfoFileListViewModel.SelectedIndex = value;
                ImageToImageTargetFileListViewModel.SelectedIndex = value;
                SetProperty(ref selectedIndex, value);
            }
        }

        public DelegateCommand SyncFileNamesCommand => new (() =>
        {
            if (PngInfoFileListViewModel.Files.Count() != ImageToImageTargetFileListViewModel.Files.Count())
            {
                PublishSystemMessage("リネーム操作は左右のリストのファイル数が同数でなければ実行できません。");
                return;
            }

            FileRenameUtil.RenameFiles(
                PngInfoFileListViewModel.Files.ToList(), ImageToImageTargetFileListViewModel.Files.ToList());

            ImageToImageTargetFileListViewModel.LoadFiles();

            PublishSystemMessage(string.Empty);
        });

        public DelegateCommand ToggleCursorSyncModeCommand => new (() =>
        {
            EnabledCursorPositionSyncMode = !EnabledCursorPositionSyncMode;
        });

        public DelegateCommand BrowserControlCommand => new DelegateCommand(() =>
        {
            var pvm = PngInfoFileListViewModel;
            var ivm = ImageToImageTargetFileListViewModel;

            if (pvm.Files.Count != ivm.Files.Count || pvm.Files.Count + ivm.Files.Count == 0)
            {
                PublishSystemMessage("このコマンドを実行するには、２つの作業ディレクトリにファイルが一つ以上存在し、更に同数である必要があります。");
                return;
            }

            I2IController.SetupBatchFromDirectory(
                pvm.CurrentDirectoryPath, ivm.CurrentDirectoryPath);
        });

        private void PublishSystemMessage(string message)
        {
            SystemMessagePublished?.Invoke(this, message);
        }
    }
}