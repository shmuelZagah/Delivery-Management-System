using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Order
    {

        public int DeliveryId { get; }
        public OrderType OrderType { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AirDistance { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public double Volume { get; set; }
        public double Weight { get; set; }
        public bool IsFragile { get; set; }
        public DateTime OrderCreated { get; set; }

        public DateTime ExpectedArrivalTime { get; set; }
        public DateTime LastArrivalTime { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }

        public TimeSpan TimeLeftToDeadline { get; set; }

        public List<DeliveryPerOrderInList>? CouriersForOrder { get; set; }

    }
}
