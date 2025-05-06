using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ImageRenameHelper.Models;
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

        public MainWindowViewModel()
        {
            SetupWorkingDirectories();
            SetDummies();
        }

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
            dialogService = containerProvider.Resolve<IDialogService>();
            SetupWorkingDirectories();
        }

        /// <summary>
        /// このアプリが作業中に取り扱う作業ディレクトリです。<br/>
        /// PngInfo　のファイルと、 ImageToImage の対象ファイルを入れるディレクトリ、一時的なファイル置き場の３つを格納します。
        /// </summary>
        public DirectoryInfo CurrentDirectory { get; set; }

        public string Title { get; set; } = GetAppNameWithVersion();

        public string Message { get => message; set => SetProperty(ref message, value); }

        public ImagesViewModel ImagesViewModel { get; } = new ();

        public TemporaryFilesTabViewModel TemporaryFilesTabViewModel { get; } = new ();

        public ImageToJsonTabViewModel ImageToJsonTabViewModel { get; } = new ();

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

        private static string GetAppNameWithVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return !string.IsNullOrWhiteSpace(infoVersion)
                ? $"Image Rename Helper ver:{infoVersion}"
                : "Image Rename Helper (version unknown)";
        }

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
            var metaDataSourceDir = Path.Combine(CurrentDirectory.FullName, "metadata-sources");
            var metaDataDir = Path.Combine(CurrentDirectory.FullName, "metadata");
            var temporaryDir = Path.Combine(CurrentDirectory.FullName, "temporary");

            Directory.CreateDirectory(CurrentDirectory.FullName);
            ImagesViewModel.PngInfoFileListViewModel.CurrentDirectoryPath = Directory.Exists(pngInfoDir)
                ? pngInfoDir
                : Directory.CreateDirectory(pngInfoDir).FullName;

            ImagesViewModel.PngInfoFileListViewModel.LoadFiles();

            ImagesViewModel.ImageToImageTargetFileListViewModel.CurrentDirectoryPath = Directory.Exists(imagesDir)
                ? imagesDir
                : Directory.CreateDirectory(imagesDir).FullName;

            ImagesViewModel.ImageToImageTargetFileListViewModel.LoadFiles();

            ImageToJsonTabViewModel.MetadataSourceListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(metaDataSourceDir).FullName;

            ImageToJsonTabViewModel.MetadataTextListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(metaDataDir).FullName;

            TemporaryFilesTabViewModel.TemporaryFileListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(temporaryDir).FullName;
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

            ImagesViewModel.PngInfoFileListViewModel.Files.AddRange(dummyList);
            ImagesViewModel.PngInfoFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\PngInfos\";
            ImagesViewModel.ImageToImageTargetFileListViewModel.Files.AddRange(dummyList2);
            ImagesViewModel.ImageToImageTargetFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\Pictures\";
        }
    }
}