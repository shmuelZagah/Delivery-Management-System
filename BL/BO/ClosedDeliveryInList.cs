using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ClosedDeliveryInList
    {
        public int DeliveryId { get; init; }
        public int OrderId { get; init; }
        public OrderType OrderType { get; init; }
        public string Address { get; init; } =string.Empty;
        public ShipmentType ShipmentType { get; init;    }
        public double? ActualDistance { get; init; }
        public TimeSpan ProcessingDuration { get; init; }
        public DeliveryEndType? FinishType { get; init; }

    }
}
