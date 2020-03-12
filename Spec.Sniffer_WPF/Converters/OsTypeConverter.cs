using System;
using System.Globalization;
using System.Windows.Data;

namespace Spec.Sniffer_WPF.Converters
{
    internal class OsTypeConverter : IValueConverter 
    {
         public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value?"Portable":"Standalone";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
            //ignored
        }
    }
}
