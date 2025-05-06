using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImageToJsonTabViewModel : BindableBase
    {
        public FileListViewModel MetadataSourceListViewModel { get; } = new () { SupportedExtension = ".png", };

        public FileListViewModel MetadataTextListViewModel { get; } = new () { SupportedExtension = ".json", };

        /// <summary>
        /// Extracts prompts from current metadata source files, saves them as JSON files,
        /// and reloads the metadata text file list.
        /// </summary>
        public DelegateCommand GenerateMetaDataTextsCommand => new (() =>
        {
            if (MetadataSourceListViewModel.Files.Count == 0)
            {
                return;
            }

            IEnumerable<(string FileName, Prompt Prompt)> fs = MetadataSourceListViewModel.Files.Select(f =>
                (f.Name, MetadataUtil.ExtractMetadata(f.FullName)));

            var destDir = MetadataTextListViewModel.CurrentDirectoryPath;
            foreach (var f in fs)
            {
                var name = Path.GetFileNameWithoutExtension(f.FileName);
                MetadataUtil.SavePromptToJsonFile(f.Prompt, Path.Combine(destDir, $"{name}.json"));
            }

            MetadataTextListViewModel.LoadFiles();
        });
    }
}