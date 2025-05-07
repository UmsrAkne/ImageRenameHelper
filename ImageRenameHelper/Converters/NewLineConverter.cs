using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageRenameHelper.Converters
{
    public class NewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return s.Replace("\\n", "\n");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}