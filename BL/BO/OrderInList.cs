using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OrderInList
    {
        public int? DeliveryId { get; init; }
        public int OrderId { get; init; }
        public OrderType OrderType { get; init; }
        public double AirDistance { get; init; }
        public OrderStatus OrderStatus { get; init; }
        public ScheduleStatus ScheduleStatus { get; init; }
        public TimeSpan TimeLeftToDeadline { get; init; }
        public TimeSpan TimeLeftToFinish { get; init; }
        public int CouriersCount { get; init; }

    }
}
