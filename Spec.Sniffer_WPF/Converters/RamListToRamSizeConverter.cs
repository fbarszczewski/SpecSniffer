using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SpecSniffer.Model.Spec;

namespace Spec.Sniffer_WPF.Converters
{
    internal class RamListToRamSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
                return $"{((List<Memory>)value).Sum(x=>x.Size)}GB {string.Join(" ", ((List<Memory>)value).Select(x => $"[{x.Size}]"))}";
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
