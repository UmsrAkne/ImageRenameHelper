using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;
using ImTools;
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
            SetDummies();
        }

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