using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TemporaryFilesTabViewModel : BindableBase
    {
        public FileListViewModel TemporaryFileListViewModel { get; set; } = new ();
    }
}