using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL.Helpers.Converters;

public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        if (value is BO.OrderStatus options1)
        {
            switch (options1)
            {
                case BO.OrderStatus.Open:
                    return "Open";
                case BO.OrderStatus.Close:
                    return "Client Refused Accept";
                case BO.OrderStatus.InProgress:
                    return "In Progress";
                case BO.OrderStatus.Completed:
                    return "Completed";
                case BO.OrderStatus.Canceled:
                    return "Canceled";
                default:
                    return options1.ToString();
            }
        }

        else if (value is BO.ScheduleStatus options2)
        {
            switch (options2)
            {
                case BO.ScheduleStatus.OnTime:
                    return "On Time";
                case BO.ScheduleStatus.Late:
                    return "Late";
                case BO.ScheduleStatus.InRisk:
                    return "In Risk";
                case BO.ScheduleStatus.Unknown:
                    return "Unknown";
                default:
                    return options2.ToString();
            }
        }

        else if (value is PL.OrderOptions options3)
        {
            switch (options3)
            {
                case PL.OrderOptions.OrderType:
                    return "Order Type";
                case PL.OrderOptions.ScheduleStatus:
                    return "Schedule Status";
                case PL.OrderOptions.OrderStatus:
                    return "Order Status";
                default:
                    return options3.ToString();
            }
        }

         return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}