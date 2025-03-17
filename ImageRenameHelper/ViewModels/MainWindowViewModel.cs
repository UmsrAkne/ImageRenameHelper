using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            PngInfoFileListViewModel = new FileListViewModel();
            ImageToImageTargetFileListViewModel = new FileListViewModel();

            var workingDir = new DirectoryInfo("working-directories");
            if (!workingDir.Exists)
            {
                Directory.CreateDirectory(workingDir.FullName);
            }

            CurrentDirectory = new DirectoryInfo(Path.Combine(workingDir.FullName, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")));
            var pngInfoDir = Path.Combine(CurrentDirectory.FullName, "png-info-images");
            var imagesDir = Path.Combine(CurrentDirectory.FullName, "target-images");

            Directory.CreateDirectory(CurrentDirectory.FullName);
            PngInfoFileListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(pngInfoDir).FullName;
            ImageToImageTargetFileListViewModel.CurrentDirectoryPath = Directory.CreateDirectory(imagesDir).FullName;

            // SetDummies();
        }

        /// <summary>
        /// このアプリが作業中に取り扱う作業ディレクトリです。<br/>
        /// PngInfo　のファイルと、 ImageToImage の対象ファイルを入れるディレクトリの２つを格納します。
        /// </summary>
        public DirectoryInfo CurrentDirectory { get; set; }

        public TextWrapper TextWrapper { get; set; } = new ();

        public FileListViewModel PngInfoFileListViewModel { get; }

        public FileListViewModel ImageToImageTargetFileListViewModel { get; }

        public DelegateCommand SyncFileNamesCommand => new (() =>
        {
            FileRenameUtil.RenameFiles(
                PngInfoFileListViewModel.Files.ToList(), ImageToImageTargetFileListViewModel.Files.ToList());
        });

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