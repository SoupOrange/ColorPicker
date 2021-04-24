using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ColorPicker.Converter
{
    public class ColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            var color = (SolidColorBrush)value;
         
             
            return color.ToString().Replace("#FF","#");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null)
            {
                return DependencyProperty.UnsetValue;
            }
            var result = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString().Replace("#", "#FF")));
            return result;
        }
    }
}
