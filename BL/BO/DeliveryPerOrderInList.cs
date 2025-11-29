using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DeliveryPerOrderInList
    {

        public int DeliveryId { get; }
        public int? CourierId { get; }
        public string CourierName { get; }
        public ShipmentType ShipmentType { get; }
        public DateTime StartDeliveryTime { get; }
        public DeliveryEndType? FinishType { get; }
        public DateTime? FinishTime { get; }

    }
}
