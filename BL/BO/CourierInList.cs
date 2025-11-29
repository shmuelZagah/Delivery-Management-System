using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CourierInList
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public ShipmentType ShipmentType { get; set; }
        public DateTime WorkStart { get; set; }
        public int DeliveredOnTimeCount { get; set; }
        public int DeliveredLateCount { get; set; }
        public int? CurrentOrderId { get; set; }

    }
}
