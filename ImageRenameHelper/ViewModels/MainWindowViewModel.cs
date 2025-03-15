using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using ImageRenameHelper.Utils;
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

        [Conditional("DEBUG")]
        private void SetDummies()
        {
            var dummyList = new List<FileInfo>();
            var dummyList2 = new List<FileInfo>();

            for (var i = 0; i < 10; i++)
            {
                dummyList.Add(new FileInfo($"pngInfoImage-{i}.png"));
                dummyList2.Add(new FileInfo($"targetImage-{i}.png"));
            }

            PngInfoFileListViewModel.Files.AddRange(dummyList);
            PngInfoFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\PngInfos\";
            ImageToImageTargetFileListViewModel.Files.AddRange(dummyList2);
            ImageToImageTargetFileListViewModel.CurrentDirectoryPath = @"C:\Users\tests\Pictures\";
        }
    }
}