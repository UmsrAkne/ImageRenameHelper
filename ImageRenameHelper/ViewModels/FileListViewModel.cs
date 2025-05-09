using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FileListViewModel : BindableBase
    {
        private ObservableCollection<FileListItem> files = new ();
        private string currentDirectoryPath = string.Empty;
        private FileListItem selectedItem;
        private BitmapImage previewImageSource;
        private int selectedIndex;

        public ObservableCollection<FileListItem> Files { get => files; set => SetProperty(ref files, value); }

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

        public FileListItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if (!SetProperty(ref selectedItem, value))
                {
                    return;
                }

                if (selectedItem?.Extension.ToLower() == ".png")
                {
                    LoadImage(selectedItem.FullName);
                    selectedItem.LoadMetaData();
                }

                if (selectedItem?.Extension.ToLower() == ".json")
                {
                    selectedItem.LoadText();
                }
            }
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (!Files.Any())
                {
                    value = -1;
                    SetProperty(ref selectedIndex, value);
                    return;
                }

                if (value < 0)
                {
                    value = -1;
                }

                if (value >= Files.Count())
                {
                    value = Files.Count - 1;
                }

                SetProperty(ref selectedIndex, value);
            }
        }

        public string SupportedExtension { get; set; } = "*";

        public DelegateCommand MoveUpCommand => new (() =>
        {
            if (Files.Count == 0 || SelectedItem == null)
            {
                return;
            }

            var index = Files.IndexOf(SelectedItem);
            var item = SelectedItem;

            if (index <= 0)
            {
                return;
            }

            Files.Remove(item);
            Files.Insert(index - 1, item);
            SelectedItem = item;

            ReOrder();
        });

        public DelegateCommand MoveDownCommand => new (() =>
        {
            if (Files.Count <= 1 || SelectedItem == null)
            {
                return;
            }

            var index = Files.IndexOf(SelectedItem);
            var item = SelectedItem;

            if (index < 0 || index >= Files.Count - 1)
            {
                return;
            }

            Files.Remove(item);
            Files.Insert(index + 1, item);
            SelectedItem = item;

            ReOrder();
        });

        public DelegateCommand DeleteFileCommand => new DelegateCommand(() =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            var item = SelectedItem;
            var oldIndex = Files.IndexOf(item);
            item.FileInfo.Delete();
            Files.Remove(item);
            PreviewImageSource = null;

            ReOrder();
            if (Files.Count == 0)
            {
                return;
            }

            // 予め記録しておいたカーソル位置を復元する。
            SelectedIndex = Math.Min(oldIndex, Files.Count - 1);
            SelectedItem = Files[SelectedIndex];
        });

        public DelegateCommand CopyWorkingDirectoryPathCommand => new (() =>
        {
            Clipboard.SetText(CurrentDirectoryPath);
        });

        public DelegateCommand ReverseOrderCommand => new DelegateCommand(() =>
        {
            Files = new ObservableCollection<FileListItem>(Files.Reverse());
            ReOrder();
        });

        public DelegateCommand CopySeedCommand => new DelegateCommand(() =>
        {
            if (SelectedItem != null)
            {
                Clipboard.SetText(SelectedItem.Seed);
            }
        });

        public DelegateCommand CopyFileCommand => new DelegateCommand(() =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            FileSystemUtil.CopyFile(SelectedItem.FullName, CurrentDirectoryPath);
            LoadFiles();
        });

        /// <summary>
        /// 入力されたパスにあるファイルのリストを `Files` にロードします。
        /// </summary>
        /// <param name="directoryPath">対象のディレクトリパスです。引数を入力しない場合は `CurrentDirectoryPath` が使われます。</param>
        public void LoadFiles(string directoryPath = null)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                directoryPath = CurrentDirectoryPath;
            }

            Files.Clear();
            var fileList =
                Directory.GetFiles(directoryPath).Select(f => new FileListItem(new FileInfo(f))).ToList();

            if (SupportedExtension != "*")
            {
                fileList = fileList.Where(f => f.Extension.ToLower().EndsWith(SupportedExtension)).ToList();
            }

            Files.AddRange(fileList);
            ReOrder();

            CurrentDirectoryPath = directoryPath;

            if (Files.Count == 0)
            {
                PreviewImageSource = null;
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

        private void ReOrder()
        {
            for (var i = 0; i < Files.Count; i++)
            {
                Files[i].Order = i;
                Files[i].LineNumber = i + 1;
            }
        }
    }
}