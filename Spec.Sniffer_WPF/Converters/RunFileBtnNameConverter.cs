using System;
using System.Globalization;
using System.Windows.Data;

namespace Spec.Sniffer_WPF.Converters
{
    internal class RunFileBtnNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"Run {value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}