using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Spec.Sniffer_WPF.Converters
{
    internal class StringListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
                return string.Join("/",value as List<string>);
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
