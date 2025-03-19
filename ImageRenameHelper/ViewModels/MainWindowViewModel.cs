using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;
using ImageRenameHelper.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private string message;
        private int selectedIndex;
        private bool enabledCursorPositionSyncMode;

        public MainWindowViewModel()
        {
            PngInfoFileListViewModel = new FileListViewModel();
            ImageToImageTargetFileListViewModel = new FileListViewModel();

            SetupWorkingDirectories();

            // SetDummies();
        }

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
            dialogService = containerProvider.Resolve<IDialogService>();
        }

        /// <summary>
        /// このアプリが作業中に取り扱う作業ディレクトリです。<br/>
        /// PngInfo　のファイルと、 ImageToImage の対象ファイルを入れるディレクトリの２つを格納します。
        /// </summary>
        public DirectoryInfo CurrentDirectory { get; set; }

        public TextWrapper TextWrapper { get; set; } = new ();

        public string Message { get => message; set => SetProperty(ref message, value); }

        public FileListViewModel PngInfoFileListViewModel { get; }

        public FileListViewModel ImageToImageTargetFileListViewModel { get; }

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
                Message = "リネーム操作は左右のリストのファイル数が同数でなければ実行できません。";
                return;
            }

            FileRenameUtil.RenameFiles(
                PngInfoFileListViewModel.Files.ToList(), ImageToImageTargetFileListViewModel.Files.ToList());

            ImageToImageTargetFileListViewModel.LoadFiles();
            Message = string.Empty;
        });

        public DelegateCommand ToggleCursorSyncModeCommand => new (() =>
        {
            EnabledCursorPositionSyncMode = !EnabledCursorPositionSyncMode;
        });

        public DelegateCommand ShowWorkingDirectoryChangePageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WorkingDirectoryChangePage), new DialogParameters(), _ => { });
        });

        /// <summary>
        /// 作業ディレクトリを作成し、`PngInfoFileListViewModel` と `ImageToImageTargetFileListViewModel` の CurrentDirectoryPath にセットします。<br/>
        /// 作業ディレクトリのベースディレクトリは日時からネーミングされます。
        /// </summary>
        /// <param name="currentDirectoryPath">作業ディレクトリのフルパスを指定します。無指定の場合、作業ディレクトリの名前は自動決定・作成されます。</param>
        private void SetupWorkingDirectories(string currentDirectoryPath = "")
        {
            var workingDir = new DirectoryInfo("working-directories");
            if (!workingDir.Exists)
            {
                Directory.CreateDirectory(workingDir.FullName);
            }

            CurrentDirectory = currentDirectoryPath == string.Empty
                    ? new DirectoryInfo(Path.Combine(workingDir.FullName, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")))
                    : new DirectoryInfo(currentDirectoryPath);

            var pngInfoDir = Path.Combine(CurrentDirectory.FullName, "png-info-images");
            var imagesDir = Path.Combine(CurrentDirectory.FullName, "target-images");

            Directory.CreateDirectory(CurrentDirectory.FullName);
            PngInfoFileListViewModel.CurrentDirectoryPath = Directory.Exists(pngInfoDir)
                ? pngInfoDir
                : Directory.CreateDirectory(pngInfoDir).FullName;

            ImageToImageTargetFileListViewModel.CurrentDirectoryPath = Directory.Exists(imagesDir)
                ? imagesDir
                : Directory.CreateDirectory(imagesDir).FullName;
        }

        [Conditional("DEBUG")]
        private void SetDummies()
        {
            var dummyList = new List<FileListItem>();
            var dummyList2 = new List<FileListItem>();

            for (var i = 0; i < 10; i++)
            {
                dummyList.Add(new FileListItem(new FileInfo($"pngInfoImage-{i}.png")));
                dummyList2.Add(new FileListItem(new FileInfo($"targetImage-{i}.png")));
            }

            PngInfoFileListViewModel.Files.AddRange(dummyList);
            PngInfoFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\PngInfos\";
            ImageToImageTargetFileListViewModel.Files.AddRange(dummyList2);
            ImageToImageTargetFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\Pictures\";
        }
    }
}