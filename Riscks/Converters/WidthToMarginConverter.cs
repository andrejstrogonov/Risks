using Riscks.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Riscks.Converters
{
    public class WidthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //double num = double.Parse((string)value);
            //double len = (Math.Truncate(Math.Log10(num+1)+1))*(WindowConstants.LabelFontSize*3/5);
            return new Thickness(0, 0, -(double)value, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
