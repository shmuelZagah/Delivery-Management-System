using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CourierInList
    {
        public int Id { get; }
        public string FullName { get; }
        public bool IsActive { get; }
        public ShipmentType ShipmentType { get; }
        public DateTime WorkStart { get; }
        public int DeliveredOnTimeCount { get;}
        public int DeliveredLateCount { get;  }
        public int? CurrentOrderId { get;  }

    }
}
