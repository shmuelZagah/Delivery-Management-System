using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PL.Helpers.Converters
{
    public class BoolToSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double targetHeight = 1;

            if (parameter != null && double.TryParse(parameter.ToString(), out double result))
            {
                targetHeight = result;
            }

            if (value is bool isUpdateMode)
            {
                if (isUpdateMode)
                {
                    return new GridLength(targetHeight, GridUnitType.Star);
                }
                else
                {
                    return new GridLength(0);
                }
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // לא בשימוש במקרה הזה
        }
    }
}
