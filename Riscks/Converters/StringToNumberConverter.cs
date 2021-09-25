using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Riscks.Converters
{
    public class StringToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*string val = value.ToString();
            string val_notspaces = string.Join("", val.Where(p=>(p>='0' && p<='9') ||(p=='.')|| (p==',')).ToList());
            double number = 0;
            if(double.TryParse(val_notspaces,out number))
            {
                return number;
            }
            return val;*/
            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString();
            string val_notspaces = val.Replace(" ", "");// не удалять, val.Replace(!!" "!!, "") символ важен
            //string val_notspaces = string.Join("", val.Where(p => (p >= '0' && p <= '9') || (p == '.') || (p == ',')).ToList());
            double number = 0;
            if (double.TryParse(val_notspaces, out number))
            {
                return number;
            }
            return val;
        }
    }
}
