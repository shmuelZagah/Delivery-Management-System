using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OrderInList
    {
        public int? DeliveryId { get; }
        public int OrderId { get; }
        public OrderType OrderType { get; }
        public double AirDistance { get; }
        public OrderStatus OrderStatus { get; }
        public ScheduleStatus ScheduleStatus { get; }
        public TimeSpan TimeLeftToDeadline { get; }
        public TimeSpan TimeLeftToFinish { get; }
        public int CouriersCount { get; }

    }
}
