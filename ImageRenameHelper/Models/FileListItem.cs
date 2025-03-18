using System.IO;
using MetadataExtractor;
using MetadataExtractor.Formats.Png;
using Prism.Mvvm;

namespace ImageRenameHelper.Models
{
    public class FileListItem : BindableBase
    {
        private readonly string noMetadataMessage = "No metadata found in the PNG file.";
        private readonly FileInfo fileInfo;
        private int lineNumber;
        private int order;
        private string metaDataText = string.Empty;

        public FileListItem(FileInfo fi)
        {
            FileInfo = fi;
        }

        public FileInfo FileInfo { get => fileInfo; private init => SetProperty(ref fileInfo, value); }

        public string Name => FileInfo?.Name;

        public string FullName => FileInfo.FullName;

        public string DirectoryName { get; set; }

        public string Extension => fileInfo.Extension;

        public int LineNumber { get => lineNumber; set => SetProperty(ref lineNumber, value); }

        public int Order { get => order; set => SetProperty(ref order, value); }

        public string MetaDataText { get => metaDataText; set => SetProperty(ref metaDataText, value); }

        public void LoadMetaData()
        {
            var directories = ImageMetadataReader.ReadMetadata(FullName);

            if (!string.IsNullOrWhiteSpace(MetaDataText))
            {
                return;
            }

            foreach (var directory in directories)
            {
                if (directory is not PngDirectory pngDirectory)
                {
                    continue;
                }

                foreach (var tag in pngDirectory.Tags)
                {
                    if (tag.Name.StartsWith("Textual Data"))
                    {
                        MetaDataText += $"{tag.Name}: {tag.Description}";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(MetaDataText))
            {
                MetaDataText = noMetadataMessage;
            }
        }
    }
}