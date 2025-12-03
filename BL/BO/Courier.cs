using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace BO;
public class Courier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public ShipmentType ShipmentType { get; set; }
    public DateTime StartTime { get; init; }
    public int OrdersDeliveredInTime { get; init; }

    public int OrdersDeliveredAfterTime { get; init; }

    public BO.OrderInProgress? InProgress { get; init; }

    public override string ToString() => this.ToStringProperty();
}
