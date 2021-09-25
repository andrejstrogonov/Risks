using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Riscks.ViewModel;
using SQLiteLibrary.DataAccess;

namespace Riscks.Converters
{
    public class BoolToIntConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool? val = (bool?)value;
                if (val is null)
                    return 1;
                if ((bool)val)
                    return 2;
                return 0;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (Math.Round((double)value))
            {
                case 0: return false;
                case 2: return true;
            }
            return null;
        }
    }
}
