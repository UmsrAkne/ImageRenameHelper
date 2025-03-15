using System.Diagnostics;
using ImageRenameHelper.Utils;
using Prism.Mvvm;

namespace ImageRenameHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            SetDummies();
        }

        public TextWrapper TextWrapper { get; set; } = new ();

        [Conditional("DEBUG")]
        private void SetDummies()
        {
        }
    }
}