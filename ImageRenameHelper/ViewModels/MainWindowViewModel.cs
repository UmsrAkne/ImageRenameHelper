﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BrowserController.Controllers;
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
            TemporaryFileListViewModel = new FileListViewModel();

            SetupWorkingDirectories();

            SetDummies();
        }

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
            dialogService = containerProvider.Resolve<IDialogService>();

            PngInfoFileListViewModel = new FileListViewModel();
            ImageToImageTargetFileListViewModel = new FileListViewModel();
            TemporaryFileListViewModel = new FileListViewModel();

            SetupWorkingDirectories();
        }

        /// <summary>
        /// このアプリが作業中に取り扱う作業ディレクトリです。<br/>
        /// PngInfo　のファイルと、 ImageToImage の対象ファイルを入れるディレクトリ、一時的なファイル置き場の３つを格納します。
        /// </summary>
        public DirectoryInfo CurrentDirectory { get; set; }

        public TextWrapper TextWrapper { get; set; } = new ();

        public string Message { get => message; set => SetProperty(ref message, value); }

        public FileListViewModel PngInfoFileListViewModel { get; }

        public FileListViewModel ImageToImageTargetFileListViewModel { get; }

        public FileListViewModel TemporaryFileListViewModel { get; }

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
            dialogService.ShowDialog(nameof(WorkingDirectoryChangePage), new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    SetupWorkingDirectories(
                        result.Parameters.GetValue<string>(
                            nameof(WorkingDirectoryChangePageViewModel.WorkingDirectoryName)));
                }
            });
        });

        public DelegateCommand BrowserControlCommand => new DelegateCommand(() =>
        {
            var pvm = PngInfoFileListViewModel;
            var ivm = ImageToImageTargetFileListViewModel;

            if (pvm.Files.Count != ivm.Files.Count || pvm.Files.Count + ivm.Files.Count == 0)
            {
                Message = "このコマンドを実行するには、２つの作業ディレクトリにファイルが一つ以上存在し、更に同数である必要があります。";
                return;
            }

            I2IController.SetupBatchFromDirectory(
                pvm.CurrentDirectoryPath, ivm.CurrentDirectoryPath);
        });

        /// <summary>
        /// 作業ディレクトリを作成し、`PngInfoFileListViewModel` と `ImageToImageTargetFileListViewModel` の CurrentDirectoryPath にセットします。<br/>
        /// 作業ディレクトリのベースディレクトリは日時からネーミングされます。
        /// </summary>
        /// <param name="currentDirectoryName">作業ディレクトリのフルパスを指定します。無指定の場合、作業ディレクトリの名前は自動決定・作成されます。</param>
        private void SetupWorkingDirectories(string currentDirectoryName = "")
        {
            var workingDir = new DirectoryInfo("working-directories");
            if (!workingDir.Exists)
            {
                Directory.CreateDirectory(workingDir.FullName);
            }

            var dirName = string.IsNullOrEmpty(currentDirectoryName)
                ? DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")
                : currentDirectoryName;

            CurrentDirectory = new DirectoryInfo(Path.Combine(workingDir.FullName, dirName));

            var pngInfoDir = Path.Combine(CurrentDirectory.FullName, "png-info-images");
            var imagesDir = Path.Combine(CurrentDirectory.FullName, "target-images");
            var temporaryDir = Path.Combine(CurrentDirectory.FullName, "temporary");

            Directory.CreateDirectory(CurrentDirectory.FullName);
            PngInfoFileListViewModel.CurrentDirectoryPath = Directory.Exists(pngInfoDir)
                ? pngInfoDir
                : Directory.CreateDirectory(pngInfoDir).FullName;

            PngInfoFileListViewModel.LoadFiles();

            ImageToImageTargetFileListViewModel.CurrentDirectoryPath = Directory.Exists(imagesDir)
                ? imagesDir
                : Directory.CreateDirectory(imagesDir).FullName;

            ImageToImageTargetFileListViewModel.LoadFiles();

            TemporaryFileListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(temporaryDir).FullName;
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