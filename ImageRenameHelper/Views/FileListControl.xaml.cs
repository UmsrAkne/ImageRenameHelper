using System.Windows;
using System.Windows.Controls;

namespace ImageRenameHelper.Views
{
    public partial class FileListControl : UserControl
    {
        public static readonly DependencyProperty ImageAlignmentProperty =
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