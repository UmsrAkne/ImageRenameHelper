using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ImageRenameHelper.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace ImageRenameHelper.Behaviors
{
    public class FileDropBehavior : Behavior<ContentControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewDragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDragOver -= OnDragOver;
            AssociatedObject.Drop -= OnDrop;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] { Length: > 0, } files)
            {
                return;
            }

            if (AssociatedObject.DataContext is not FileListViewModel viewModel
                || string.IsNullOrEmpty(viewModel.CurrentDirectoryPath))
            {
                return;
            }

            foreach (var file in files)
            {
                try
                {
                    var destFile = Path.Combine(viewModel.CurrentDirectoryPath, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            viewModel.LoadFiles();
        }
    }
}