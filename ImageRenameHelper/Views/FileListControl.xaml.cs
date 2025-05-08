using System.Windows;

namespace ImageRenameHelper.Views
{
    public partial class FileListControl
    {
        public readonly static DependencyProperty ImageAlignmentProperty =
            DependencyProperty.Register(
                nameof(ImageAlignment),
                typeof(HorizontalAlignment),
                typeof(FileListControl),
                new PropertyMetadata(HorizontalAlignment.Center));

        public FileListControl()
        {
            InitializeComponent();
        }

        public HorizontalAlignment ImageAlignment
        {
            get => (HorizontalAlignment)GetValue(ImageAlignmentProperty);
            set => SetValue(ImageAlignmentProperty, value);
        }
    }
}