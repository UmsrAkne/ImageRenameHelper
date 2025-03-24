using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace ImageRenameHelper.Converters
{
    public class DirectoryPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var directory = new DirectoryInfo((string)value);
            return $"{directory.Parent?.Name}\\{directory.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}