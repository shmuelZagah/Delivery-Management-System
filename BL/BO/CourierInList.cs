using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CourierInList
    {
        public int Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public ShipmentType ShipmentType { get; init; }
        public DateTime StartTime { get; init; }
        public int DeliveredOnTimeCount { get; init; }
        public int DeliveredLateCount { get; init; }
        public int? CurrentOrderId { get; init; }

    }
}
