using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Helpers.Converters; 

class InputNumericToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string input = value as string;

        if (string.IsNullOrEmpty(input)) return Brushes.White;

        if (long.TryParse(input, out long result))
        {
            return result >= 0 ? Brushes.White : Brushes.Red;
        }
        else
        {
            return Brushes.Red;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
