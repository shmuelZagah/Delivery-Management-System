using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace BO
{
    public class DeliveryPerOrderInList
    {

        public int DeliveryId { get; init; }
        public int? CourierId { get; init; }
        public string CourierName { get; init; }= string.Empty;
        public ShipmentType ShipmentType { get; init; }
        public DateTime StartDeliveryTime { get; init; }
        public DeliveryEndType? FinishType { get; init; }
        public DateTime? FinishTime { get; init; }

        public override string ToString() => this.ToStringProperty();
    }
}
