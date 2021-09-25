using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization;
using System.Windows.Data;

namespace Riscks.Converters
{
    public class ColorFromPaletteConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionaryCollection listeCouleurs = parameter as ResourceDictionaryCollection;
            int indice = (int)value;
            return listeCouleurs[indice % listeCouleurs.Count]["Background"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
