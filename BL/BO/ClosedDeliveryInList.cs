using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class ClosedDeliveryInList
    {
        public int DeliveryId { get; }
        public int OrderId { get; }
        public OrderType OrderType { get; }
        public string Address { get; } =string.Empty;
        public ShipmentType ShipmentType { get; }
        public double? ActualDistance { get; }
        public TimeSpan ProcessingDuration { get; }
        public DeliveryEndType FinishType { get; }

    }
}
