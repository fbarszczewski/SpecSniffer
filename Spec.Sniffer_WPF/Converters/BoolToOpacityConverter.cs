using System;
using System.Globalization;
using System.Windows.Data;

namespace Spec.Sniffer_WPF.Converters
{
    internal class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? 0.82 : 0.1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
            //ignored
        }
    }
}