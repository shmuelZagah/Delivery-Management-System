using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class Courier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public ShipmentType ShipmentType { get; set; }
    public DateTime StartTime { get; init; }
    public int OrdersDeliveredInTime { get; }

    public int OrdersDeliveredAfterTime { get; }

    public BO.OrderInProgress? InProgress { get; } 

}
