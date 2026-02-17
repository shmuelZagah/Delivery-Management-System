using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BO; 

namespace PL.Helpers.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.ScheduleStatus status1)
        {
            switch (status1)
            {
                case BO.ScheduleStatus.OnTime:
                    return Brushes.LightGreen;
                case BO.ScheduleStatus.Late:
                    return Brushes.Red;
                case BO.ScheduleStatus.InRisk:
                    return Brushes.Orange;
                case BO.ScheduleStatus.Unknown:
                    return Brushes.Gray;
                default:
                    return Brushes.White;
            }
        }

        if (value is BO.OrderStatus status2)
        {
            switch (status2)
            {
                case BO.OrderStatus.Completed:
                    return Brushes.LightGreen;
                case BO.OrderStatus.InProgress:
                    return Brushes.GreenYellow;
                case BO.OrderStatus.Canceled:
                    return Brushes.Red;
                case BO.OrderStatus.Open:
                    return Brushes.Orange;
                case BO.OrderStatus.Close:
                    return Brushes.IndianRed;
                default:
                    return Brushes.White;
            }
        }
        return Brushes.White;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}