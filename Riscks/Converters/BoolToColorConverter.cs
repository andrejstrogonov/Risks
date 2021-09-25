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
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                SolidColorBrush colorBrush = new SolidColorBrush();
                switch ((bool?)value)
                {
                    case true: colorBrush.Color = (Color)ColorConverter.ConvertFromString("#b9f718"); break;
                    case false: colorBrush.Color = (Color)ColorConverter.ConvertFromString("#f72d18"); break;
                    case null: colorBrush.Color = (Color)ColorConverter.ConvertFromString("#939393"); break;
                }
                return colorBrush;
            }
            catch(Exception)
            {
                return null; 
            }
            //color= Colors.Gray;
            //return color;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }



}
