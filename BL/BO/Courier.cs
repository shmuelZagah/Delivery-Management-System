using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Courier
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

        public string password { get; set; }
        public bool isActive { get; set; }
        public double? maxDistance { get; set; }
        public ShipmentType hipmentType { get; set; }
        public DateTime StartTime { get; }
        public int ordersDeliveredInTime { get; }

        public int ordersDeliveredAfterTime { get; }

        public BO.OrderInProgress? InProgress { get; } 

    }
}
