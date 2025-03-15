using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FileListViewModel : BindableBase
    {
        private ObservableCollection<FileInfo> files = new ();
        private string currentDirectoryPath = string.Empty;
        private FileInfo selectedItem;
        private BitmapImage previewImageSource;

        public ObservableCollection<FileInfo> Files { get => files; set => SetProperty(ref files, value); }

        public string CurrentDirectoryPath
        {
            get => currentDirectoryPath;
            set => SetProperty(ref currentDirectoryPath, value);
        }

        public BitmapImage PreviewImageSource
        {
            get => previewImageSource;
            set => SetProperty(ref previewImageSource, value);
        }

        public FileInfo SelectedItem
        {
            get => selectedItem;
            set
            {
                if (!SetProperty(ref selectedItem, value))
                {
                    return;
                }

                if (selectedItem.Extension.ToLower() == ".png")
                {
                    LoadImage(selectedItem.FullName);
                }
            }
        }

        private void LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze(); // UIスレッド以外でも安全に使うため

            PreviewImageSource = bitmap;
        }
    }
}