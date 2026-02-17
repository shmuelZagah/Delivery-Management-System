using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Helpers.Converters
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is BO.Order)
            {
                BO.Order order = (BO.Order)value;
                if (!(order.OrderStatus == BO.OrderStatus.Open || order.OrderStatus == BO.OrderStatus.InProgress))
                {
                    return Visibility.Visible;
                }
            }

            if (value is BO.OrderStatus status1)
            {

                if (status1 == BO.OrderStatus.Open || status1 == BO.OrderStatus.InProgress)
                {
                    return Visibility.Visible;
                }


                return Visibility.Collapsed;
            }

            if (value is BO.OrderInProgress status2)
            {

                if (status2 != null)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            if (value is BO.CourierInList status3)
            {

                if (status3.CurrentOrderId == null && (status3.DeliveredOnTimeCount + status3.DeliveredLateCount) == 0)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class NullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}