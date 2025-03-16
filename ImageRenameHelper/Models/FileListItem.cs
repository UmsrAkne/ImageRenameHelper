using System.IO;
using Prism.Mvvm;

namespace ImageRenameHelper.Models
{
    public class FileListItem : BindableBase
    {
        private readonly FileInfo fileInfo;

        public FileListItem(FileInfo fi)
        {
            FileInfo = fi;
        }

        public FileInfo FileInfo { get => fileInfo; private init => SetProperty(ref fileInfo, value); }

        public string Name => FileInfo?.Name;

        public string FullName => FileInfo.FullName;

        public string DirectoryName { get; set; }

        public string Extension { get; set; }
    }
}